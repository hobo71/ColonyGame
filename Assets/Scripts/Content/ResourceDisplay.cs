﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {

    public static bool menuOpened = false;

    public GameObject resIconPrefab;
    public GameObject resIconPrefabExt;
    public GameObject longDisplay;
    public List<Sprite> sprites;
    private Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();
    private Dictionary<string, bool> active = new Dictionary<string, bool>();
    private GameObject[] displays = new GameObject[System.Enum.GetValues(typeof(HPHandler.ressources)).Length];
    private GameObject[] displaysExt = new GameObject[System.Enum.GetValues(typeof(HPHandler.ressources)).Length];
    private Color colorOn = new Color(96 / 255f, 96 / 255f, 96 / 255f, 0.66f);
    private Color colorOff = new Color(96 / 255f, 0.9f, 96 / 255f, 0.66f);
    private static ResourceDisplay instance;
    private int activeElemsCount = 0;

	// Use this for initialization
	void Start () {

        //PlayerPrefs.DeleteAll();
        instance = this;
		icons.Add("wood", sprites[0]);
		icons.Add("stone", sprites[1]);
		icons.Add("scrap", sprites[2]);

		active.Add("wood", true);
		active.Add("stone", true);
		active.Add("scrap", false);

        int curCount = 0;;
        int cPosX = -20;
        int cPosY = -10;

        bool hasPrefs = PlayerPrefs.GetInt("hasPrefsDisplay") == 1;

        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            displays[curCount] = GameObject.Instantiate(resIconPrefab, this.gameObject.transform);
            displays[curCount].transform.localPosition = new Vector3(cPosX, cPosY, 0);

            Sprite icon = null;
            icons.TryGetValue(res.ToString().ToLower(), out icon);
            displays[curCount].GetComponent<Image>().sprite = icon;

            displays[curCount].transform.GetChild(0).GetComponent<Text>().text = ResourceHandler.getAmoumt(res).ToString();
            
            bool activated = false;
            if (!hasPrefs) {
                active.TryGetValue(res.ToString().ToLower(), out activated);
            } else {
                activated = PlayerPrefs.GetInt(res.ToString()) == 0;
                active[res.ToString().ToLower()] = activated;
            }
            
            if (!activated) {
                displays[curCount].SetActive(false); 
                cPosX -= 90;
            } else {
                activeElemsCount++;
            }
            curCount++;
            cPosX += 90;

        }

	}
	
	// Update is called once per frame
	void Update () {
        /*if (woodCounter == null) {
            woodCounter = GameObject.Find("WoodCounter").GetComponent<UnityEngine.UI.Text>();
        }
        if (stoneCounter == null) {
            stoneCounter = GameObject.Find("StoneCounter").GetComponent<UnityEngine.UI.Text>();
        }

		woodCounter.text = ResourceHandler.getAmoumt(HPHandler.ressources.Wood).ToString();
		stoneCounter.text = ResourceHandler.getAmoumt(HPHandler.ressources.Stone).ToString();*/
        
        int i = 0;
        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            displays[i].transform.GetChild(0).GetComponent<Text>().text = ResourceHandler.getAmoumt(res).ToString();
            i++;
        }

	}

    public void menuClicked() {
        menuOpened = true;
        Debug.Log("clicked ressource menu");
        this.gameObject.SetActive(false);
        longDisplay.SetActive(true);
        Time.timeScale = 0.1f;

        int curCount = 0;
        int cPosX = -205;
        int cPosY = -20;

        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            
            displaysExt[curCount] = GameObject.Instantiate(resIconPrefabExt, longDisplay.transform);
            displaysExt[curCount].transform.localPosition = new Vector3(cPosX, cPosY, 0);

            Sprite icon = null;
            icons.TryGetValue(res.ToString().ToLower(), out icon);
            displaysExt[curCount].transform.GetChild(3).GetComponent<Image>().sprite = icon;

            displaysExt[curCount].transform.GetChild(1).GetComponent<Text>().text = ResourceHandler.getAmoumt(res).ToString();
            displaysExt[curCount].transform.GetChild(2).GetComponent<Text>().text = res.ToString();
            
            bool activated = false;
            active.TryGetValue(res.ToString().ToLower(), out activated);
            if (!activated) {
                displaysExt[curCount].transform.GetChild(0).GetComponent<Image>().color = colorOn;
            } else {
                displaysExt[curCount].transform.GetChild(0).GetComponent<Image>().color = colorOff;
            }

            curCount++;
            cPosX += 140;

            if (curCount > 0 && curCount % 3 == 0) {
                cPosX = -205;
                cPosY -= 40;
            }

        }

    }

    public void menuCloseClicked() {

        foreach (GameObject obj in displaysExt) {
            GameObject.Destroy(obj);
        }

        this.gameObject.SetActive(true);
        longDisplay.SetActive(false);
        menuOpened = false;
        Time.timeScale = 1.0f;

        int cPosX = -20;
        int i = 0;
        foreach(HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            displays[i].transform.localPosition = new Vector3(cPosX, -10, 0);
            
            bool activated = true;
            //Debug.Log("elem name:" + display.transform.GetChild(0).GetComponent<Text>().text.ToLower());
            active.TryGetValue(res.ToString().ToLower(), out activated);
            if (!activated) {
                cPosX -= 90;
            }

            cPosX += 90;
            i++;
        }

        save();
    }

    public static void toggleElem(string name) {
        Debug.Log("toggling element: " + name);
        instance.toggleThing(name);

    }
    
    private void toggleThing(string name) {
        instance.active[name.ToLower()] = !instance.active[name.ToLower()];
        bool activated = false;
        active.TryGetValue(name.ToLower(), out activated);
        Debug.Log("target value: " + activated);

        if (!activated && activeElemsCount == 1) {
            Debug.Log("Need at least 1 active item");
            instance.active[name.ToLower()] = !instance.active[name.ToLower()];
            return;
        }
        
        int curCount = 0;

        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            if (res.ToString().Equals(name)) {
                if (!activated) {
                    displaysExt[curCount].transform.GetChild(0).GetComponent<Image>().color = colorOn;
                    displays[curCount].SetActive(false);
                    activeElemsCount--;
                } else {
                    displaysExt[curCount].transform.GetChild(0).GetComponent<Image>().color = colorOff;
                    displays[curCount].SetActive(true);
                    activeElemsCount++;
                    
                }
                return;
            }
            
            curCount++;
        }
    }

    private void save() {
        //0 = enabled; 1 = disabled
        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            int enabled = 0;
            bool activated;
            active.TryGetValue(res.ToString().ToLower(), out activated);
            enabled = activated ? 0 : 1;
            PlayerPrefs.SetInt(res.ToString(), enabled);
            PlayerPrefs.SetInt("hasPrefsDisplay", 1);
            Debug.Log("stored playerpref: " + res.ToString() + enabled + activated);
        }
    }
}
