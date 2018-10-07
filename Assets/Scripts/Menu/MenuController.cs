using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    
    private Vector3 origScale;
    private float curScale = 1.0f;
    private float tarScale = 1.0f;
    private bool fade = false;
    private bool faded = false;
    private List<Transform> toMove = new List<Transform>();

	// Use this for initialization
	void Start () {
		origScale = this.transform.localScale;
	}

    void LateUpdate() {
        if (curScale != tarScale) {
            curScale = tarScale + (curScale - tarScale) * 0.6f;
        }

        if (Mathf.Abs(curScale - tarScale) < 0.01) {
            curScale = tarScale;
        }
        this.transform.localScale = origScale * curScale;

        if (fade) {

            foreach(Transform mover in toMove) {

                Color col;

                try {
                    col = mover.GetComponent<Image>().color;
                } catch(NullReferenceException ex) {
                    try {
                    col = mover.GetComponent<Text>().color;
                    } catch (NullReferenceException e) {
                        continue;
                    }
                }
                

                col.a *= 0.8f;
                try {
                    mover.GetComponent<Image>().color = col;
                } catch(NullReferenceException ex) {
                    mover.GetComponent<Text>().color = col;
                }

                if (col.a < 0.01f) {
                    fade = false;
                    faded = true;
                }
            }

            
        }

        if (faded) {
            Color color = this.GetComponentInChildren<Text>().color;
            color.a *= 0.8f;
            this.GetComponentInChildren<Text>().color = color;

            Color col = this.GetComponent<Image>().color;
            col.a *= 0.7f;
            this.GetComponent<Image>().color = col;

            if (col.a < 0.01f) {
                fade = false;
                faded = false;
                clickDone();
            }

        }
    }

    public void buttonDown() {
        Debug.Log("Pointer down on " + this.gameObject.name);
        tarScale = 0.95f;
    }

    public void buttonUp() {
        Debug.Log("Pointer up on " + this.gameObject.name);
        tarScale = 1.0f;
    }

    public void handleClick() {
        Debug.Log("Pointer clicked on " + this.gameObject.name);
        tarScale = 1.1f;
        fade = true;

        toMove.Clear();
        addChilds(toMove, GameObject.Find("Canvas").transform);
    }

    public void goToMainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private void addChilds(List<Transform> addTo, Transform parent) {
        foreach (Transform child in parent) {
            if (!child.name.Equals("Background") && !child.transform.Equals(this.transform)) {
                addTo.Add(child);
                addChilds(addTo, child);
            }
        }
    }

    private void clickDone() {
        Debug.Log("Animation Done");

        switch (this.gameObject.name) {
            case "Button_New":
                newClicked();
                break;
            case "Button_Continue":
                continueClicked();
                break;
            case "Button_Create":
                createClicked();
                break;
            default:
                break;
        }
    }

    private void createClicked() {
        Debug.Log("Creating new empty scene...");
        SceneManager.LoadScene("MainGame");
    }

    private void newClicked() {
        Debug.Log("New Scene creation now");
        SceneManager.LoadScene("Menu_NewGame", LoadSceneMode.Single);
    }

    private void continueClicked() {
        print("Continuing last save...");
        //this.GetComponent<SaveLoad>().Load();
        
        SceneManager.LoadScene("Game_Emptyload", LoadSceneMode.Single);
        print("scene loaded " + SceneManager.GetActiveScene().name);
    }

}
