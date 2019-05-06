using System;
using System.Collections.Generic;
using UnityEngine;

public class clickDetector : MonoBehaviour {

    private float onStart;
    public static bool overlayClicked;
    public GameObject ClickableInfo;

    //new int, used to display the number of active overlays/menus, click detection is only happening when this equals 0.
    //increment when opening a menu, decrement when closing
    public static int menusOpened = 0;

    private static List<Outline> outlines = new List<Outline>();

    private Action<GameObject> nextAction;

    // Update is called once per frame
    void Update() {

        if (Building.buildingMode || ClickableInfo.activeSelf || ResourceDisplay.menuOpened || menusOpened > 0) {
            return;
        }

        if (((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began)) || Input.GetMouseButtonDown(0)) {
            onStart = Time.time;
        }

        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended)) {


            if (Input.GetTouch(0).deltaPosition.magnitude > 1.0f || overlayClicked) {
                overlayClicked = false;
                return;
            }

            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            clearPopUps();
            handleRay(raycast);
        }

        if ((Input.GetMouseButtonUp(0))) {
            if (overlayClicked) {
                overlayClicked = false;
                return;
            }
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            clearPopUps();
            handleRay(raycast);
        }

        /*if (highlight != null) {

            float scalar = 1.0f;

            if (highlightTime <= 0.1f) {
                scalar = highlightTime * 5.0f;
            } else {
                float thisTime = highlightTime - 0.1f;
                scalar = 1 - (thisTime) * (1 / 0.7f);
            }
            outlineEffect.lineColor0.a = scalar;
            outlineEffect.fillAmount = 0.3f * scalar;

            highlightTime += Time.unscaledDeltaTime;

            if (highlightTime > hightlightMaxTime) {
                GameObject.Destroy(highlight.GetComponent<cakeslice.Outline>());
                highlight = null;
                outlineEffect = null;
                GameObject.Find("Main Camera").GetComponent<cakeslice.OutlineEffect>().enabled = false;
            }
        }*/
        if (outlines.Count > 0) {
            List<Outline> toRemove = new List<Outline>();
            foreach (Outline outline in outlines) {
                try {
                outline.OutlineWidth -= 0.1f;
                if (outline.OutlineWidth < 0.1f) {
                    Destroy(outline);
                    //outlines.Remove(outline);
                    toRemove.Add(outline);
                }
                } catch (NullReferenceException ex) {
                    toRemove.Add(outline);
                }
            }
            foreach(Outline elem in toRemove) outlines.Remove(elem);
            toRemove.Clear();
        }

    }

    private void handleRay(Ray raycast) {

        RaycastHit raycastHit;
        int layerMask = LayerMask.GetMask("Clickable");
        //Debug.Log("UI Open: " + canvasController.UIOpen);

        if (Physics.Raycast(raycast, out raycastHit, 200.0f, layerMask)) {
            Debug.Log("hit layer: " + raycastHit.collider.gameObject.layer + " checking for: " + LayerMask.NameToLayer("Clickable"));
            //if (raycastHit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Clickable"))) {
            float pressTime = Time.time - onStart;
            Debug.Log("clickable tag clicked, time pressed: " + pressTime);
            clickable target = (clickable)raycastHit.transform.gameObject.GetComponent(typeof(clickable));
            
            createHighlight(raycastHit.transform.gameObject);

            if (nextAction != null) {
                var call = nextAction;
                nextAction = null;
                call.Invoke(raycastHit.transform.gameObject);
                return;
            }

            if (pressTime > 0.2f) {
                target.handleLongClick();
            } else {
                target.handleClick();
            }
        }

    }

    private void createHighlight(GameObject on) {
        print("creating highlight for: " + on.name);
        
        //destroy existing effect
        if (on.GetComponent<Outline>() != null) {
            GameObject.Destroy(on.GetComponent<Outline>());
        }
        Outline effect = on.AddComponent<Outline>();
        effect.OutlineColor = Color.cyan;
        effect.OutlineWidth = 4.0f;
        outlines.Add(effect);
    }

    public static void clearPopUps() {
        ClickOptions.clear();
    }

    public void setNextClickAction(Action<GameObject> onClick) {
        print("next click has been overridden");
        nextAction = onClick;
    }

    public void resetNextClick() {
        nextAction = null;
    }
}
