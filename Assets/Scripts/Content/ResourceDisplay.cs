using System;
using System.Collections;
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

    //used for ressource type selectors
    public static Action<List<HPHandler.ressources>> closeCallback = null;
    public static List<HPHandler.ressources> callbackList = null;
    public static inventory callbackInv = null;

	// Use this for initialization
	void Start () {

        //PlayerPrefs.DeleteAll();
        instance = this;
		icons.Add("wood", sprites[0]);
		icons.Add("stone", sprites[1]);
		icons.Add("scrap", sprites[2]);
		icons.Add("trees", sprites[3]);
		icons.Add("iron", sprites[4]);
		icons.Add("oreiron", sprites[5]);
		icons.Add("gold", sprites[6]);
		icons.Add("oregold", sprites[7]);
		icons.Add("iridium", sprites[8]);
		icons.Add("oreiridium", sprites[9]);

		active.Add("wood", true);
		active.Add("stone", true);
		active.Add("scrap", false);
		active.Add("trees", false);
		active.Add("iron", true);
		active.Add("oreiron", false);
		active.Add("gold", false);
		active.Add("oregold", false);
		active.Add("iridium", false);
		active.Add("oreiridium", false);

        int curCount = 0;;
        int cPosX = -20;
        int cPosY = -10;

        bool hasPrefs = PlayerPrefs.GetInt("hasPrefsDisplay") == 1;

        //create elements
        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            displays[curCount] = GameObject.Instantiate(resIconPrefab, this.gameObject.transform);
            displays[curCount].transform.localPosition = new Vector3(cPosX, cPosY, 0);

            Sprite icon = null;
            icons.TryGetValue(res.ToString().ToLower(), out icon);
            displays[curCount].GetComponent<Image>().sprite = icon;

            var amount = (int) (Math.Round(ResourceHandler.getAmoumt(res)));

            displays[curCount].transform.GetChild(0).GetComponent<Text>().text = amount.ToString();
            
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

    public static void openListSelect(List<HPHandler.ressources> elems, Action<List<HPHandler.ressources>> elem, inventory inv) {
        print("opening list select!");
        callbackList = elems;
        callbackInv = inv;
        closeCallback = elem;
        instance.menuClicked();
        //TODO set current actives to list
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
            var amount = (int) (Math.Round(ResourceHandler.getAmoumt(res)));
            displays[i].transform.GetChild(0).GetComponent<Text>().text = amount.ToString();
            i++;
        }

	}

    public void menuClicked() {
        menuOpened = true;
        Debug.Log("clicked ressource menu");

        if (closeCallback != null) {
            setToList(callbackList);
            //setToInv(callbackInv);
        } else {
            foreach(HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
                var activated = PlayerPrefs.GetInt(res.ToString()) == 0;
                active[res.ToString().ToLower()] = activated;
            }
        }

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

            if (closeCallback == null)
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

        
        if (closeCallback != null) {
            setToInv(callbackInv);
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

        //if callback is defined, do not edit current ressource displays
        if (closeCallback != null) {
            print("closed ressource display, using callback!");
            closeCallback.Invoke(callbackList);
            closeCallback = null;
            callbackList = null;
            return;
        }

        //set current display values again
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

    private void setToList(List<HPHandler.ressources> list) {
		var listcont = "";
		foreach (var part in list) {
			listcont += part.ToString() + "; ";
		}
		print("setting to list: " + listcont);
        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            if (list.Contains(res)) {
                active[res.ToString().ToLower()] = true;
            } else {
                active[res.ToString().ToLower()] = false;
            }
        }
    }

    private void setToInv(inventory inv) {
        
		print("setting to inv: " + inv);
        int curCount = 0;
        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            var amount = inv.getAmount(res);
            displaysExt[curCount].transform.GetChild(1).GetComponent<Text>().text = amount.ToString();
            curCount++;
        }
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
                    //deselecting elem
                    displaysExt[curCount].transform.GetChild(0).GetComponent<Image>().color = colorOn;
                    if  (closeCallback == null) {
                        displays[curCount].SetActive(false);
                    } else {
                        callbackList.Remove(res);
                    }
                    
                    activeElemsCount--;
                } else {
                    //selecting elem
                    displaysExt[curCount].transform.GetChild(0).GetComponent<Image>().color = colorOff;
                    if  (closeCallback == null) {
                        displays[curCount].SetActive(true);
                    } else {
                        callbackList.Add(res);
                    }
                    
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
