using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script on UI elements, active is the gameobject that opened it
public class ConveyorConfigurator : MonoBehaviour {

	private pipeHandler active = null;
	public GameObject titleLeft;
	public GameObject titleRight;
	public GameObject drainLeft;
	public GameObject drainAllLeft;
	public GameObject drainRight;
	public GameObject drainallRight;

	// Use this for initialization
	void Start () {
		//clickDetector.menusOpened++;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void closePressed() {
		this.gameObject.SetActive(false);
		clickDetector.menusOpened--;
		var data = active.getData();
		data.drainAllLeft = drainAllLeft.GetComponent<Toggle>().isOn;
		data.drainAllRight = drainallRight.GetComponent<Toggle>().isOn;
		data.drainLeft = drainLeft.GetComponent<Toggle>().isOn;
		data.drainRight = drainRight.GetComponent<Toggle>().isOn;
	}

	private void selectionDone(List<HPHandler.ressources> selected) {
		var listcont = "";
		foreach (var part in selected) {
			listcont += part.ToString() + "; ";
		}
		print("ressource select done, got list: " + listcont);
		this.gameObject.SetActive(true);
		active.getData().drainingLeft = selected;
	}

	public void setInstance(pipeHandler handler) {
		this.active = handler;
		this.titleLeft.GetComponent<Text>().text = active.getData().from.gameObject.name;
		this.titleRight.GetComponent<Text>().text = active.getData().to.gameObject.name;
	}

	public void confLeftClicked() {
		ResourceDisplay.openListSelect(active.getData().drainingLeft, selectionDone);
		this.gameObject.SetActive(false);
	}
}
