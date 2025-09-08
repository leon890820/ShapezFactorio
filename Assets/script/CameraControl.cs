using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{

    public Transform lookAtPoint;


    public float mouseSensitivity = 0.5f;
    public float keySensitivity = 0.1f;

    public float cameraMoveSpeed = 2.0f;

    private float mouseHorizontal;
    private float mouseVertical;  
    private float mouseRotateSensitivity = 1.0f;


    [HideInInspector]
    public static float scroll_number = 5;
    public static bool galaxy = false;


    private Vector3 forwardVector;
    private  Vector3 rightVector;

    public float baseDistance = 5.0f;
    private float distance;

    public static bool galaxyMode {
        get {
            return scroll_number >= 15;
        }

    }


    // Start is called before the first frame update
    void Start(){
        distance = baseDistance;

    }

    // Update is called once per frame
    void Update() {


        

        float speed = distance / baseDistance * 0.5f;

        Quaternion rotation = Quaternion.Euler(0.0f, lookAtPoint.transform.rotation.eulerAngles.y, 0.0f);
        Matrix4x4 m = Matrix4x4.Rotate(rotation);
        mouseHorizontal = -Input.GetAxis("Horizontal") * keySensitivity * speed;
        mouseVertical = -Input.GetAxis("Vertical") * keySensitivity * speed;


        if (Input.GetMouseButton(1)) {

            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * speed;
            mouseVertical = Input.GetAxis("Mouse Y") * mouseSensitivity * speed;
       
        }

        forwardVector = m.MultiplyPoint3x4(Vector3.forward);
        rightVector = m.MultiplyPoint3x4(Vector3.right);
        lookAtPoint.Translate(-forwardVector * mouseVertical - rightVector * mouseHorizontal, Space.World);

        if (Input.GetMouseButton(2)) {

            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            mouseHorizontal = Input.GetAxis("Mouse X") * mouseRotateSensitivity;
            mouseVertical = Input.GetAxis("Mouse Y") * mouseRotateSensitivity;
            if (lookAtPoint.eulerAngles.x > 80.0f ) {
                
                lookAtPoint.rotation = Quaternion.Euler(80.0f, lookAtPoint.eulerAngles.y, lookAtPoint.eulerAngles.z);
            }
            if (lookAtPoint.eulerAngles.x < 10.0f) {
                lookAtPoint.rotation = Quaternion.Euler(10.0f, lookAtPoint.eulerAngles.y, lookAtPoint.eulerAngles.z);
            }

            lookAtPoint.Rotate(new Vector3(-mouseVertical, 0, 0), Space.Self);
            lookAtPoint.Rotate(new Vector3(0, mouseHorizontal, 0), Space.World);

        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) {
            if (scroll_number < 0) return;
            scroll_number--;
            if (scroll_number == 14) galaxy = true;
            distance *= 0.8f;
            
            

        } else if (scroll < 0) {
            if (scroll_number >= 20) return;
            scroll_number++;
            if (scroll_number == 15) galaxy = true;
            distance /= 0.8f;
           
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition,new Vector3(0, 0, -distance), Time.deltaTime * cameraMoveSpeed);


    }



}
