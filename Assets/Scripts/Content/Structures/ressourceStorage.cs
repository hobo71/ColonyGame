using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ressourceStorage : MonoBehaviour, clickable {

    public Sprite infoBut;
    public Sprite moveToBut;

    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();

        controller.setTitle(this.gameObject.name);
        controller.setDesc(getDesc());
    }

    private string getDesc() {
        return "A Structure that can be used as ressource gathering point. Clicking it will force all nearby units to deliver everything they're carrying"
            +  Environment.NewLine
            + this.GetComponent<inventory>().ToString();

    }

    public PopUpCanvas.popUpOption[] getOptions() {
         PopUpCanvas.popUpOption[] options;

         PopUpCanvas.popUpOption info = new  PopUpCanvas.popUpOption("Info", infoBut);
         PopUpCanvas.popUpOption goTo = new  PopUpCanvas.popUpOption("Deliver", moveToBut);
        
        options = new PopUpCanvas.popUpOption[]{info, goTo};
        return options;
    }

    public void handleClick() {
        Debug.Log("clicked ressource");
        deliverIdles();
    }

    public void handleLongClick() {
        Debug.Log("long clicked ressource");
        this.GetComponent<ClickOptions>().Create();
    }

    public void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            case "Info":
                Debug.Log("handling info click for ressource Storage");
                displayInfo();
                break;
            default:
                deliverIdles();
                break;
        }
    }

    private void deliverIdles() {
        Debug.Log("setting units to deliver now");

        GameObject[] carriers = GameObject.FindGameObjectsWithTag("mover");

        foreach (GameObject carrier in carriers) {
            if (carrier.GetComponent<inventory>().getAmount() > 0) {
                carrier.GetComponent<movementController>().setTarget(this.transform);
                carrier.GetComponent<ActionController>().setState(ActionController.State.Walking);
            }
        }

    }
}
