using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SenderBelt : Belt
{
    Mesh MeshType {
        get {
            return type switch {
                BeltType.SENDER => meshes[0],
                BeltType.RECEIVER => meshes[1],
                _ => meshes[0],
            };
        }
    }


    private Belt receiverBelt;
    protected override void Awake() {
        base.Awake();
        
    }
    protected override void Start() {

        Belt belt = GetComponent<Belt>();
        belt.enabled = false;

        base.Start();
    }


    public override void SetBuildingType(PlayGroundPlatform pgp) {
        SetRimMaterial();
        SetValidColor(pgp.IsValid(this) ? 1 : 0);
        playGroundPlatform = pgp;

        Vector3Int localPos = pgp.GetBuildingLocalPosition(this);
        (int sender, int num) = pgp.IsExits(localPos);
        if (sender == -1) return;
        if (TrySpawnSender(sender, num)) {
            SetBuildingTypeSender(sender);
            TrySpawnReceiver(sender, num);
        } else {
            SetBuildingTypeReceiver(R(sender, 2));
        }
        
    }

    public override void SetBuildingTypeForce(PlayGroundPlatform pgp, int dirI) {
        SetBuildingType(pgp);
    }

    public void SetBuildingTypeSender(int rot) {
        type = BeltType.SENDER;
        SetRotation(rot);
        meshFilter.mesh = MeshType;
    }

    public void SetBuildingTypeReceiver(int rot) {
        type = BeltType.RECEIVER;
        SetRotation(rot);
        meshFilter.mesh = MeshType;
    }

    public bool TrySpawnSender(int rot, int num) {
        PlayGroundPlatform neibor = GalaxyManager.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);
        if (!neibor) return true;
        Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
        SenderBelt nbelt = neibor.GetBuilding(pos)?.GetComponent<SenderBelt>();
        if (!nbelt) return true;
        if (nbelt.type == BeltType.SENDER) {
            nbelt.receiverBelt = this;
            return false;
        }
        return true;
    }

    public void TrySpawnReceiver(int rot, int num) {
        PlayGroundPlatform neibor = GalaxyManager.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);
        if (!neibor) return;
        Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
        SenderBelt belt = Instantiate(Clone().object_prefab).GetComponent<SenderBelt>();
        belt.UpdateBlueprintState(pos, neibor);
        belt.SetBuildingTypeReceiver(rot);
        receiverBelt = belt;
        PlayerControll.bluePrintBuildings.Add(belt);
    }


}
