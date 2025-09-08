using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : PuttableObject {
    [HideInInspector]
    public Dictionary<Wire,WireLine> linkWires;
    private bool power;
    public bool pass;
    public bool bePower;


    public Color[] powerColor;
    // Start is called before the first frame update
    public override void CreatObjectOnTheWorld(Vector3 p, Quaternion q) {
        GameObject go = Instantiate(puttable_object, p, q);
        ChunkCoord c = new(p);
        GameObject pt = GameObject.Find("Wire");
        go.GetComponentInChildren<Wire>().ChangeTypeAndRotation(type, rotation);
        go.transform.parent = pt.transform;
        go.name = "Wire : [" + c.x + "," + c.y + "] (" + c.z + "," + c.w + ")";
        for (int y = 0; y < voxel_side.y; y += 1) {
            for (int x = 0; x < voxel_side.x; x += 1) {
                world.AddObjectToChunk(transform.position + new Vector3(x, 0, y), go);
            }
        }

    }

    void Start()
    {
        world = GameObject.Find("World").GetComponent<GenerateWorld>();
        puttable_object = world.puttable_object_setting.objectPack[0].object_prefab;
        voxel_side = new Vector2Int(1, 1);
        linkWires = new Dictionary<Wire, WireLine>();
    }



    // Update is called once per frame
    void Update() 
    {
        if (power) {
            bePower = power;
            PassPower();
        }
        pass = false;
        
       
    }

    private void LateUpdate() {
        Color color = bePower ? powerColor[1] : powerColor[0];
        
        SetObjectColor(gameObject, color);

        foreach (KeyValuePair<Wire, WireLine> item in linkWires) {
            WireLine wl = item.Value;
            SetObjectColor(wl.sphere1, color);
            SetObjectColor(wl.sphere2, color);
            SetObjectColor(wl.cylinder, color);
        }
        bePower = false;

    }


    public void PassPower() {
        if (pass) return;
        pass = true;
        foreach (KeyValuePair<Wire, WireLine> item in linkWires) {
            Wire w = item.Key;
            w.bePower = bePower;          
            w.PassPower();
        }
    }

    public void SetPower(bool b) {
        power = b;
        bePower = b;
    }

    public bool GetPower() {
        return bePower;
    }

    public void LinkWire(Wire w,WireLine wl) { 
        linkWires.Add(w,wl);
    }

    public void UnLinkWire(Wire w) {
        w.SetPower(false);
        w.PassPower();
        if (linkWires[w] == null) {
            linkWires.Remove(w);
            return;
        }
        Destroy(linkWires[w].gameLocation);
        linkWires.Remove(w);
    }

    public void ChangeWireColor(WireLine wl,float f ,Color color) { 
        wl.sphere1.GetComponent<MeshRenderer>().materials[0].SetColor("_RimColor", color);
        wl.sphere2.GetComponent<MeshRenderer>().materials[0].SetColor("_RimColor", color);
        wl.cylinder.GetComponent<MeshRenderer>().materials[0].SetColor("_RimColor", color);
        SetObjectRim(wl.sphere1,f);
        SetObjectRim(wl.sphere2, f);
        SetObjectRim(wl.cylinder, f);
    }

    public void BeSelect() {
        foreach (KeyValuePair<Wire, WireLine> item in linkWires) {
            ChangeWireColor(item.Value, 1.0f, new Color(0.4f, 1.0f, 0.4f));
        }
    }

    public void UnSelect() {
        foreach (KeyValuePair<Wire, WireLine> item in linkWires) {
            ChangeWireColor(item.Value, 0.0f, new Color(0.4f, 1.0f, 0.4f));
            
        }
    }

    private void SetObjectRim(GameObject obj, float r) {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();        
        if (mr) mr.materials[0].SetFloat("_RimScale", r);
        MeshRenderer[] meshRenderer = obj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderer.Length; i += 1) {
            if (meshRenderer[i]) meshRenderer[i].materials[0].SetFloat("_RimScale", r);
        }

    }

    private void SetObjectColor(GameObject obj, Color r) {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr) mr.materials[0].SetColor("_Color", r);
        MeshRenderer[] meshRenderer = obj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderer.Length; i += 1) {
            if (meshRenderer[i]) meshRenderer[i].materials[0].SetColor("_Color", r);
        }

    }

}
