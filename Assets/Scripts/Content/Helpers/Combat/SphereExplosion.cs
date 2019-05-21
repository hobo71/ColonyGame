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
		var alreadyHit = new List<GameObject>();
		int i = 0;
		while (i < hitColliders.Length) {
			var hp = hitColliders[i].gameObject.GetComponent<HPHandler>();
			if (hp != null && !alreadyHit.Contains(hp.gameObject)) {
				hp.HP -= Damage;
				alreadyHit.Add(hp.gameObject);
			}
			i++;
		}
	}

	private void FixedUpdate() {
		timePassed += Time.deltaTime;
		if (timePassed > 3f) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
