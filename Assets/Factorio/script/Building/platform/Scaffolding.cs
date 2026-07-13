using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaffolding : FactorioPlatformBuilding{

    public GameObject leg;


    protected override void Awake() {
        Init();
        base.Awake();        
    }


    // Start is called before the first frame update
    protected override void Start(){   
        base.Start();
    }


    private void Init() {
        for (int i = 0; i < 4; i++) { 
            Vector3Int dir = FactorioData.direction[i] + FactorioData.direction[(i + 1) % 4];
            GameObject go = Instantiate(leg);
            go.transform.SetParent(pivotTransform, false);
            go.transform.localPosition = new Vector3(dir.x * buildingSize.x * 0.5f , -1.0f , dir.z * buildingSize.z * 0.5f);
        }
    }

    public override void SetPosition(Vector3 pos) {


        int gridX = Mathf.FloorToInt(pos.x);
        int gridZ = Mathf.FloorToInt(pos.z);

        float centerX = gridX  + 0.5f * (buildingSize.x % 2);
        float centerZ = gridZ  + 0.5f * (buildingSize.z % 2);

        transform.position = new Vector3(centerX, pos.y, centerZ);
    }

}
