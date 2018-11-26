using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipeHandler : MonoBehaviour, clickable {

    public Sprite infoBut;
    public Sprite stopBut;
    public Sprite startBut;
    private ConveyorHandler.conveyorConnection data;

	private bool active = false;

    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();

        controller.setTitle(this.gameObject.name);
        controller.setDesc("Used to move items between structures");
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption conf = new PopUpCanvas.popUpOption("conf", infoBut);
        PopUpCanvas.popUpOption start = new PopUpCanvas.popUpOption("doStop", stopBut);
        PopUpCanvas.popUpOption stop = new PopUpCanvas.popUpOption("doStart", startBut);

        options = new PopUpCanvas.popUpOption[] { conf, start, stop };
        return options;
    }

    public void handleClick() {
		if (this.active) {
			doStop();
		} else {
			doStart();
		}
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
    }

    public void handleOption(string option) {

        Debug.Log("handling option: " + option);

        switch (option) {
            case "conf":
				doConf();
                break;
            case "doStop":
				doStop();
                break;
            case "doStart":
				doStart();
                break;
            default:
                displayInfo();
                break;
        }
    }

	private void doStart() {
		active = true;
	}

	private void doStop() {
		active = false;
	}

	private void doConf() {
        Scene_Controller.getInstance().conveyorConfigurator.SetActive(true);
        Scene_Controller.getInstance().conveyorConfigurator.GetComponent<ConveyorConfigurator>().setInstance(this);
        clickDetector.menusOpened++;
		//TODO
	}

    // Update is called once per frame
    void FixedUpdate() {

    }

    public void setData(ConveyorHandler.conveyorConnection data)  {
        this.data = data;
        print("connection got data: " + data.from + " -> " + data.to);
    }

    public ConveyorHandler.conveyorConnection getData() {
        return data;
    }
}
