using System;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Content.Helpers.Combat {
	public class UnitSelection : MonoBehaviour {
		
		public List<CombatUnit> activeSelected = new List<CombatUnit>();
		public GameObject cancelBut;
		public Animator Anim;

		private Camera cam;
		private Vector2 referenceRes;
		private RectTransform rectAnim;

		private void Awake() {
			cam = GameObject.Find("Main Camera").GetComponent<Camera>();
			referenceRes = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution;
			rectAnim = GameObject.Find("UnitSelectorAnim").GetComponent<RectTransform>();
		}

		public void startSelection() {
			Debug.Log("starting unit selection");
			Time.timeScale = 0.1f;
			Scene_Controller.getInstance().hideAllUI();
			cancelBut.SetActive(true);
			
			GameObject.Find("Main Camera").GetComponent<clickDetector>().setNextClickAction(selectTarget);
			Anim.SetTrigger("play");
			selectUnitsOnScreen();
			Time.timeScale = 0.1f;
		}

		private void selectUnitsOnScreen() {
			activeSelected.Clear();
			
			var potTargets = HPHandler.factionMembers[HPHandler.Faction.Terran];
			foreach (var unit in potTargets) {
				if (unit.GetComponent<CombatUnit>() == null) {
					continue;
				}
				var sPos = cam.WorldToScreenPoint(unit.transform.position);
				
				var success = RectTransformUtility.RectangleContainsScreenPoint(rectAnim, sPos);
				if (!success) continue;
				activeSelected.Add(unit.GetComponent<CombatUnit>());
					
				SkinnedMeshOutline effect = unit.AddComponent<SkinnedMeshOutline>();
				effect.OutlineColor = Color.blue;
				effect.OutlineWidth = 8.0f;
//				var scaledPosX = sPos.x / Screen.width * referenceRes.x;
//				var scaledPosY = sPos.y / Screen.height * referenceRes.y;
			}

			if (activeSelected.Count < 1) {
				restoreNormal();
			}
		}

		public void restoreNormal() {
			Time.timeScale = 1f;
			Scene_Controller.getInstance().restoreDefaultUI();
			cancelBut.SetActive(false);

			foreach (var unit in activeSelected) {
				try {
					GameObject.Destroy(unit.GetComponent<SkinnedMeshOutline>());
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			}
		}

		public void selectTarget(GameObject target) {
			print("selected target: " + target.name);
			restoreNormal();
			
			//order units
			NavMeshHit hit;
			var success = NavMesh.SamplePosition(target.transform.position, out hit, 20f, NavMesh.AllAreas);
			print("found target: " + success);
			if (success) {
				foreach (var elem in activeSelected) {
					elem.moveTo(hit.position);
				}
				Notification.createNotification(target, Notification.sprites.Targeting, "");
			}
		}
	}
}
