using UnityEngine;

public class TouchCamera : MonoBehaviour {

    private static readonly float PanSpeed = 20f;
    private static readonly float ZoomSpeedTouch = 0.01f;
    private static readonly float ZoomSpeedMouse = 4.0f;
    public static readonly float zoomAngleModifier = 3f;

    public static readonly float[] BoundsX = new float[] { -450f, 400f };
    public static readonly float[] BoundsZ = new float[] { -410f, 400f };
    public static readonly float[] ZoomBounds = new float[] { 7f, 60f };

    private Camera cam;

    private static float curHeight = 10f;
    private bool panActive;
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only
    private Vector3 camTargetOffset;

    public bool zoomActive;
    public GameObject ClickableInfo;
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        // If there's an open menu, or the clicker is being pressed, ignore the touch.
        /*if (GameManager.Instance.MenuManager.HasOpenMenu || GameManager.Instance.BitSpawnManager.IsSpawningBits) {
			return;
		}*/
        if (camTargetOffset.magnitude > 0.05) {
            //Debug.Log("Cam target pos: " + camTargetOffset);
            Vector3 moveBy = camTargetOffset * 0.7f;
            transform.Translate(moveBy, Space.World);
            camTargetOffset *= 0.7f;
        }

        if (Building.buildingMode || ClickableInfo.activeSelf || ResourceDisplay.menuOpened) {
            return;
        }

        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer) {
            HandleTouch();
        } else {
            HandleMouse();
        }

        ClampToBounds();
    }

    void HandleTouch() {
        switch (Input.touchCount) {

            case 1: // Panning
                zoomActive = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                    panActive = true;
                } else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved) {
                    PanCamera(touch.position);
                }
                break;

            case 2: // Zooming or rotating TODO
                panActive = false;

                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!zoomActive) {
                    lastZoomPositions = newPositions;
                    zoomActive = true;
                } else {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);

                    float angle = Vector3.SignedAngle(lastZoomPositions[1] - lastZoomPositions[0], newPositions[1] - newPositions[0], Vector3.Cross(lastZoomPositions[1] - lastZoomPositions[0], newPositions[1] - newPositions[0]));
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch, angle);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                panActive = false;
                zoomActive = false;
                break;
        }
    }

    void HandleMouse() {
        // On mouse down, capture it's position.
        // On mouse up, disable panning.
        // If there is no mouse being pressed, do nothing.
        if (Input.GetMouseButtonDown(0)) {
            panActive = true;
            lastPanPosition = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0)) {
            panActive = false;
        } else if (Input.GetMouseButton(0)) {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomActive = true;
        ZoomCamera(scroll, ZoomSpeedMouse, 0);
        zoomActive = false;
    }

    void PanCamera(Vector3 newPanPosition) {
        if (!panActive) {
            return;
        }

        // Translate the camera position based on the new input position
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed * 0.9f, 0, offset.y * PanSpeed * 1.1f);

        var ray = new Ray(this.transform.position, this.transform.forward);
        bool didHit = Physics.Raycast(ray, 5f);
        if (didHit) {
            float moveAm = -move.magnitude;
            Vector3 moveby = new Vector3(0, 0, moveAm);
            moveby *= transform.position.y * 0.2f;

            transform.Translate(moveby, Space.Self);
            curHeight = this.transform.position.y;
        }

        move = transform.rotation * move;
        move.y = 0;
        move *= transform.position.y * 0.2f;
        transform.Translate(move, Space.World);
        //ClampToBounds();
        camTargetOffset = move;
        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed, float angle) {
        if (!zoomActive || offset == 0 || (offset > 0 && transform.position.y < ZoomBounds[0]) || (offset < 0 && transform.position.y > ZoomBounds[1])) {
            return;
        }

        Vector3 moveby = new Vector3(0, 0, offset * speed);
        moveby *= transform.position.y * 0.2f;

        var ray = new Ray(this.transform.position, this.transform.forward);
        bool didHit = Physics.Raycast(ray, 5f);
        if (didHit && moveby.z > 0) return;

        transform.Translate(moveby, Space.Self);
        curHeight = this.transform.position.y;
        transform.Rotate(new Vector3(0f, 1f, 0f), angle, Space.World);
        //transform.Rotate(new Vector3(1,0,0), - offset * speed * zoomAngleModifier);
    }

    void ClampToBounds() {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);

        transform.position = pos;
    }

    public static float getHeight() {
        return curHeight;
    }

}