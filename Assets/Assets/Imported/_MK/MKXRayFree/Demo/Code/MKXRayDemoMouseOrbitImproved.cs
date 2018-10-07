//////////////////////////////////////////////////////////////////////////////
//Credit to:    http://wiki.unity3d.com/index.php?title=MouseOrbitImproved
//license  :    https://creativecommons.org/licenses/by-sa/3.0/             
// Modifications 
// - LateUpdate -> if mousebutton 0 is pressed
// - LateUpdate -> added Touch support
// - added namespace
// - changed Class name
//////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

namespace MK.XRay
{
    public class MKXRayDemoMouseOrbitImproved : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        public float distanceMin = .5f;
        public float distanceMax = 15f;

        private Rigidbody rigidbody;

        float x = 0.0f;
        float y = 0.0f;

        private Vector2 lastPosTouch;
        private Vector2 currentPosTouch;
        private float lastX;
        private float lastY;

        // Use this for initialization
        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            rigidbody = GetComponent<Rigidbody>();

            // Make the rigid body not change rotation
            if (rigidbody != null)
            {
                rigidbody.freezeRotation = true;
            }
        }

        void LateUpdate()
        {
            if (Input.GetMouseButton(0) && !MKXRayDemoControl.SettingsUsed)
            {
                if (target)
                {
#if !UNITY_ANDROID || UNITY_EDITOR
                    x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
#else
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    lastPosTouch = Input.GetTouch(0).position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    currentPosTouch = Input.GetTouch(0).position;
                    x -= (lastPosTouch.x - currentPosTouch.x) * xSpeed * distance * Time.smoothDeltaTime * 0.02f;
                    y -= (currentPosTouch.y - lastPosTouch.y) * ySpeed * Time.smoothDeltaTime * 0.02f;
                    lastPosTouch = currentPosTouch;
                }
#endif
                    y = ClampAngle(y, yMinLimit, yMaxLimit);

                    Quaternion rotation = Quaternion.Euler(y, x, 0);

                    distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                    RaycastHit hit;
                    if (Physics.Linecast(target.position, transform.position, out hit))
                    {
                        distance -= hit.distance;
                    }
                    Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                    Vector3 position = rotation * negDistance + target.position;

                    transform.rotation = rotation;
                    transform.position = position;
                }
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}