using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickOptions : MonoBehaviour {
    
    public GameObject Canvas;
    public GameObject template;
    public static GameObject inventoryBar;
    public static GameObject energyBar;
    public static GameObject HPBar;
    public GameObject assignEnergyBar;
    public GameObject assignInventoryBar;
    public GameObject assignHPBar;

    private GameObject curBarInv = null;
    private GameObject curBarEnergy = null;
    private GameObject curBarHP = null;
    private GameObject curTempDisplay = null;
    public GameObject tempDisplay = null;

    public static List<GameObject> PopUps = new List<GameObject>();
    public static bool UIOpen = false;

    private static float creationTime = 0;

    void FixedUpdate() {
        if (curBarInv != null) {
            var inv = this.GetComponent<inventory>();
            curBarInv.transform.GetChild(0).GetComponent<Image>().fillAmount = inv.getFillPercent();
        }

        if (curBarEnergy != null) {
            var energy = this.GetComponent<EnergyContainer>();
            curBarEnergy.transform.GetChild(0).GetComponent<Image>().fillAmount = (float) energy.getCurEnergy() / energy.getMaxEnergy();

        }
        
        if (curBarHP != null) {
            var hp = this.GetComponent<HPHandler>();
            curBarHP.transform.GetChild(0).GetComponent<Image>().fillAmount = (float) hp.HP / hp.getInitialHP();

        }

        if (curTempDisplay != null) {
            var textA = curTempDisplay.GetComponent<Text>();
            var textB = curTempDisplay.transform.GetChild(0).GetComponent<Text>();
            var temp = this.gameObject.GetComponent<reactorPart.IHeatableElement>();
            textA.text = temp.getTemp().ToString() + "°";
            textB.text = temp.getTemp().ToString() + "°";
            var col = new Color(temp.getTemp() / 1000, 1f, 0.02f);
            if (temp.getTemp() > 1000) {
                col = new Color(1f, 1f - ((temp.getTemp() - 1000) / 1000f), 0.02f);
            }
            //textA.color = col;
            textB.color = col;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake() {
        if (assignInventoryBar != null) {
            inventoryBar = assignInventoryBar;
        }
        
        if (assignEnergyBar != null) {
            energyBar = assignEnergyBar;
        }

        if (assignHPBar != null) {
            HPBar = assignHPBar;
        }
    }
    
    public void Create() {
        Debug.Log("Creating Click options for: " + this.name);
        UIOpen = true;

        creationTime = Time.fixedTime;

        GameObject cam = Camera.main.gameObject;

        Vector3 createAt = this.transform.position;
        Vector3 above = createAt;
        above.y += 30;

        RaycastHit hit;
        Physics.Raycast(above, createAt - above, out hit, 30.0f);
        Debug.Log("Top of element is at: " + hit.point);

        Vector3 camUp = cam.transform.up;
        camUp.Normalize();
        camUp *= 2.0f;

        createAt = hit.point + camUp;

        GameObject parent = GameObject.Instantiate(Canvas, createAt, cam.transform.rotation);
        PopUps.Add(parent);
        parent.GetComponent<PopUpCanvas>().setParent(this.transform);
        
        PopUpCanvas.popUpOption[] options = ((clickable) this.GetComponent(typeof(clickable))).getOptions();

        int count = 0;
        float elements = options.Length - 1;
        foreach (PopUpCanvas.popUpOption button in options) {
            GameObject obj = GameObject.Instantiate(template, parent.transform);
            obj.GetComponent<UnityEngine.UI.Image>().sprite = button.GetSprite();

            Vector3 pos = new Vector3(3.0f * (count - elements / 2), 0, 0);
            obj.transform.localPosition = pos;
            obj.name = button.getName();

            if (!button.getEnabled()) {
                obj.GetComponent<UnityEngine.UI.Image>().color = Color.gray;
                obj.GetComponent<EventTrigger>().triggers.RemoveAt(0);
            }

            count++;
        }
        
        var hp = this.GetComponent<HPHandler>();
        if (hp != null && !(tempDisplay != null && this.GetComponent<reactorPart.IHeatableElement>() != null && hp.HP / hp.getInitialHP() > 0.99)) {
            var obj =  GameObject.Instantiate(HPBar, parent.transform);
            obj.transform.GetChild(0).GetComponent<Image>().fillAmount = (float) hp.HP / hp.getInitialHP();
            curBarHP = obj;
        }

        var energy = this.GetComponent<EnergyContainer>();
        if (energy != null) {
            var obj =  GameObject.Instantiate(energyBar, parent.transform);
            obj.transform.GetChild(0).GetComponent<Image>().fillAmount = (float) energy.getCurEnergy() / energy.getMaxEnergy();
            curBarEnergy = obj;
        }
        
        var inv = this.GetComponent<inventory>();
        if (inv != null) {
            var obj =  GameObject.Instantiate(inventoryBar, parent.transform);
            obj.transform.GetChild(0).GetComponent<Image>().fillAmount = inv.getFillPercent();
            if (energy == null) {
                obj.transform.localPosition = energyBar.transform.localPosition;
            }
            curBarInv = obj;
        }

        if (tempDisplay != null && this.GetComponent<reactorPart.IHeatableElement>() != null) {
            //reactor part found
            var obj =  GameObject.Instantiate(tempDisplay, parent.transform);
            var pos = obj.transform.localPosition;
            pos.y -= 1f;
            if (options.Length > 0) {
                pos.x -= 7.5f;
                pos.y -= 0.5f;
            }
            obj.transform.localPosition = pos;
            this.curTempDisplay = obj;
        }

    }

    public static void clear() {

        if (Time.fixedTime - creationTime < 0.3f) {
            return;
        }

        foreach (GameObject obj in PopUps) {
            obj.GetComponent<PopUpDestroyer>().destroy();
        }
        PopUps.Clear();
    }
}
