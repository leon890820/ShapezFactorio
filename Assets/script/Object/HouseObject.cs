using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Building : PuttableObject {
    
}
public class HouseObject : Building {
    public int max_villige = 4;
    private void Start() {
        
        world = GameObject.Find("world").GetComponent<GenerateWorld>();
        //puttable_object = world.puttable_object_setting.house.object_prefab;

    }

    public override void CreatObjectOnTheWorld(Vector3 p, Quaternion q) {
        GameObject go = Instantiate(puttable_object, p, q);
        ChunkCoord c = new(p);
        GameObject pt = GameObject.Find("House");
        go.GetComponentInChildren<HouseObject>().ChangeTypeAndRotation(type, rotation);
        go.transform.parent = pt.transform;
        go.name = "House : [" + c.x + "," + c.y + "] (" + c.z + "," + c.w + ")";
        for (int y = 0; y < voxel_side.y; y += 1) {
            for (int x = 0; x < voxel_side.x; x += 1) {
                world.AddObjectToChunk(transform.position + new Vector3(x,0,y), go);
            }
        }
        
    }
}

public class StoreHouse : Building {
    
    public override void CreatObjectOnTheWorld(Vector3 p, Quaternion q) {

    }
}
