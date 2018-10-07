using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.XRay
{
    public class MKXRayCopyPropertiesRuntime : MonoBehaviour
    {

        private Material src;
        private Material[] dst;

        public void Start()
        {
            src = transform.root.GetComponent<MeshRenderer>().sharedMaterial;

            Material[] tmp = GetComponent<MeshRenderer>().sharedMaterials;
            dst = new Material[tmp.Length];

            for(int i = 0; i < dst.Length; i++)
            {
                dst[i] = new Material(tmp[i]);
            }

            GetComponent<MeshRenderer>().sharedMaterials = dst;
        }

        void Update()
        {
            for (int i = 0; i < dst.Length; i++)
            {
                MKXRayFreeMaterialHelper.SetEmissionColor(dst[i], MKXRayFreeMaterialHelper.GetEmissionColor(src));
                MKXRayFreeMaterialHelper.SetXRayInside(dst[i], MKXRayFreeMaterialHelper.GetXRayInside(src));
                MKXRayFreeMaterialHelper.SetXRayRimSize(dst[i], MKXRayFreeMaterialHelper.GetXRayRimSize(src));
            }
        }
    }
}
