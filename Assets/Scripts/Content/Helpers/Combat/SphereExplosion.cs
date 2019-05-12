using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereExplosion : MonoBehaviour {
	
	public float Damage;
	public float range;
	private float timePassed = 0f;
	
	// Use this for initialization
	void Start () {
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, range);
		int i = 0;
		while (i < hitColliders.Length) {
			var hp = hitColliders[i].gameObject.GetComponent<HPHandler>();
			if (hp != null) {
				hp.HP -= Damage;
			}
			i++;
		}
	}

	private void FixedUpdate() {
		timePassed += Time.deltaTime;
		if (timePassed > 3f) {
			Destroy(this.gameObject);
		}
	}
}
