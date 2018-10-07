using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpCanvas : MonoBehaviour {

    public static Transform parentObj;
    
    public class popUpOption {
        private readonly string name;
        private readonly Sprite sourceImage;
        private bool enabled;

        public popUpOption(string Name, Sprite sourceImage) {
            name = Name;
            this.sourceImage = sourceImage;
            enabled = true;
        }

        public popUpOption(string Name, Sprite sourceImage, bool enabled) {
            name = Name;
            this.sourceImage = sourceImage;
            this.enabled = enabled;
        }

        public string getName() {
            return name;
        }

        public Sprite GetSprite() {
            return sourceImage;
        }

        public void setEnabled(bool enabled) {
            this.enabled = enabled;
        }

        public bool getEnabled() {
            return enabled;
        }

    }

    public void setParent(Transform parent) {
        Debug.Log("set gameobjecting for canvas");
        parentObj = parent;
    }

    public void handleClick(string action) {

        clickDetector.overlayClicked = true;
        ClickOptions.clear();

        Debug.Log("clicked button: " + action + " parent: " + parentObj);

        ((clickable) parentObj.gameObject.GetComponent(typeof(clickable))).handleOption(action);
    }
}
