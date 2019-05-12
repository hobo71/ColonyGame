using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Editor {
    public class Placement_Generator : EditorWindow {
        private string dispName = "Insert Name";
        private string Description = "Insert description here";
        private string path = "Assets/Prefabs/Structures/";
        private GameObject prefabGO;
        private Material placementMat;
        private Sprite icon;
        private bool overridePrice = true;

        private static Material placementMatS = null;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Generate Placement")]
        static void Init() {
            // Get existing open window or if none, make a new one:
            Placement_Generator window = (Placement_Generator) EditorWindow.GetWindow(typeof(Placement_Generator));
            window.Show();
        }

        void OnGUI() {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            dispName = EditorGUILayout.TextField("Display Name", dispName);
            Description = EditorGUILayout.TextField("Description", Description);
            path = EditorGUILayout.TextField("path", path);
            prefabGO = (GameObject) EditorGUILayout.ObjectField("Prefab", prefabGO, typeof(GameObject), true);
            placementMat = (Material) EditorGUILayout.ObjectField("Placement Mat", placementMat, typeof(Material), false);
            icon = (Sprite) EditorGUILayout.ObjectField("Placement Mat", icon, typeof(Sprite), false);

            if (placementMatS == null && placementMat != null) {
                placementMatS = placementMat;
            }
            
            
            if(GUILayout.Button("Generate")) {
                
                //mark scene as dirty, give "undo" action
                Undo.RecordObject(GameObject.Find("Terrain"), "Generate Placement");
                
                Debug.Log("generating placement for: " + prefabGO.name);
                var clone = GameObject.Instantiate(prefabGO, prefabGO.transform.position, Quaternion.identity);
                clone.gameObject.name = prefabGO.name + "_Placement";
                //destroy scripts
                List<Component> components = new List<Component>();
                foreach (var monoBehaviour in clone.GetComponents<MonoBehaviour>()) components.Add(monoBehaviour);
                foreach (var monoBehaviour in clone.GetComponents<Collider>()) components.Add(monoBehaviour);
                foreach (var monoBehaviour in clone.GetComponentsInChildren<MonoBehaviour>()) components.Add(monoBehaviour);
                foreach (var monoBehaviour in clone.GetComponentsInChildren<Collider>()) components.Add(monoBehaviour);
                foreach (var elem in components) {
                    GameObject.DestroyImmediate(elem);
                }
                
                List<Renderer> renderers = new List<Renderer>();
                foreach (var component in clone.GetComponents<Renderer>()) renderers.Add(component);
                foreach (var component in clone.GetComponentsInChildren<Renderer>()) renderers.Add(component);
                foreach (var renderer in renderers) {
                    renderer.material = placementMat;
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                    renderer.allowOcclusionWhenDynamic = false;
                }

                if (prefabGO.GetComponent<MeshFilter>() == null) {
                    prefabGO.AddComponent<MeshFilter>();
                }
                var prefab = PrefabUtility.CreatePrefab(path + prefabGO.name + ".prefab", prefabGO);
                var placement = PrefabUtility.CreatePrefab(path + clone.name + ".prefab", clone);
                
                
                //generate building manager data
                var data = new BuildingManager.placementData();
                data.cost = new List<ressourceStack>
                    {new ressourceStack(50, ressources.Stone), new ressourceStack(50, ressources.Wood)};
                data.desc = Description;
                data.icon = icon;
                data.prefab = prefab;
                data.placement = placement;
                data.dispName = dispName;

                var manager = GameObject.Find("Terrain").GetComponent<BuildingManager>();
                manager.autoPrefabs.Add(data);
                GameObject.DestroyImmediate(clone);
                
                //add prefab to saveload
                var saveLoad = GameObject.Find("Terrain").GetComponent<SaveLoad>();
                saveLoad.prefabs.Add(prefab);

                //make editor record changes, so they dont get lost
                PrefabUtility.RecordPrefabInstancePropertyModifications(GameObject.Find("Terrain"));
                EditorUtility.SetDirty(saveLoad);
                EditorUtility.SetDirty(manager);
                
            }
        }
    }
}