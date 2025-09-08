using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObject : Building{
    public int[,] dir = { { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };
    private MeshFilter meshFilter;
    
    private void Start() {
        world = GameObject.Find("world").GetComponent<GenerateWorld>();
        //puttable_object = world.puttable_object_setting.road.object_prefab;
        meshFilter = GetComponent<MeshFilter>();
        resources.Add(new Stone(10));
    }

    public override void ChangeTypeAndRotation(int t,int r) {
        type = t; rotation = r;
        if(!world) world = GameObject.Find("world").GetComponent<GenerateWorld>();
        if(!meshFilter) meshFilter = GetComponent<MeshFilter>();
        //meshFilter.mesh = world.puttable_object_setting.road.mesh[type];
        transform.rotation = Quaternion.Euler(0, rotation * 90.0f, 0);
       
       
    }

    public override void CheckSurround() {
        if (!world) world = GameObject.Find("world").GetComponent<GenerateWorld>();
        GameObject[] surround_object = world.GetSurroundObject(new ChunkCoord(transform.position));
        int t = 0;
        int r = 0;
        bool[] roads_bool = { false, false, false, false };
        int count = 0;
        for (int i = 0; i < surround_object.Length; i += 1) {
            if (surround_object[i] && surround_object[i].CompareTag("Road")) {
                roads_bool[i] = true;
                count += 1;
            }
        }
       
        switch (count) {
            case 0:
            case 1:
                t = 0;
                if (roads_bool[0] || roads_bool[2]) r = 0;
                else r = 1;
                break;
            case 2:
                if (roads_bool[0] && roads_bool[0] == roads_bool[2]) {
                    t = 0;
                    r = 0;
                } else if (roads_bool[1] && roads_bool[1] == roads_bool[3]) {
                    t = 0;
                    r = 1;
                } else {
                    for (int i = 0; i < roads_bool.Length; i += 1) {
                        if (!roads_bool[i]) {
                            t = 1; r = (4 - i + 2) % 4;
                            break;
                        }
                    }
                }
                break;
            case 3:
                for (int i = 0; i < roads_bool.Length; i += 1) {
                    if (!roads_bool[i]) {
                        t = 1; r = (4 - i + 2) % 4;
                        break;
                    }
                }
                break;
            case 4:
                t = 2;
                break;
        }
        
        ChangeTypeAndRotation(t, r);

    }

    public override void CreatObjectOnTheWorld(Vector3 p, Quaternion q) {
        GameObject go = Instantiate(puttable_object, p, q);
        ChunkCoord c = new(p);
        GameObject pt = GameObject.Find("Road");
        go.GetComponentInChildren<RoadObject>().ChangeTypeAndRotation(type,rotation);      
        go.transform.parent = pt.transform;       
        go.name = "Road : [" + c.x + "," + c.y + "] (" + c.z + "," + c.w+")";
        world.AddObjectToChunk(transform.position, go);
    }
}
