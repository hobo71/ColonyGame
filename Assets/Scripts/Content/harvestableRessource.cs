using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class harvestableRessource : MonoBehaviour, clickable {

    public Sprite infoBut;
    public Sprite moveToBut;
    
    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();
        
        controller.setTitle(this.gameObject.name, "Title");
        controller.setDesc(getDesc());
    }

    
    private string getDesc() {
        return "A naturally appearing ressource that can be harvested."
            +  Environment.NewLine
            + "Kind: " + this.GetComponent<HPHandler>().niceText();

    }

    public PopUpCanvas.popUpOption[] getOptions() {
         PopUpCanvas.popUpOption[] options;

         PopUpCanvas.popUpOption info = new  PopUpCanvas.popUpOption("Info", infoBut);
         PopUpCanvas.popUpOption goTo = new  PopUpCanvas.popUpOption("Harvest", moveToBut);
        
        options = new PopUpCanvas.popUpOption[]{info, goTo};
        return options;
    }

    public void handleClick() {
        Debug.Log("clicked ressource");
        moveHere();
    }

    public void handleLongClick() {
        Debug.Log("long clicked ressource");
        this.GetComponent<ClickOptions>().Create();
    }

    public void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            case "Harvest":
                moveHere();
                break;
            default:
                displayInfo();
                break;
        }
    }

    public void moveHere() {
        if (setDrill()) {
            return;
        }
        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        Transform nearestMover = GetClosestMover(movers, true, this.transform);

        if (nearestMover == null) {
            Debug.Log("no idle mover available");
            nearestMover = GetClosestMover(movers, false, this.transform);
        }

        nearestMover.GetComponent<movementController>().setTarget(this.transform);

    }

    private bool setDrill() {

        if (this.GetComponent<HPHandler>().type != HPHandler.ressources.Stone) {
            return false;
        }
        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("clickable")) {
            if (obj.GetComponent<DrillPlatformBasic>() != null) {
                if (!obj.GetComponent<DrillPlatformBasic>().isBusy()) {
                    obj.GetComponent<DrillPlatformBasic>().setTarget(this.gameObject);
                    return true;
                }
                
            }
        }
        return false;
    }

    //if notIdle is set to true it'll only return workers that are idle
    public static  Transform GetClosestMover (GameObject[] movers, bool onlyIdle, Transform self) {

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = self.position;

        foreach(GameObject potentialTarget in movers) {

            if (!potentialTarget.GetComponent<ActionController>().getState().Equals(ActionController.State.Idle) && onlyIdle) {
                continue;
            }

            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
     
        return bestTarget;
    }
}
