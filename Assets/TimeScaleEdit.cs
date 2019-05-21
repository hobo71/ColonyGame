using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleEdit : MonoBehaviour {

    public float timeScale = 1f;

    private void Update() {
        Time.timeScale = timeScale;
    }
}
