using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MK.XRay
{
    public class MKXRayDemoControl : MonoBehaviour
    {
        private static bool settingsUsed = false;
        public static bool SettingsUsed
        {
            get { return settingsUsed; }
        }

        private static int currentModel = -1;
        public static int CurrentModel
        {
            get { return currentModel; }
        }

        [SerializeField]
        private List<Material> baseMaterials = new List<Material>();
        private List<Material> currentMaterials = new List<Material>();

        [SerializeField]
        private List<GameObject> gameObjects = new List<GameObject>();
        private List<MeshRenderer> renderers = new List<MeshRenderer>();


        [SerializeField]
        private Slider emissionIntensitySlider;
        private float emissionIntensity;
        public float EmissionIntensity
        {
            get { return emissionIntensity; }
            set
            {
                emissionIntensity = value;
                MKXRayFreeMaterialHelper.SetEmissionColor(currentMaterials[currentModel], Color.Lerp(Color.black, new Color(2, 2, 2, 1), emissionIntensity));
            }
        }

        [SerializeField]
        private Slider xRayInsideSlider;
        private float xRayInside;
        public float XRayInside
        {
            get { return xRayInside; }
            set
            {
                xRayInside = value;
                MKXRayFreeMaterialHelper.SetXRayInside(currentMaterials[currentModel], xRayInside);
            }
        }

        [SerializeField]
        private Slider xRayRimSizeSlider;
        private float xRayRimSize;
        public float XRayRimSize
        {
            get { return xRayRimSize; }
            set
            {
                xRayRimSize = value;
                MKXRayFreeMaterialHelper.SetXRayRimSize(currentMaterials[currentModel], xRayRimSize);
            }
        }

        private void SetupMaterials()
        {
            currentMaterials.Clear();
            renderers.Clear();
            foreach (GameObject go in gameObjects)
            {
                renderers.Add(go.GetComponent<MeshRenderer>());
            }
            foreach (Material m in baseMaterials)
            {
                currentMaterials.Add(new Material(m));
            }
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material = currentMaterials[i];
            }
        }

        private void Awake()
        {
            SetupMaterials();
            ChangeModel();
        }

        public void ChangeModel()
        {
            currentModel++;
            if (currentModel > gameObjects.Count - 1)
                currentModel = 0;
            foreach (GameObject go in gameObjects)
                go.SetActive(false);
            gameObjects[currentModel].SetActive(true);
            SetValuesFromMaterial();
            SetMaterialSettingsToSliders();
        }

        private void SetMaterialSettingsToSliders()
        {
            emissionIntensitySlider.value = emissionIntensity;
            xRayInsideSlider.value = xRayInside;
            xRayRimSizeSlider.value = xRayRimSize;
        }

        private void SetValuesFromMaterial()
        {
            emissionIntensity = MKXRayFreeMaterialHelper.GetEmissionColor(currentMaterials[currentModel]).r / 2.0f;
            xRayInside = MKXRayFreeMaterialHelper.GetXRayInside(currentMaterials[currentModel]);
            xRayRimSize = MKXRayFreeMaterialHelper.GetXRayRimSize(currentMaterials[currentModel]);
        }

        private void Update()
        {

#if !UNITY_ANDROID || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                settingsUsed = true;
            }

            if (Input.GetMouseButtonUp(0))
                settingsUsed = false;
#else
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            settingsUsed = true;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            settingsUsed = false;
        }
#endif
        }
    }
}