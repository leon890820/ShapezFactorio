using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : PuttableObject {
    public Transform pivot;
    public Wire wire;
    public Board board;

    // Start is called before the first frame update
    public override void CreatObjectOnTheWorld(Vector3 p, Quaternion q) {
        GameObject go = Instantiate(puttable_object, p, q);
        ChunkCoord c = new(p);
        GameObject pt = GameObject.Find("Switch");
        go.GetComponentInChildren<Switch>().ChangeTypeAndRotation(type, rotation);
        go.transform.parent = pt.transform;
        go.name = "Switch : [" + c.x + "," + c.y + "] (" + c.z + "," + c.w + ")";
        for (int y = 0; y < voxel_side.y; y += 1) {
            for (int x = 0; x < voxel_side.x; x += 1) {
                world.AddObjectToChunk(transform.position + new Vector3(x, 0, y), go);
            }
        }

    }

    void Start() {
        world = GameObject.Find("World").GetComponent<GenerateWorld>();
        puttable_object = world.puttable_object_setting.objectPack[1].object_prefab;
        voxel_side = new Vector2Int(1, 1);      
    }

    public override void ChangeTypeAndRotation(int t, int r) {
        type = t; rotation = r;
        pivot.rotation = Quaternion.Euler(0, rotation * 90.0f, 0);
    }



    // Update is called once per frame
    void Update() {
        if (board.change) {
            board.change = false;
            wire.SetPower(board.GetTurn());
        }
    }
}
