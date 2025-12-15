using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        SpawnBuilding("PlayerGround1x1");
    }



    void SpawnBuilding(string name) { 
        FactorioPrefabBaseObject gameObject = PrefabManager.Instance.GetPrefab(name);
        PlayerControll.Instance.SpawnBuilding(gameObject);
        PlayerControll.Instance.BluePrintBuildingUpdate();
        PlayerControll.Instance.PutBuildings();
        PlayerControll.Instance.DisableBlueprintBuilding();
    }
}
