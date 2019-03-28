using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaveLoad : MonoBehaviour {

    public List<GameObject> prefabs;
    public GameObject loadingIcon;

    public static bool creatingNew = false;

    void Start() {
        InvokeRepeating("autoSave", 120, 120);
    }

    void autoSave() {
        print("starting autosave");
        save(Scene_Controller.saveName);
    }

    public void save(string name) {

        StartCoroutine(saveASync(name));
    }

    private IEnumerator saveASync(string name) {

        loadingIcon.SetActive(true);
        float curTime = Time.realtimeSinceStartup;
        List<System.Object> toSave = new List<System.Object>();
        yield return new WaitForSeconds(0.1f);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects) {
            try {
                if (obj.layer == 10) {
                    continue;
                }
            } catch (Exception ex) {
                print("object probably has been destroyed");
                continue;
            }

            yield return new WaitForSeconds(0.005f);
            try {
                string prefabName = obj.name;
                prefabName = prefabName.Replace("Clone", "");
                prefabName = prefabName.Replace(" ", "");
                prefabName = prefabName.Replace("(", "");
                prefabName = prefabName.Replace(")", "");
                prefabName = Regex.Replace(prefabName, @"[\d-]", string.Empty);
                print("searching for prefab: " + prefabName);

                GameObject prefabFound = null;
                foreach (GameObject prefab in prefabs) {
                    if (prefab.name.Equals(prefabName)) {
                        prefabFound = prefab;
                        break;
                    }
                }

                if (prefabFound == null) {
                    print("found no valid prefab, going to next item, searched for: " + prefabName);
                    continue;
                }

                GameObjectInfo info = new GameObjectInfo(obj);
                toSave.Add(info);
            } catch (Exception ex) {
                print("Error while saving thing: " + ex);
            }

        }

        foreach (System.Object obj in toSave) {
            print(obj);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + name);
        bf.Serialize(file, toSave);
        file.Close();
        print("saving done..., saved To: " + Application.persistentDataPath);

        float timeNeeded = Time.realtimeSinceStartup - curTime;
        print("Time needed: " + timeNeeded);
        loadingIcon.SetActive(false);
    }

    [Serializable]
    private class GameObjectInfo {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public SerializableVector3 scale;
        public string prefabName;
        public string originalName;
        public List<SerializationInfo> info = new List<SerializationInfo>();

        public GameObjectInfo(GameObject obj) {
            this.position = obj.transform.position;
            this.rotation = obj.transform.rotation;
            this.scale = obj.transform.localScale;
            this.prefabName = obj.name;
            this.originalName = obj.name;

            prefabName = prefabName.Replace("Clone", "");
            prefabName = prefabName.Replace(" ", "");
            prefabName = prefabName.Replace("(", "");
            prefabName = prefabName.Replace(")", "");
            prefabName = Regex.Replace(prefabName, @"[\d-]", string.Empty);
            SerializableInfo[] data = obj.GetComponents<SerializableInfo>();

            foreach (SerializableInfo elem in data) {
                info.Add(elem.getSerialize());
            }

            print("found ser. Info on obj: " + info.Count);
        }

        public override string ToString() {
            return prefabName + " scale=" + scale + " rotation=" + rotation + " position=" + position;
        }
    }

    [System.Serializable]
    public struct SerializableVector3 {
        /// <summary>
        /// x component
        /// </summary>
        public float x;

        /// <summary>
        /// y component
        /// </summary>
        public float y;

        /// <summary>
        /// z component
        /// </summary>
        public float z;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        /// <param name="rZ"></param>
        public SerializableVector3(float rX, float rY, float rZ) {
            x = rX;
            y = rY;
            z = rZ;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        /// <summary>
        /// Automatic conversion from SerializableVector3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector3(SerializableVector3 rValue) {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        /// <summary>
        /// Automatic conversion from Vector3 to SerializableVector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableVector3(Vector3 rValue) {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }
    }

    [System.Serializable]
    public struct SerializableQuaternion {
        /// <summary>
        /// x component
        /// </summary>
        public float x;

        /// <summary>
        /// y component
        /// </summary>
        public float y;

        /// <summary>
        /// z component
        /// </summary>
        public float z;

        /// <summary>
        /// w component
        /// </summary>
        public float w;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        /// <param name="rZ"></param>
        /// <param name="rW"></param>
        public SerializableQuaternion(float rX, float rY, float rZ, float rW) {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }

        /// <summary>
        /// Automatic conversion from SerializableQuaternion to Quaternion
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Quaternion(SerializableQuaternion rValue) {
            return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        /// <summary>
        /// Automatic conversion from Quaternion to SerializableQuaternion
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableQuaternion(Quaternion rValue) {
            return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }
    }

    void OnLevelWasLoaded(int level) {

        if (level != 3) {
            return;
        }

        print("level was loaded");

        //Destroy testing objects
        GameObject.Destroy(GameObject.Find("TestingObjects"));

        StartCoroutine(Load());
    }

    public IEnumerator Load() {

        print("loading last save");
        List<System.Object> list = null;

        //load "newGame" save
        if (creatingNew) {
            creatingNew = false;
            print("found createNew flag, loading different save! Accessing: " + Application.streamingAssetsPath + "/newGameSave");

            var filePath = Application.streamingAssetsPath + "/newGameSave";
            var www = new WWW(filePath);
            yield return www;

            if (!string.IsNullOrEmpty(www.error)) {
                Debug.LogError("Can't read newGameSave");
            }

            BinaryFormatter bf = new BinaryFormatter();
            var ms = new MemoryStream(www.bytes);
            print("opened file");
            list = (List<System.Object>)bf.Deserialize(ms);

            print("loading done!");

            //load last save
        } else if (File.Exists(Application.persistentDataPath + "/default")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/default", FileMode.Open);
            list = (List<System.Object>)bf.Deserialize(file);
            file.Close();

            print("loading done!");
        }



        if (list == null) {
            print("Error, no save game found");
            yield break;
        }

        print("deserializing...");

        foreach (System.Object obj in list) {
            print(obj);
        }

        print("deserialization done...");

        List<GameObject> spawns = new List<GameObject>();

        foreach (System.Object obj in list) {
            GameObjectInfo info = (GameObjectInfo)obj;

            GameObject prefab = null;

            foreach (GameObject elem in prefabs) {
                if (elem.name.Equals(info.prefabName)) {
                    prefab = elem;
                    break;
                }
            }

            print("creating " + info.prefabName);
            GameObject spawned = GameObject.Instantiate(prefab, info.position, info.rotation);
            spawned.name = info.originalName;
            spawned.transform.localScale = info.scale;
            /*for (int i = 0; i < info.info.Count; i++) {
                //spawned.GetComponents<SerializableInfo>()[i].handleDeserialization(info.info[i]);
                ((SerializableInfo) spawned.GetComponent(info.info[i].scriptTarget)).handleDeserialization(info.info[i]);
            }*/
            spawns.Add(spawned);

        }

        print("all objects spawned, initing scripts...");

        for (int j = 0; j < spawns.Count; j++) {
            GameObjectInfo info = (GameObjectInfo)list[j];

            for (int i = 0; i < info.info.Count; i++) {
                //spawned.GetComponents<SerializableInfo>()[i].handleDeserialization(info.info[i]);
                ((SerializableInfo)spawns[j].GetComponent(info.info[i].scriptTarget)).handleDeserialization(info.info[i]);
            }

        }

        print("gameobjects added");
    }

    public interface SerializableInfo {
        SerializationInfo getSerialize();
        void handleDeserialization(SerializationInfo info);
    }

    [System.Serializable]
    public abstract class SerializationInfo {
        public abstract string scriptTarget { get; }
    }
}
