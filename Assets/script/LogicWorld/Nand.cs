using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nand : PuttableObject
{
    public Transform pivot;

    public Wire[] inputs;
    public Wire[] outputs;
    // Start is called before the first frame update
    void Start(){
        world = GameObject.Find("World").GetComponent<GenerateWorld>();
        puttable_object = world.puttable_object_setting.objectPack[2].object_prefab;
        voxel_side = new Vector2Int(2, 1);
    }

    // Update is called once per frame
    void Update(){
        if (inputs[0].GetPower() && inputs[1].GetPower()) {
            outputs[0].SetPower(false);
        } else { 
            outputs[0].SetPower(true);
        }
    }

    public override void CreatObjectOnTheWorld(Vector3 p, Quaternion q) {
        GameObject go = Instantiate(puttable_object, p, q);
        ChunkCoord c = new(p);
        GameObject pt = GameObject.Find("Gate");
        go.GetComponentInChildren<Nand>().ChangeTypeAndRotation(type, rotation);
        go.transform.parent = pt.transform;
        float theda = -rotation * 0.5f * Mathf.PI;
        float co = Mathf.Cos(theda); float s = Mathf.Sin(theda);
        go.name = "Nand : [" + c.x + "," + c.y + "] (" + c.z + "," + c.w + ")";
        for (int y = 0; y < voxel_side.y; y += 1) {
            for (int x = 0; x < voxel_side.x; x += 1) {
                float rx = Mathf.Round(co * x - s * y);
                float ry = Mathf.Round(s * x + co * y);
                world.AddObjectToChunk(p + new Vector3(rx, 0, ry), go);
            }
        }

    }

    public override void ChangeTypeAndRotation(int t, int r) {
        type = t; rotation = r;
        pivot.rotation = Quaternion.Euler(0, rotation * 90.0f, 0);
    }
}
