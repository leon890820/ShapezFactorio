using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StairBelt : Belt {

    Mesh MeshType {
        get {
            return type switch {
                BeltType.UPSTRAIGHT => meshes[0],
                BeltType.UPLEFT => meshes[1],
                BeltType.UPBACK => meshes[2],
                BeltType.UPRIGHT => meshes[1],
                _ => meshes[0],
            };
        }
    }

    private int rotation_sec;
    private FactorioGameObjectBase[] beltBackpad;
    private float[] beltCount;

    protected override void Awake() {       
        base.Awake();
        buildingSize = new Vector3Int(1, 2, 1);
        beltDirections = new BuildingDirection[8];

        beltBackpad = new FactorioGameObjectBase[6];
        beltCount = new float[6];
    }


    protected override void Start() {

        Belt belt = GetComponent<Belt>();
        belt.enabled = false;
        
        base.Start();
    }

    public override void Run() {
        for (int i = 0; i < beltCount.Length; i++) {
            if (beltBackpad[i]) {
                beltCount[i] += Time.deltaTime * beltSpeed * 4f;
            }
        }

        for (int i = 0; i < beltCount.Length - 1; i++) {
            if (beltCount[i] > 1f) {
                if (beltBackpad[i + 1]) continue;
                beltCount[i] = 0f;
                beltBackpad[i + 1] = beltBackpad[i];
                beltBackpad[i] = null;
            }
        }
        if (beltCount[^1] > 1f) {
            TryOutput(rotation_sec);
        }

        for (int i = 0; i < beltCount.Length; i++) {
            if (!beltBackpad[i]) continue;
            beltBackpad[i].transform.localPosition = GetResourceLocalPosition(i, beltCount[i]);
        }       

    }

    public override void TryOutput(int dir) {
        Vector3Int direction = FactorioData.direction[dir] + new Vector3Int(0,1,0);        
        Vector3Int pos = playGroundPlatform.GetLocalPositions(transform.position) + direction;
        FactorioPlatformBuilding neighbor = playGroundPlatform.GetBuilding(this, direction);

        if (!neighbor) return;

        // 嘗試將遠端物品輸出到鄰居
        if (neighbor.TryInput(beltBackpad[^1], pos, R(dir + 2, 4), false)) {
            beltBackpad[^1] = null;
            beltCount[^1] = 0f;
        }
    }

    public Vector3 GetResourceLocalPosition(float i, float time) {
        time = Mathf.Clamp01(time);
        Vector3 dir = FactorioData.direction[rotation];
        Vector3 dir_sec = FactorioData.direction[rotation_sec];
        if (i < 2) {
            return midPos  + dir * 0.25f * (time - 2f + i);
        } else if (i < 4) {
            return midPos + Vector3.up * 0.5f * (i - 2f + time);
        } else {
            return midPos + Vector3.up + dir_sec * 0.25f * (time + i - 4);
        }
        
    }

    public override bool TryInput(FactorioGameObjectBase resource, Vector3Int pos, int dir, bool mid) {
        if (mid) {
            return false;
        } else {
            Vector3Int localPos = playGroundPlatform.GetLocalPositions(transform.position);
            int bias = pos.y - localPos.y;

            if (beltDirections[dir + bias * 4] is BuildingDirection.OUPUT or BuildingDirection.NONE) return false;
            if (beltBackpad[0]) return false;

            resource.transform.SetParent(transform);
            resource.transform.localPosition = GetResourceLocalPosition(0, 0f);

            beltBackpad[0] = resource;
            return true;
        }
    }

    public override void SetBuildingType(PlayGroundPlatform pgp) {
        for (int i = 0; i < 4; i++) {
            beltDirections[i] = (rotation + 2) % 4 == i ? BuildingDirection.INPUT : BuildingDirection.NONE;
        }
        for (int i = 4; i < 8; i++) {
            beltDirections[i] = rotation_sec % 4 == i - 4 ? BuildingDirection.OUPUT : BuildingDirection.NONE;
        }

        SetRotation(rotation);
        SetBeltType();        
        meshFilter.mesh = MeshType;
        ApplyMeshTransform();
    }

    protected override void SetBeltType() {
        int bias = R(rotation_sec - rotation, 4);
        switch (bias) {
            case 0:
                type = BeltType.UPSTRAIGHT;
                break;
            case 1:
                type = BeltType.UPRIGHT;
                break;
            case 2:
                type = BeltType.UPBACK;
                break;
            case 3:
                type = BeltType.UPLEFT;
                break;
        
        }

    }

    public void SetRotationSec(int r) { 
        rotation_sec = (r + 4) % 4;
    }

    public override BuildingDirection GetDirectionType(Vector3Int pos, int dir) {
        Vector3Int localPos =  playGroundPlatform.GetBuildingLocalPosition(this);
        int bias = pos.y - localPos.y;

        if (beltDirections[dir + bias * 4] == BuildingDirection.OUPUT) return BuildingDirection.INPUT;
        if (beltDirections[dir + bias * 4] == BuildingDirection.INPUT) return BuildingDirection.OUPUT;
        return BuildingDirection.NONE;
    }



}
