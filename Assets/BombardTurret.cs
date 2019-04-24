using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardTurret : MonoBehaviour {

	public float maxRange;
	public float minRange;
	public float attacksPerSecond;
	public GameObject projectilePrefab;

	private GameObject enemy;
	private Collider enemyCollider;
	private float timeElapsed = 0f;

	// Use this for initialization
	void Start () {
	}

	private void FixedUpdate() {
		if (enemy == null) {
			enemy = findClosestEnemy();
			if (enemy != null) {
				enemyCollider = enemy.GetComponent<Collider>();
			}
		}

		if (enemy == null) {
			return;
		}

		var position = enemy.transform.position;
		var toTar = position - this.transform.position;
		var upperPos = this.transform.position + toTar * 0.66f + Vector3.up * toTar.magnitude * 0.33f;
		var impactPos = enemyCollider.ClosestPoint(upperPos);

		timeElapsed += Time.deltaTime;
		if (timeElapsed >= 1 / attacksPerSecond) {
			timeElapsed -= 1 / attacksPerSecond;
			shoot(enemy, impactPos);
		}
	}

	private void shoot(GameObject target, Vector3 impactPos) {
		var bullet = GameObject.Instantiate(projectilePrefab, this.transform.position + Vector3.up * 10f,
			Quaternion.LookRotation(Vector3.up));
		
		bullet.GetComponent<Projectile>().init(target, impactPos);
		bullet.GetComponent<HPHandler>().faction = this.GetComponent<HPHandler>().faction;
	}
	
	private GameObject findClosestEnemy() {
		
		//gather potential targets
		HPHandler.Faction self = this.GetComponent<HPHandler>().faction;
		GameObject target = null;
		float closestDistance = maxRange;
		foreach (HPHandler.Faction item in Enum.GetValues(typeof(HPHandler.Faction))) {
			if (item == self || item == HPHandler.Faction.Neutral) {
				continue;
			}
			
			//loop through faction members
			var factionMembers = HPHandler.factionMembers;
			foreach (var enemy in factionMembers[item]) {
				var dist = Vector3.Distance(this.gameObject.transform.position, enemy.gameObject.transform.position);
				if (dist < closestDistance && dist > minRange) {
					target = enemy.gameObject;
					closestDistance = dist;
				}
			}
		}

		return target;
	}
}
