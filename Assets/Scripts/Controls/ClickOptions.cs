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
    public GameObject assignEnergyBar;
    public GameObject assignInventoryBar;

    private GameObject curBarInv = null;
    private GameObject curBarEnergy = null;

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
