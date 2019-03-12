using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLightRotation : MonoBehaviour {

	private static readonly float counterTimer = 120;

	private static float intensity = 1.0f;
	private static int counter = 0;

	public static float getIntensity() {
		return intensity;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		counter++;
		if (counter < counterTimer) {
			return;
		}

		counter = 0;
		float speedHack = 1f;
		this.transform.Rotate(new Vector3(0, speedHack * 0.4f * 1.5f * Time.deltaTime * counterTimer, speedHack * 0.4f * 0.3f * Time.deltaTime * counterTimer), Space.World);

		intensity = (this.transform.localRotation.eulerAngles.x % 360 + this.transform.rotation.z % 360) / 60f + 0.45f;
		this.gameObject.GetComponent<Light>().intensity = intensity;
	}
}
