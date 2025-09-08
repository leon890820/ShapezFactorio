using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.EventSystems;
using System;

public class SelectObject : MonoBehaviour
{
    public UnityEngine.Camera cam;
    public UImanager manager;
    public bool selectMode = false;
    public GenerateWorld world;
    public Material rim_material;
    public Material wire_material;
    public Color rim_color_acc;
    public Color rim_color_dec;

    [HideInInspector]
    public bool isInInformation;

    private GameObject selected_object;
    private PuttableObjectSetting puttableObjectSetting;
    private Ray mouse_ray;
    private Vector3 mouse_target;
    private GameObject put_object;
    private bool select_wire = false;
    private GameObject second_object;
    private Color greenColor = new Color(0.4f, 0.9f, 0.4f, 1.0f);
    private int rotation = 0;


    // Start is called before the first frame update
    void Start()
    {
        //world = GameObject.Find("world").GetComponent<GenerateWorld>();
        puttableObjectSetting = world.puttable_object_setting;
        mouse_ray = cam.ScreenPointToRay(Input.mousePosition);
        
    }

    // Update is called once per frame
    void Update()
    {

        SetMouseRay();

        if (selectMode) {
            put_object.transform.position = mouse_target;
            if (world.HasObject(mouse_target)) put_object.GetComponentInChildren<MeshRenderer>().material.SetColor("_RimColor",rim_color_dec);
            else put_object.GetComponentInChildren<MeshRenderer>().material.SetColor("_RimColor", rim_color_acc);
            put_object.GetComponentInChildren<PuttableObject>().CheckSurround();

            if (Input.GetKeyDown("r")) {
                rotation += 1;
                rotation %= 4;
                put_object.GetComponentInChildren<PuttableObject>().ChangeTypeAndRotation(0, rotation);
            }
        }




        SelectWorldObject();

        SelectWire();

        if (Input.GetMouseButtonDown(1) && selectMode) {
            selectMode = false;
            Destroy(put_object);
        }
        Debug.DrawRay(mouse_ray.origin, mouse_ray.direction * 10, Color.yellow);
    }

    private void SelectWorldObject() {
        if (Input.GetMouseButtonDown(0) && !selectMode && !IsOverTheUI()) {


            Reset(selected_object);
            if (Physics.Raycast(mouse_ray, out RaycastHit hit)) {
                selected_object = hit.collider.gameObject;
                selected_object.GetComponent<SelectableObject>().SetCanvasEnable(true);
                selected_object.GetComponent<SelectableObject>().OnClick();
                if (selected_object.CompareTag("Wire")) select_wire = true;
                
                SetObjectRim(selected_object, 1.0f);
            }
        } else if (Input.GetMouseButtonDown(0) && selectMode) {

            if (world.HasObject(mouse_target)) return;
            put_object.GetComponentInChildren<PuttableObject>().CreatObjectOnTheWorld(mouse_target, Quaternion.identity);
            GameObject[] surround_object = world.GetSurroundObject(new ChunkCoord(mouse_target));
            for (int i = 0; i < surround_object.Length; i += 1) {
                if (!surround_object[i]) continue;
                surround_object[i].GetComponentInChildren<PuttableObject>().CheckSurround();
            }
        }

    }

    private void SelectWire() {
        if (select_wire) {
            selected_object.GetComponent<Wire>().BeSelect();
            if (second_object != selected_object) Reset(second_object);
            if (Physics.Raycast(mouse_ray, out RaycastHit hit)) {
                second_object = hit.collider.gameObject;

                if (second_object != selected_object && second_object.CompareTag("Wire")) {
                    second_object.GetComponent<MeshRenderer>().materials[0].SetColor("_RimColor", greenColor);
                    SetObjectRim(second_object, 1.0f);
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                selected_object.GetComponent<Wire>().UnSelect();
                select_wire = false;
                Reset(selected_object);
                Reset(second_object);
                if (second_object != selected_object && second_object.CompareTag("Wire")) {
                    Wire wire1 = selected_object.GetComponent<Wire>();
                    Wire wire2 = second_object.GetComponent<Wire>();
                    
                    if (CheckWireHasConnect(selected_object, second_object)) {
                        wire1.UnLinkWire(wire2);
                        wire2.UnLinkWire(wire1);                       
                    } else {
                        WireLine wireline = new WireLine(selected_object.transform, second_object.transform, selected_object.transform.parent, wire_material);
                        selected_object.GetComponent<Wire>().LinkWire(second_object.GetComponent<Wire>(), wireline);
                        second_object.GetComponent<Wire>().LinkWire(selected_object.GetComponent<Wire>(), wireline);
                    }
                }
            }
        }

    }
    private bool CheckWireHasConnect(GameObject go1,GameObject go2) { 
        Wire wire1 = go1.GetComponent<Wire>();
        Wire wire2 = go2.GetComponent<Wire>();
        if (wire1 == null || wire2 == null) return false;
        if (wire1.linkWires.ContainsKey(wire2) || wire2.linkWires.ContainsKey(wire1)) return true;
        return false;
        
    }

    private void SetObjectRim(GameObject obj,float r) {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        manager.SetInfoImage(obj.GetComponent<SelectableObject>().GetInfoSprite());
        if (mr) mr.materials[0].SetFloat("_RimScale", r);
        MeshRenderer[] meshRenderer = obj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderer.Length; i += 1) {
            if (meshRenderer[i]) meshRenderer[i].materials[0].SetFloat("_RimScale", r);
        }

    }
    private void SetMouseRay() {
        mouse_ray = cam.ScreenPointToRay(Input.mousePosition);
        float t = -mouse_ray.origin.y / mouse_ray.direction.y;
        mouse_target = mouse_ray.origin + t * mouse_ray.direction;
        mouse_target.x = Mathf.FloorToInt(mouse_target.x);
        mouse_target.z = Mathf.FloorToInt(mouse_target.z);
    }

    private bool IsOverTheUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
    private void Reset(GameObject obj) {
        if (obj) {
            manager.SetTransparent();
            obj.GetComponent<SelectableObject>().SetCanvasEnable(false);
            SetObjectRim(obj, 0.0f);
        }
    }

    


    public void SelectSelectObjectToWire() {
        put_object = Instantiate(puttableObjectSetting.objectPack[0].object_prefab,mouse_target,Quaternion.identity);
        MeshRenderer[] mr = put_object.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mr.Length; i += 1) {
            mr[i].GetComponent<MeshRenderer>().material = rim_material;
        }       
        SetSelectModeTrue();
    }


    public void SelectSelectObjectToSwitch() {
        put_object = Instantiate(puttableObjectSetting.objectPack[1].object_prefab, mouse_target, Quaternion.identity);
        put_object.GetComponentInChildren<PuttableObject>().ChangeTypeAndRotation(0, rotation);
        MeshRenderer[] mr = put_object.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mr.Length; i += 1) {
            mr[i].GetComponent<MeshRenderer>().material = rim_material;
        }
        SetSelectModeTrue();

    }

    public void SelectSelectObjectToNandGate() {
        put_object = Instantiate(puttableObjectSetting.objectPack[2].object_prefab, mouse_target, Quaternion.identity);
        put_object.GetComponentInChildren<PuttableObject>().ChangeTypeAndRotation(0, rotation);
        MeshRenderer[] mr = put_object.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mr.Length; i += 1) {
            mr[i].GetComponent<MeshRenderer>().material = rim_material;
        }
        SetSelectModeTrue();

    }


    public void SetSelectModeFalse() {
        selectMode = false;
    }

    public void SetSelectModeTrue() {
        selectMode = true;
    }
}
