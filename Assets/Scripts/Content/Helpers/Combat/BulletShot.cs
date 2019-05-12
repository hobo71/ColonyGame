using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShot : MonoBehaviour {

	private ParticleSystem PSystem;
	private ParticleCollisionEvent[] CollisionEvents;

	public float damagePerBullet = 1f;

	// Use this for initialization
	void Start() {
		CollisionEvents = new ParticleCollisionEvent[8];
		PSystem = this.GetComponent<ParticleSystem>();
	}

	public void OnParticleCollision(GameObject other) {
		//print("bullet hit target:" + other.name);

		var HP = other.GetComponent<HPHandler>();
		if (HP != null) {
			HP.HP -= damagePerBullet;
		}
	}
}