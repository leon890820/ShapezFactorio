using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour{
    public static CameraControl Instance { get; private set; }
    public Transform lookAtPoint;
    public float mouseSensitivity = 0.5f;
    public float keySensitivity = 0.1f;
    public float cameraMoveSpeed = 2.0f;
    public float baseDistance = 5.0f;

    private float mouseHorizontal;
    private float mouseVertical;  
    private float mouseRotateSensitivity = 1.0f;

    private float scroll_number = 5;   
    private float distance;

    public bool GalaxyMode {
        get {
            return scroll_number >= 15;
        }
    }
    public bool galaxy { get; set; }



    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start(){
        distance = baseDistance;
    }

    // Update is called once per frame
    private void Update() {        
        CameraTranslate();
        LookAtPointTranslate();
        CameraRotate();
        CameraScale();
    }

    private void CameraTranslate() {
        float speed = distance / baseDistance * 0.5f;
        mouseHorizontal = -Input.GetAxis("Horizontal") * keySensitivity * speed;
        mouseVertical = -Input.GetAxis("Vertical") * keySensitivity * speed;
        if (Input.GetMouseButton(1)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }
            mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * speed;
            mouseVertical = Input.GetAxis("Mouse Y") * mouseSensitivity * speed;
        }
    }

    private void LookAtPointTranslate() {
        Quaternion rotation = Quaternion.Euler(0.0f, lookAtPoint.transform.rotation.eulerAngles.y, 0.0f);
        var forwardVector = rotation * Vector3.forward;
        var rightVector = rotation * Vector3.right;
        lookAtPoint.Translate(-forwardVector * mouseVertical - rightVector * mouseHorizontal, Space.World);
    }

    private void CameraRotate() {
        if (Input.GetMouseButton(2)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }
            RestrictLookAtPointRotation();
            RotateLookAtPoint();
        }

    }

    private void RestrictLookAtPointRotation() {
        if (lookAtPoint.eulerAngles.x > 80.0f) {
            lookAtPoint.rotation = Quaternion.Euler(80.0f, lookAtPoint.eulerAngles.y, lookAtPoint.eulerAngles.z);
        }
        if (lookAtPoint.eulerAngles.x < 10.0f) {
            lookAtPoint.rotation = Quaternion.Euler(10.0f, lookAtPoint.eulerAngles.y, lookAtPoint.eulerAngles.z);
        }
    }

    private void RotateLookAtPoint() {
        mouseHorizontal = Input.GetAxis("Mouse X") * mouseRotateSensitivity;
        mouseVertical = Input.GetAxis("Mouse Y") * mouseRotateSensitivity;
        lookAtPoint.Rotate(new Vector3(-mouseVertical, 0, 0), Space.Self);
        lookAtPoint.Rotate(new Vector3(0, mouseHorizontal, 0), Space.World);
    }

    private void CameraScale() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) {
            ScrollUp();
        } else if (scroll < 0) {
            ScrollDown();
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -distance), Time.deltaTime * cameraMoveSpeed);
    }

    private void ScrollUp() {
        if (scroll_number < 0) return;
        scroll_number--;
        if (scroll_number == 14) galaxy = true;
        distance *= 0.8f;
    }

    private void ScrollDown() {
        if (scroll_number >= 20) return;
        scroll_number++;
        if (scroll_number == 15) galaxy = true;
        distance /= 0.8f;
    }
}
