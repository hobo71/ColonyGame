using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DefaultTurret : MonoBehaviour {

	private GameObject enemy = null;
	private bool isRotated = false;
	private Collider enemyCollider;
	private int barrel = 0;
	private float timePassed = 0f;
	private int updateTimer = 0;
	
	//Debug part
	
	public float range = 16f;
	public float turnSpeed = 0.1f;
	public float angleMustMatch = 5f;
	public float bulletVelocity = 20f;
	public float attacksPerSecond = 6f;
	public float damagePerBullet = 3f;
	public float deviateAngle = 5f;
	public LineRenderer lineRenderer;
	public ParticleSystem emitter;
	public Transform turret;
	public List<GameObject> barrels = new List<GameObject>();
	public List<GameObject> barrelRender = new List<GameObject>();
	private Projectile projectile;

	private void Start() {
		emitter.gameObject.GetComponent<BulletShot>().damagePerBullet = damagePerBullet;
	}

	private void FixedUpdate() {

		if (enemy == null) {
			enemy = findClosestEnemy(range);
			if (enemy != null) {
				enemyCollider = enemy.transform.GetComponent<Collider>();
				projectile = enemy.GetComponent<Projectile>();
			}
		}

		if (enemy == null) {
			lineRenderer.SetPosition(1, new Vector3(30f, 0 ,0));
			return;
		}

		updateTimer++;
		if (updateTimer % 10 == 0 && !isValidTarget(enemy)) {
			enemy = null;
			return;
		}
		
		//attack target
		Vector3 target;
		Vector3 dir;
		if (projectile != null) {
			var estimatedTime = Vector3.Distance(enemy.transform.position, lineRenderer.gameObject.transform.position) / bulletVelocity;
			estimatedTime -= Time.deltaTime * 2;
			target = projectile.futurePosition(estimatedTime);
			
			//rotate
			var ownPos = lineRenderer.gameObject.transform.position;
			dir = target - ownPos;
			rotateTowards(target);
		}
		else {
			target = enemyCollider.ClosestPoint(lineRenderer.gameObject.transform.position);
			dir = rotateTowards(target);
		}
		drawTargetingLine(target);
		if (isRotated) {
			timePassed += Time.deltaTime;
			if (timePassed > 1 / attacksPerSecond) {
				timePassed -= 1 / attacksPerSecond;
				lineRenderer.gameObject.SetActive(true);

				
				shoot(dir);
			}
		}

	}
	private void shoot(Vector3 dir) {
		
		//animate barrel movement
		var xPos = Random.Range(-0.3f, 0.1f);
		barrelRender[barrel % barrels.Count].transform.localPosition = new Vector3(xPos, 0, 0);

		//emit bullet particle
		var vel = dir.normalized * bulletVelocity;
		
		var deviate = new Vector3(Random.Range(-deviateAngle, deviateAngle), Random.Range(-deviateAngle, deviateAngle), Random.Range(-deviateAngle, deviateAngle));
		//deviate /= bulletVelocity;
		vel += deviate;
		
		var emitParams = new ParticleSystem.EmitParams {
			position = barrels[barrel++ % barrels.Count].transform.position, 
			velocity = vel
		};
		emitter.Emit(emitParams, 1);
		
	}

	private Vector3 rotateTowards(Vector3 target) {

		//rotate turret
		var ownPos = lineRenderer.gameObject.transform.position;
		var toTarget = target - ownPos;
		
		Quaternion targetRotation = Quaternion.LookRotation(toTarget);
		targetRotation *= Quaternion.Euler(new Vector3(0, 1, 0) * -90f);

		var rotation = turret.transform.rotation;
		var tar = Quaternion.Lerp(rotation, targetRotation, turnSpeed);

		isRotated = Mathf.Abs(Quaternion.Angle(targetRotation, rotation)) < angleMustMatch;
		
		rotation = tar;
		turret.transform.rotation = rotation;

		return toTarget;
	}

	private void drawTargetingLine(Vector3 target) {
		var dist = Vector3.Distance(this.transform.position, target);
		lineRenderer.SetPosition(1, new Vector3(dist * 2 + 5, 0 ,0));
	}
	

	private GameObject findClosestEnemy(float range) {
		
		//gather potential targets
		HPHandler.Faction self = this.GetComponent<HPHandler>().faction;
		GameObject target = null;
		float closestDistance = range;
		foreach (HPHandler.Faction item in Enum.GetValues(typeof(HPHandler.Faction))) {
			if (item == self || item == HPHandler.Faction.Neutral) {
				continue;
			}
			
			//loop thorugh faction members
			var factionMembers = HPHandler.factionMembers;
			foreach (var enemy in factionMembers[item]) {
				var dist = Vector3.Distance(this.gameObject.transform.position, enemy.gameObject.transform.position);
				if (dist < closestDistance && isValidTarget(enemy.gameObject)) {
					target = enemy.gameObject;
					closestDistance = dist;
				}
			}
		}

		return target;
	}
	
	private bool isValidTarget(GameObject target) {

		var ownPos = lineRenderer.gameObject.transform.position;
		var targetPos = target.GetComponent<Collider>().ClosestPoint(ownPos);
		var dir = targetPos - ownPos;

		var ray = new Ray(ownPos, dir);
		RaycastHit hit;
		var success = Physics.Raycast(ray, out hit, range);

		if (success && hit.transform.gameObject.Equals(target)) {
			return true;
		}
		
		//print("invalid raycast: " + target.name + " hit=" + hit.transform.gameObject.name);
		
		return false;
	}
}
