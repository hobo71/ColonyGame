using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour {
    private static List<Sprite> availSprites;
    public List<Sprite> assignSprites;
    public GameObject assignCanvas;
    private static GameObject canvas;
    public GameObject assignText;
    private static GameObject text;
    public GameObject assignImage;
    private static GameObject image;

    private static GameObject visibleCanvas = null;
    private static GameObject visibleText = null;
    private static GameObject visibleImage = null;
    private static bool spinning = false;
    private static bool spinMode2 = false;

    public enum sprites {
        Energy_Low, Working, Starting, Stopping, Targeting
    }


    private static Sprite GetSprite(sprites sprite) {
        switch (sprite) {
            case sprites.Energy_Low:
                return availSprites[0];
            case sprites.Working:
                return availSprites[1];
            case sprites.Starting:
                return availSprites[2];
            case sprites.Stopping:
                return availSprites[3];
            case sprites.Targeting:
                return availSprites[4];
            default:
                return null;
        }
    }

	// Use this for initialization
	void Start () {
		availSprites = assignSprites;
        canvas = assignCanvas;
        text = assignText;
        image = assignImage;
	}
	
    private static float timePassed = 0;
	// Update is called once per frame
	void Update () {

        if (visibleCanvas != null) {

            if (spinning) {
                if (spinMode2) {
                    visibleImage.transform.Rotate(new Vector3(0, 1, 0) , 4);
                } else {
                    visibleImage.transform.Rotate(new Vector3(0, 0, 1) , 4);
                }
                
            }

		    timePassed += Time.deltaTime;
            if (timePassed > 1) {
                float a = (1.5f - timePassed) * 2f;
                setAlpha(a);

                if (a < 0.05) {
                    print("fading done");
                    GameObject.Destroy(visibleText);
                    GameObject.Destroy(visibleImage);
                    GameObject.Destroy(visibleCanvas);
                    visibleCanvas = null;
                    visibleImage = null;
                    visibleText = null;
                    timePassed = 0;
                }

            } else if (timePassed < 0.1f){
                float a = (timePassed) * 10f;
                setAlpha(a);
            }
        }
	}

    private static void reset() {
        GameObject.Destroy(visibleText);
        GameObject.Destroy(visibleImage);
        GameObject.Destroy(visibleCanvas);
        visibleCanvas = null;
        visibleImage = null;
        visibleText = null;
        timePassed = 0;
    }

    private static void setAlpha(float a) {
        Color image = visibleImage.GetComponent<UnityEngine.UI.Image>().color;
        image.a = a;
        visibleImage.GetComponent<UnityEngine.UI.Image>().color = image;
        Color textCol = visibleText.GetComponent<UnityEngine.UI.Text>().color;
        textCol.a = a;
        visibleText.GetComponent<UnityEngine.UI.Text>().color = textCol;
    }

    public static void createNotification(GameObject on, sprites name, string Text, Color color, bool spinning) {

        if (visibleCanvas != null) {
            reset();
        }

        Notification.spinning = spinning;
        Debug.Log("Creating Click options for: " + on.name);

        GameObject cam = Camera.main.gameObject;

        Vector3 createAt = on.transform.position;
        Vector3 above = createAt;
        above.y += 30;

        RaycastHit hit;
        Physics.Raycast(above, createAt - above, out hit, 30.0f);
        Debug.Log("Top of element is at: " + hit.point);

        Vector3 camUp = cam.transform.up;
        camUp.Normalize();
        camUp *= 2.0f;

        createAt = hit.point + camUp;

        GameObject parent = GameObject.Instantiate(canvas, createAt, cam.transform.rotation);
        GameObject iconCreated = GameObject.Instantiate(image, parent.transform);
        GameObject textCreated = GameObject.Instantiate(text, parent.transform);

        iconCreated.GetComponent<UnityEngine.UI.Image>().sprite = GetSprite(name);
        iconCreated.GetComponent<UnityEngine.UI.Image>().color = color;
        textCreated.GetComponent<UnityEngine.UI.Text>().text = Text;
        textCreated.GetComponent<UnityEngine.UI.Text>().color = new Color(0.8f, 0.8f, 0.8f);

        if (Text.Equals("")) {
            iconCreated.transform.localPosition = Vector3.zero;
        }

        if (name.Equals(sprites.Starting) && spinning) {
            spinMode2 = true;
        } else {
            spinMode2 = false;
        }

        visibleCanvas = parent;
        visibleImage = iconCreated;
        visibleText = textCreated;
        setAlpha(0.0f);
    }

    public static void createNotification(GameObject on, sprites name, string Text) {
        createNotification(on, name, Text, Color.white, false);
    }

    public static void createNotification(GameObject on, sprites name, string Text, Color color) {
        createNotification(on, name, Text, color, false);
    }
}
