using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoClicked : MonoBehaviour {

    public static GameObject script;
    private static String actName = "";

    public GameObject ClickableInfo;
    public GameObject uiCanvas;
    public GameObject menuButton;
    public GameObject buildingMenu;

    private float[] prices;

    public static InfoClicked getInstance() {
        return script.GetComponent<InfoClicked>();
    }

    void Start () {
	    ClickableInfo = uiCanvas;
        if (this.gameObject.transform.parent.name.Equals("ClickableInfo")) {
            script = this.gameObject;
        }

        close();
	}

    public void setPrice(float[] prices) {
        this.prices = prices;
    }

    void FixedUpdate() {
        try {
            if (!transform.parent.Find("Cost").gameObject.activeSelf) {
                return;
            }
        } catch (NullReferenceException ex) {
            return;
        }

        if (prices[0] <= ResourceHandler.getAmoumt(HPHandler.ressources.Wood) && prices[1] <= ResourceHandler.getAmoumt(HPHandler.ressources.Stone)) {
            setBuildable(true);
        } else {
            setBuildable(false);
        }
    }

	public void closeClicked() {
        Debug.Log("Closing Info Window");
        clickDetector.overlayClicked = true;
        close();
    }

    private void setBuildable(bool buildable) {
        //Debug.Log("setting build interactible: " + buildable + " price: " + prices[0] + " available: " + ResourceHandler.totalWood);
        this.transform.parent.Find("DoBuild").GetComponent<UnityEngine.UI.Button>().interactable = buildable;
    }

    public void setTitle(string text, string actName) {
        transform.parent.Find("Title").GetComponent<UnityEngine.UI.Text>().text = text;
        InfoClicked.actName = actName;
    }

    public void setDesc(string desc) {
        transform.parent.Find("Description").GetComponent<UnityEngine.UI.Text>().text = desc;
    }

    public void setPrice(string price) {
        transform.parent.Find("Cost").GetComponent<UnityEngine.UI.Text>().text = price;
    }

    private void close() {
        ClickableInfo.SetActive(false);
        if (!buildingMenu.activeSelf) {
            menuButton.SetActive(true);
        }
        
    }

    public void show() {
        ClickableInfo.SetActive(true);
        menuButton.SetActive(false);
        GameObject.Find("Terrain").GetComponent<Building>().closeClicked();
    }

    public void handleBuild() {
        
        Debug.Log("Clicked build button on preview");
        clickDetector.overlayClicked = true;
        ClickableInfo.SetActive(false);
        
        GameObject.Find("Terrain").GetComponent<Building>().buildClicked(actName);

    }
}
