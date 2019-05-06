using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretConfigurator : MonoBehaviour, clickable {


    public void handleClick() {
        //TODO
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
    }

    public void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            case "Activate":
                //TODO
                break;
            case "Deactivate":
                //TODO
                break;
        }
    }

    public void displayInfo() {
        //not reachable, need to rework interface
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption activate = new PopUpCanvas.popUpOption("Activate", UIPrefabCache.ActivateBut);
        PopUpCanvas.popUpOption deactivate = new PopUpCanvas.popUpOption("Deactivate", UIPrefabCache.DeactivateBut);

        var options = new[] {activate, deactivate};
        return options;
    }
}