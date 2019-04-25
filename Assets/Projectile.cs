using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private GameObject target;
	private Vector3 startPos;
	private Vector3 impactPos;
	private float progress = 0;
	private float flightTime;
	private float maxHeight;
	
	public float speed;
	public float damage;
	public float damageRadius;
	public GameObject explosionPrefab;
	
	public void init([NotNull] GameObject target, Vector3 impactPos) {
		if (target == null) throw new ArgumentNullException("target");
		this.target = target;
		this.impactPos = impactPos;
		this.startPos = this.transform.position;
		this.flightTime = (impactPos - startPos).magnitude / speed;
		maxHeight = 4f * flightTime;
	}

	private void FixedUpdate() {
		//formula: 4*x - 4*x^2
		var prog = progress / flightTime;
		var curPos = startPos + (impactPos - startPos) * prog;
		var heightAdd = Vector3.up * maxHeight * (float) (4 * prog - 4 * Math.Pow(prog, 2));
		curPos += heightAdd;

		if (!float.IsNaN(curPos.x)) {
			this.transform.position = curPos;
		}


		progress += Time.deltaTime;

	}

	//returns the estimated position in now+delta time, used to make turrets more accurate
	public Vector3 futurePosition(float delta) {
		var fTime = progress + delta;
		fTime /= flightTime;
		var fPos = startPos + (impactPos - startPos) * fTime;
		var heightAdd = Vector3.up * maxHeight * (float) (4 * fTime - 4 * Math.Pow(fTime, 2));
		fPos += heightAdd;
		
		//print("future position in " + delta + " is: " + fPos);
		return fPos;
	}

	private void OnCollisionEnter(Collision other) {
		print("projectile hit: " + other.gameObject.name);
		
		var explosion = GameObject.Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
		explosion.GetComponent<SphereExplosion>().range = damageRadius;
		explosion.GetComponent<SphereExplosion>().Damage = damage;
		GameObject.Destroy(this);
	}
}
