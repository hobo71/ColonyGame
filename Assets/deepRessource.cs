using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deepRessource : harvestableRessource {

    public Sprite drillBut;
    public GameObject markerPrefab;
    public GameObject drillPrefab;
    public GameObject activeDrill = null;
    override public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption goTo = new PopUpCanvas.popUpOption("Harvest", moveToBut);
        PopUpCanvas.popUpOption makeDrill = new PopUpCanvas.popUpOption("CreateDrill", drillBut);

        for (int i = 0; i < DeepDrillPlatform.getPrice().Length; i++) {
            //print("i: " + i + " kind=" + DeepDrillPlatform.getPrice()[i].getRessource() + " availamount=" + ResourceHandler.getAmoumt(DeepDrillPlatform.getPrice()[i].getRessource()) + " req=" + DeepDrillPlatform.getPrice()[i].getAmount());
            if (ResourceHandler.getAmoumt(DeepDrillPlatform.getPrice()[i].getRessource()) < DeepDrillPlatform.getPrice()[i].getAmount()) {
                //ressources missing
                makeDrill.setEnabled(false);
            }

        }

        if (activeDrill != null) {
            makeDrill.setEnabled(false);
        }

        options = new PopUpCanvas.popUpOption[] { info, goTo, makeDrill };
        return options;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            case "Harvest":
                moveHere();
                break;
            case "CreateDrill":
                createDrill();
                break;
            default:
                displayInfo();
                break;
        }
    }

    private void createDrill() {
        print("creating drill on mineral patches!");
        var elem = markerPrefab;
        var beacon = GameObject.Instantiate(elem, this.transform.position, Quaternion.identity);

        beacon.GetComponent<MeshCollider>().sharedMesh = this.GetComponent<MeshFilter>().sharedMesh;
        beacon.GetComponent<MeshFilter>().sharedMesh = this.GetComponent<MeshFilter>().sharedMesh;
        //beacon.transform.localScale = new Vector3(3f, 3f, 3f);

        beacon.GetComponent<building_marker>().buildTo = drillPrefab;
        //beacon.transform.Rotate(new Vector3(-90f, 0 , 0), Space.Self);
        var pos = beacon.transform.position;
        pos.y += 1f;
        beacon.transform.position = pos;
    }

}
