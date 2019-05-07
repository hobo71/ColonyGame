using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretConfigurator : MonoBehaviour, clickable {
    public interface ConfigurableTurret {
        void setActive(bool val);
        bool getActive();
    }

    public void handleClick() {
        displayState(getActive());
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
    }

    public void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            case "Activate":
                this.GetComponent<ConfigurableTurret>().setActive(true);
                displayState(true);
                break;
            case "Deactivate":
                this.GetComponent<ConfigurableTurret>().setActive(false);
                displayState(false);
                break;
        }
    }
    private void displayState(bool state) {
        if (state) {
            Notification.createNotification(this.gameObject, Notification.sprites.Working, "Working....", Color.cyan,
                true);
        }
        else {
            Notification.createNotification(this.gameObject, Notification.sprites.Stopping, "Idle", Color.yellow,
                false);
        }
    }

    private bool getActive() {
        return this.GetComponent<ConfigurableTurret>().getActive();
    }

    public void displayInfo() {
        //not reachable, need to rework interface
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption activate = new PopUpCanvas.popUpOption("Activate", UIPrefabCache.ActivateBut);
        PopUpCanvas.popUpOption deactivate = new PopUpCanvas.popUpOption("Deactivate", UIPrefabCache.DeactivateBut);

        activate.setEnabled(!getActive());
        deactivate.setEnabled(getActive());

        var options = new[] {activate, deactivate};
        return options;
    }
}