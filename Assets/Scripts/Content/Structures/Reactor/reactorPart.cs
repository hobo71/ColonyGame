using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reactorPart : MonoBehaviour, clickable, reactorPart.IHeatableElement {

    public interface IHeatableElement {
        float getTemp();
        void setData(ReactorLogic.HeatableStructure data);
    }

    private ReactorLogic.HeatableStructure data = null;

    public void displayInfo() {
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options = new PopUpCanvas.popUpOption[] { };
        return options;
    }

    public void handleClick() {

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
    }

    public void handleOption(string option) {
        return;
    }

    public float getTemp() {
        return (int) data.temperature;
    }

    public void setData(ReactorLogic.HeatableStructure data) {
      this.data = data;
    }
}
