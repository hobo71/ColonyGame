using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MK.XRay
{
    public class AutoEnableObjects : MonoBehaviour
    {
        public Toggle xRayToggle;
        public GameObject basic;
        public GameObject xray;

        void Update()
        {
            if(xRayToggle.isOn)
            {
                basic.SetActive(false);
                xray.SetActive(true);
            }
            else
            {
                basic.SetActive(true);
                xray.SetActive(false);
            }
        }
    }
}
