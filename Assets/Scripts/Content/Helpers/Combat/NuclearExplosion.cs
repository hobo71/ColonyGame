using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearExplosion : MonoBehaviour {

    public float Damage;
    public float range;

    private ParticleSystem partRing;
    private Transform sphere;
    private float progress = 0;
    private float duration;

    // Use this for initialization
    void Start() {
        duration = range / 8f;
        partRing = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        var main = partRing.main;
        main.duration = duration;
        main.startLifetime = duration;
        main.startSize = range * 2.5f;

        sphere = this.transform.GetChild(1);
    }
    void FixedUpdate() {
        if (progress > duration) {
			sphere.GetComponent<MeshRenderer>().enabled = false;
			var emission = sphere.GetChild(0).GetComponent<ParticleSystem>().emission;
			emission.enabled = false;
            return;
        }

        //scale sphere
		var scale = progress / duration * range * 2;
        sphere.localScale = new Vector3(scale, scale / 2, scale);

        //destroy stuff
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, range * (progress / duration));
        int i = 0;
        while (i < hitColliders.Length) {
            var hp = hitColliders[i].gameObject.GetComponent<HPHandler>();
            if (hp != null) {
                hp.HP -= Damage * Time.deltaTime / duration * 2;
            }
            i++;
        }

        progress += Time.deltaTime;
    }
}
