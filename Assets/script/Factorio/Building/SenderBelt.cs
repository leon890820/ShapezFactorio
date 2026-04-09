using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private SenderBelt receiverBelt;
    public FactorioGameObjectBase[] beltBackpad;
    private float[] beltCount;
    FactorioGameObjectBase senderObject;
    private float senderCount;

    protected override void Awake() {
        base.Awake();
        beltBackpad = new FactorioGameObjectBase[4];
        beltCount = new float[4];
    }
    protected override void Start() {
        var belts = GetComponents<Belt>();

        Belt belt = null;

        foreach (var b in belts) {
            if (b.GetType() == typeof(Belt)) {
                belt = b;
                break;
            }
        }

        if (belt != null) {
            belt.enabled = false;
        } else {
            Debug.LogWarning("No pure Belt found on " + gameObject.name);
        }

        base.Start();
    }

    public override void Run() {

        for (int i = 0; i < beltBackpad.Length; i++) {
            if (!beltBackpad[i]) continue;
            beltCount[i] += Time.deltaTime * beltSpeed * 4f;
        }

        for (int i = 0; i < beltCount.Length - 1; i++) {
            if (beltCount[i] > 1f) {
                if (!beltBackpad[i + 1]) {
                    beltCount[i] = 0f;
                    beltBackpad[i + 1] = beltBackpad[i];
                    beltBackpad[i] = null;
                }
            }
        }
        if (beltCount[^1] > 1f) {
            if (type == BeltType.SENDER) {
                if (!senderObject && receiverBelt && !receiverBelt.beltBackpad[0]) {
                    beltCount[^1] = 0f;
                    senderObject = beltBackpad[^1];
                    beltBackpad[^1] = null;
                }
            } else {
                TryOutput(rotation);
            }
        }

        for (int i = 0; i < beltCount.Length; i++) {
            if (!beltBackpad[i]) continue;
            beltBackpad[i].transform.localPosition = GetResourceLocalPosition(i, beltCount[i]);
        }

        if (type != BeltType.SENDER) return;
        if (senderObject) {
            senderCount += Time.deltaTime * beltSpeed;
            senderObject.transform.localPosition = GetResourceLocalPosition(senderCount);
        }
        if (senderCount > 1f) {
            if (receiverBelt.TryInput(senderObject, new Vector3Int(), R(rotation, 2), false)) {
                senderCount = 0f;
                senderObject = null;
            }
            
        }

        
    }

    public override bool TryInput(FactorioGameObjectBase resource, Vector3Int pos, int dir, bool mid) {
        if (mid) return false;
        if (beltDirections[dir] is BuildingDirection.OUPUT or BuildingDirection.NONE) return false;
        if (beltBackpad[0]) return false;

        resource.transform.SetParent(transform);
        resource.transform.localPosition = GetResourceLocalPosition(0, 0f);
        beltBackpad[0] = resource;

        return true;
    }


    public override void TryOutput(int dir) {
        Vector3Int direction = FactorioData.direction[dir];
        Vector3Int pos = playGroundPlatform.GetLocalPositions(transform.position) + new Vector3Int(direction.x, 0, direction.y);
        FactorioPlatformBuilding neighbor = playGroundPlatform.GetBuilding(this, direction);

        if (!neighbor) return;

        // 嘗試將遠端物品輸出到鄰居
        if (neighbor.TryInput(beltBackpad[^1], pos, R(dir , 2), false)) {
            beltBackpad[^1] = null;
            beltCount[^1] = 0f;
        }
    }

    public Vector3 GetResourceLocalPosition(int i, float time) {
        time = Mathf.Clamp01(time);
        Vector3 dir = FactorioData.direction[rotation];
        float x = (i + time);
        float t = Mathf.Clamp01(x - 0.5f);
        if (type == BeltType.SENDER)
            return midPos + (time - 2f + i) * 0.25f * dir + 0.25f * 0.33f * (x - 1) * (3 * t * t - 2 * t * t * t) * Vector3.up;
        x = 4f - (i + time);
        t = Mathf.Clamp01(x - 0.5f);
        return midPos + (time - 2f + i) * 0.25f * dir + 0.25f * 0.56f * (x - 1) * (3 * t * t - 2 * t * t * t) * Vector3.up;
    }

    public Vector3 GetResourceLocalPosition(float time) {
        time = Mathf.Clamp01(time);
        Vector3 dir = FactorioData.direction[rotation];
        Vector3 pos = midPos + 0.5f * dir + (-1.6f * time * time + 1.8f * time + 0.25f) * Vector3.up + dir * time * 2f;
        return pos;
    }

    public override void SetBuildingType(PlayGroundPlatform pgp) {
        SetRimMaterial();
        SetValidColor(pgp.IsValid(this) ? 1 : 0);
        playGroundPlatform = pgp;
        Vector3Int localPos = pgp.GetBuildingLocalPosition(this);
        (int sender, int num) = pgp.IsExits(localPos);
        if (sender == -1) return;
        ResetAllDirection();
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
        beltDirections[R(rot, 2)] = BuildingDirection.INPUT;
        type = BeltType.SENDER;
        SetRotation(rot);
        meshFilter.mesh = MeshType;
    }

    public void SetBuildingTypeReceiver(int rot) {
        beltDirections[R(rot, 2)] = BuildingDirection.INPUT;
        beltDirections[R(rot, 4)] = BuildingDirection.OUPUT;
        type = BeltType.RECEIVER;
        SetRotation(rot);
        meshFilter.mesh = MeshType;
    }

    public bool TrySpawnSender(int rot, int num) {
        PlayGroundPlatform neibor = GalaxyManager.Instance.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);
        if (!neibor) return true;
        Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
        SenderBelt nbelt = neibor.GetBuilding(pos)?.GetComponent<SenderBelt>();
        if (!nbelt) return true;
        if (nbelt.type == BeltType.SENDER) {
            return false;
        }
        return true;
    }

    public void TrySpawnReceiver(int rot, int num) {
        PlayGroundPlatform neibor = GalaxyManager.Instance.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);
        if (!neibor) return;
        Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
        SenderBelt belt = Instantiate(Clone().object_prefab).GetComponent<SenderBelt>();
        belt.enabled = true;
        belt.UpdateBlueprintState(pos, neibor);
        belt.SetBuildingTypeReceiver(rot);
        receiverBelt = belt;
        PlayerControll.Instance.bluePrintBuildings.Add(belt);
    }

    public override bool TryPutBuilding() {
        TryGetPlatformUnderMouse(out var hit, out var pgp, transform.position);
        if (pgp.IsValid(this)) return false;
        if (type == BeltType.RECEIVER) {
            Vector3Int localPos = pgp.GetBuildingLocalPosition(this);            
            (int rot, int num) = pgp.IsExits(localPos);
            PlayGroundPlatform neibor = GalaxyManager.Instance.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);            
            Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
            SenderBelt nbelt = neibor.GetBuilding(pos)?.GetComponent<SenderBelt>();
            if(nbelt) nbelt.receiverBelt = this;            
        }
        return pgp.SetBuilding(this);
    }

    public void FindSenderBelt() {
        Vector3Int localPos = playGroundPlatform.GetBuildingLocalPosition(this);
        (int rot, int num) = playGroundPlatform.IsExits(localPos);
        PlayGroundPlatform neibor = GalaxyManager.Instance.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);
        if (!neibor) return;
        if (type == BeltType.RECEIVER) {
            Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
            SenderBelt nbelt = neibor.GetBuilding(pos)?.GetComponent<SenderBelt>();
            if (nbelt) nbelt.receiverBelt = this;
        } else if (type == BeltType.SENDER) {
            Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
            SenderBelt nbelt = neibor.GetBuilding(pos)?.GetComponent<SenderBelt>();
            if (nbelt?.type == BeltType.RECEIVER) receiverBelt = nbelt;
        }
    }

    public override void PutBulding() {
        SetOriginalMaterial();
        SetBluePrintMode(false);
        InitBuilding();
        FindSenderBelt();
    }

    protected override void ApplyMesh() {
        meshFilter.mesh = MeshType;
        ApplyMeshTransform();
    }

    public override void CloneBuilding(FactorioBuilding building) {
        enabled = true;
        var belt = building as SenderBelt;
        bias_rotation = belt.bias_rotation;
        SetRotation(belt.GetRotation());
        type = belt.type;
        for (int i = 0; i < beltDirections.Length; i++) {
            beltDirections[i] = belt.beltDirections[i];
        }
        meshFilter.mesh = MeshType;
        ApplyMeshTransform();
    }

    public override BlueprintData GetBlueprintData() {
        var extra = new BeltExtraData() {
            beltType = type,
            biasRotation = bias_rotation,
            beltDirections = beltDirections,
        };
        var bias = playGroundPlatform.platformSize * 10;
        return new BlueprintData() {
            name = GetType().Name,
            x = Mathf.FloorToInt(transform.localPosition.x) + bias.x,
            y = Mathf.FloorToInt(transform.localPosition.y),
            z = Mathf.FloorToInt(transform.localPosition.z) + bias.y,
            rotation = GetRotation(),
            extraJson = JsonUtility.ToJson(extra)
        };
    }

}
