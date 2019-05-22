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

	public void closePressed() {
		this.gameObject.SetActive(false);
		clickDetector.menusOpened--;
		var data = active.getData();
		data.drainAllLeft = drainAllLeft.GetComponent<Toggle>().isOn;
		data.drainAllRight = drainallRight.GetComponent<Toggle>().isOn;
		data.drainLeft = drainLeft.GetComponent<Toggle>().isOn;
		data.drainRight = drainRight.GetComponent<Toggle>().isOn;
	}

	private void selectionDone(List<ressources> selected) {
		print("ressource select done, got list: ");
		this.gameObject.SetActive(true);
		//active.drainingLeft.Add(HPHandler.ressources.Iridium);
        //print("active= " + active.GetHashCode() + active.getData());
	}

	public void setInstance(pipeHandler handler) {
		this.active = handler;
		this.titleLeft.GetComponent<Text>().text = active.getData().from.gameObject.name;
		this.titleRight.GetComponent<Text>().text = active.getData().to.gameObject.name;
        var data = active.getData();
		drainAllLeft.GetComponent<Toggle>().isOn = data.drainAllLeft;
		drainallRight.GetComponent<Toggle>().isOn = data.drainAllRight;
		drainLeft.GetComponent<Toggle>().isOn = data.drainLeft;
		drainRight.GetComponent<Toggle>().isOn = data.drainRight;
	}

	public void confLeftClicked() {
        print("active= " + active.GetHashCode() + active.getData());
		ResourceDisplay.openListSelect(active.getData().drainingLeft, selectionDone, active.getData().from.GetComponent<inventory>());
		this.gameObject.SetActive(false);
	}

	public void confRightClicked() {
        print("active= " + active.GetHashCode() + active.getData());
		ResourceDisplay.openListSelect(active.getData().drainingRight, selectionDone, active.getData().to.GetComponent<inventory>());
		this.gameObject.SetActive(false);
	}
}
