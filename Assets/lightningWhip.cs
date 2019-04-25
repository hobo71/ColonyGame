using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightningWhip : MonoBehaviour {
    private float progress = 0;

    public float lifeTime;
    // Use this for initialization
    void Start() {
    }

    private void FixedUpdate() {
        progress += Time.deltaTime;
        if (progress > lifeTime) {
            GameObject.Destroy(this.gameObject);
        }
    }
}