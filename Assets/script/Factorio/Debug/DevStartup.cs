using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        SpawnBuilding("PlayerGround1x1", new Vector3Int());
    }



    void SpawnBuilding(string name, Vector3Int pos) { 
        PlayerControll.Instance.DevPutBlueBuilding(name, pos);
    }
}
