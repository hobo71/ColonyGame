using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.XRay
{
    public class InfoTextDisable : MonoBehaviour
    {
        public GameObject objectToDestroy;
        void Update()
        {
            if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            {
                Destroy(objectToDestroy);
                Destroy(this);
            }
        }
    }
}
