using System;
using System.Collections;
using System.Collections.Generic;
using Content.Helpers;
using UnityEngine;

public class BombardTurret : SimpleTower{

	public GameObject projectilePrefab;

	public override void shoot(GameObject target, Vector3 impactPos) {
		var bullet = GameObject.Instantiate(projectilePrefab, this.transform.position + Vector3.up * 10f,
			Quaternion.LookRotation(Vector3.up));
		
		bullet.GetComponent<Projectile>().init(target, impactPos);
		bullet.GetComponent<HPHandler>().faction = this.GetComponent<HPHandler>().faction;
	}

	public override bool useRandom() {
		return false;
	}

	public override Vector3 getImpactPos() {
		var position = enemy.transform.position;
		var toTar = position - this.transform.position;
		var upperPos = this.transform.position + toTar * 0.66f + Vector3.up * toTar.magnitude * 0.33f;
		return enemyCollider.ClosestPoint(upperPos);
	}

	public override ressourceStack[] getCost() {
		
		ressourceStack[] cost = new ressourceStack[2];

		cost[0] = new ressourceStack(100, ressources.Wood);
		cost[1] = new ressourceStack(100, ressources.Stone);
		return cost;
	}
}
