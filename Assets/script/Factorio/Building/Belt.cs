using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;


public class Belt : FactorioPlatformBuilding {

    public float beltSpeed = 0.5f;
    public Mesh[] meshes;
    public MeshFilter meshFilter;

    public BeltType type = BeltType.Straight;

    private int bias_rotation;

    private FactorioGameObjectBase[,] beltBackpad;
    private float[,] beltCount;
    protected BuildingDirection[] beltDirections;
    private FactorioGameObjectBase transport;
    private int transportCount;

    protected Vector3 midPos = new Vector3(0.5f, 0.24f, 0.5f);

    Mesh MeshType {
        get {
            return type switch {
                BeltType.Straight => meshes[0],
                BeltType.LEFT or
                BeltType.RIGHT => meshes[1],
                BeltType.ONETO2L or
                BeltType.ONETO2R => meshes[2],
                BeltType.ONETO2T => meshes[3],
                BeltType.CROSS => meshes[4],
                BeltType.TWO2ONER or
                BeltType.TWO2ONEL => meshes[5],
                BeltType.TWO2ONET => meshes[6],
                BeltType.CROSS2ONE => meshes[7],
                BeltType.SENDER => meshes[8],
                BeltType.RECEIVER => meshes[9],
                _ => meshes[0],
            };
        }
    }

    protected override void Awake() {
        beltBackpad = new FactorioGameObjectBase[4,2];
        beltCount = new float[4, 2];
        beltDirections = new BuildingDirection[4];
        base.Awake();        
    }



    protected override void Update() {      
        SetBeltTile();
        base.Update();
    }



    public override void Run() {
        for (int i = 0; i < 4; i++) {
            if (beltDirections[i] == BuildingDirection.NONE) continue;

            UpdateBackpadProgress(i); // 所有格子推進動畫

            if (beltDirections[i] == BuildingDirection.INPUT)
                HandleInputDirection(i);
            else
                HandleOutputDirection(i);
        }

        if (transport) {
            for (int i = 0; i < 4; i++) {
                int d = R(i, transportCount);
                if (beltDirections[d] == BuildingDirection.NONE || beltDirections[d] == BuildingDirection.INPUT) continue;
                if (beltBackpad[d, 0]) continue;
                transportCount = (d + 1) % 4;
                beltBackpad[d, 0] = transport;
                transport = null;
                break;
            }        
        }

        for (int i = 0; i < 4; i++) {
            for (int y = 0; y < 2; y++) {
                if (beltBackpad[i, y]) {
                    beltBackpad[i, y].transform.localPosition = GetResourceLocalPosition(i, y, beltCount[i,y]);                   
                }    
            }
        }

    }

    void UpdateBackpadProgress(int dir) {
        for (int y = 0; y < 2; y++) {
            if (beltBackpad[dir, y])
                beltCount[dir, y] += Time.deltaTime * beltSpeed * 4;
        }
    }

    void HandleInputDirection(int dir) {
        // 把物品從遠端移到近端
        if (beltCount[dir, 1] > 1f && !beltBackpad[dir, 0] && !transport) {
            beltBackpad[dir, 0] = beltBackpad[dir, 1];
            beltBackpad[dir, 1] = null;
            beltCount[dir, 1] = 0f;
        }

        // 把近端物品放到中心 transport
        if (beltCount[dir, 0] > 1f && transport == null) {
            transport = beltBackpad[dir, 0];
            beltBackpad[dir, 0] = null;
            beltCount[dir, 0] = 0f;
        }
    }

    void HandleOutputDirection(int dir) {
        // 把物品從近端推到遠端
        if (beltCount[dir, 0] > 1f && !beltBackpad[dir, 1]) {
            beltBackpad[dir, 1] = beltBackpad[dir, 0];
            beltBackpad[dir, 0] = null;
            beltCount[dir, 0] = 0f;
        }

        // 遠端滿了且前進完成，嘗試輸出
        if (beltCount[dir, 1] > 1f) {
            TryOutput(dir); // 可依方向決定輸出邏輯
        }
    }


    public virtual void TryOutput(int dir) {
        Vector3Int direction = FactorioData.direction[dir];
        Vector3Int pos = playGroundPlatform.GetLocalPositions(transform.position) + new Vector3Int(direction.x, 0, direction.y);
        FactorioPlatformBuilding neighbor = playGroundPlatform.GetBuilding(this, direction);

        if (!neighbor) return;

        // 嘗試將遠端物品輸出到鄰居
        if (neighbor.TryInput(beltBackpad[dir, 1], pos, (dir + 2) % 4, false)) {
            beltBackpad[dir, 1] = null;
            beltCount[dir, 1] = 0f;
        }
    }

    public override FactorioGameObjectBase TryBeGrab() {
        for (int i = 0; i < 4; i++){
            for (int j = 0; j < 2; j++){
                FactorioGameObjectBase resource = beltBackpad[i, j];
                if (resource != null) {
                    resource.transform.SetParent(null);  // 斷開與 Belt 的關聯
                    beltBackpad[i, j] = null;
                    return resource;
                }
            }
        }

        return null; // 沒有資源可抓
    }

    public override bool TryInput(FactorioGameObjectBase resource,Vector3Int pos, int dir, bool mid) {
        if (mid) {
            if (transport != null) return false;

            resource.transform.SetParent(transform);
            resource.transform.localPosition = midPos;
            transport = resource;
            return true;
        } else {

            
            if (beltDirections[dir] is BuildingDirection.OUPUT or BuildingDirection.NONE) return false;
            if (beltBackpad[dir, 1]) return false;

            resource.transform.SetParent(transform);
            resource.transform.localPosition = GetResourceLocalPosition(dir, 1);

            beltBackpad[dir, 1] = resource;
            return true;
        }
    }

    #region PutBulding

    public override bool UpdateAnchor() {
        if (!TryGetPlatformUnderMouse(out var hit, out var pgp)) return false;
        Vector3 pos = Floor(hit.point);

       
        var anchor = PlayerControll.anchor;
        if (anchor.Count == 0) {
            PlayerControll.AddAnchor(pos);
            return true;
        }
        
        if (anchor[anchor.Count - 1].Equals(pos)) {
            return false;
        }

        TryGetPlatformUnderMouse(out var hit0, out var pgp0, anchor[0]);
        if (!pgp0) return false;
        if(!pgp0.Equals(pgp)) return false;

        PlayerControll.PopAnchor();
        PlayerControll.AddAnchor(pos);
        return true;

    }

    public override List<FactorioBuilding> GetMultiMuilding(List<Vector3> anchor) {
        List<FactorioBuilding> result = new List<FactorioBuilding>();
        TryGetPlatformUnderMouse(out var hit, out var pgp, anchor[0]);

        if (anchor.Count == 1) {           
            FactorioPlatformBuilding fb = Instantiate(Clone().object_prefab).GetComponent<FactorioPlatformBuilding>();
            fb.SetRotation(PlayerControll.rotation);
            fb.UpdateBlueprintState(anchor[0], pgp);
            fb.SetBuildingType(pgp);
            result.Add(fb);
        } else if (anchor.Count == 2) {
            int dirIndex = GetDirection(anchor[1] - anchor[0]);
            if (anchor[1].y - anchor[0].y != 0) {
                StairBelt fb = Instantiate(Clone().object_prefab).GetComponent<StairBelt>();
                fb.enabled = true;
                fb.SetUpStair(anchor[1].y - anchor[0].y > 0);
                fb.SetRotation(PlayerControll.rotation);
                fb.SetRotationSec(dirIndex);
                fb.UpdateBlueprintState(anchor[0], pgp);
                fb.SetBuildingType(pgp);
                result.Add(fb);
                return result;
            }
            
            Vector3 dir = FactorioData.direction[dirIndex];
            int count = (int)Mathf.Max(Mathf.Abs(anchor[1].x - anchor[0].x), Mathf.Abs(anchor[1].z - anchor[0].z)) + 1;

            for (int i = 0; i < count; i++) {               
                Belt fb = Instantiate(Clone().object_prefab).GetComponent<Belt>();
                fb.SetRotation(dirIndex);
                fb.UpdateBlueprintState(anchor[0] + dir * i, pgp);
                if(i == 0) fb.SetBuildingType(pgp);
                else fb.SetBuildingTypeForce(pgp, fb.rotation);
                result.Add(fb);
            }

        }

        return result;

    }

    int GetDirection(Vector3 dir) {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) {
            return dir.x > 0 ? 0 : 2;
        } else {
            return dir.z > 0 ? 3 : 1;
        }
    }

    public override void UpdateBlueprintState(Vector3 hitPoint, PlayGroundPlatform pgp) {
        SetRimMaterial();
        SetPosition(hitPoint);
        SetValidColor(pgp.IsValid(this) ? 1 : 0);
        playGroundPlatform = pgp;       
    }

    public override void UpdateBehavior() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 anchor = PlayerControll.anchor[0];
            PlayerControll.AddAnchor(anchor);
        } else if (Input.GetMouseButtonUp(0)) {
            PlayerControll.ClearAnchor();
            PlayerControll.PutBuildings();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            PlayerControll.rotation += 1;
            foreach (FactorioPlatformBuilding fb in PlayerControll.bluePrintBuildings) {
                fb.TryGetPlatformUnderMouse(out var hit, out var pgp, fb.transform.position);
                fb.SetRotation(PlayerControll.rotation);
                fb.SetBuildingType(pgp);
            }
        }
    }

    #endregion


    public Vector3 GetResourceLocalPosition(int i, int y) {
        return GetResourceLocalPosition(i, y, 0f);
    }

    public Vector3 GetResourceLocalPosition(int i, int y, float time) {
        
        time = Mathf.Clamp01(time);
        int d2 = beltDirections[i] == BuildingDirection.OUPUT ? 1 : -1;  
        int d1 = beltDirections[i] == BuildingDirection.OUPUT ? y : y + 1;

        Vector3 dir = FactorioData.direction[i];
        Vector3 pos = midPos + dir * 0.25f * (d1 + d2 * time);
        return pos;

    }



    protected virtual void ApplyMeshTransform() {
        switch (type) {
            case BeltType.LEFT:
            case BeltType.ONETO2L:
            case BeltType.TWO2ONEL:
            case BeltType.UPLEFT:
            case BeltType.DOWNLEFT:
                pivotTransform.localScale = new Vector3(1, 1f, 1);
                break;
            case BeltType.RIGHT:
            case BeltType.ONETO2R:
            case BeltType.TWO2ONER:
            case BeltType.UPRIGHT:
            case BeltType.DOWNRIGHT:
                pivotTransform.localScale = new Vector3(1, 1f, -1);
                break;

            default:
                pivotTransform.localScale = new Vector3(1, 1f, 1);
                break;
        }
    }


    public void SetBeltTile() {
        float t = (-Time.time * beltSpeed) % 1;
        foreach (var renderer in meshRenderers) {
            originalMaterials[renderer][0].SetTextureOffset("_MainTex", new Vector2(t , 0));
        }
        rimMaterial.SetTextureOffset("_MainTex", new Vector2(t, 0));
    }

    public override void SetPosition(Vector3 pos) {
        transform.position = Floor(pos);
    }

    public override void AddRotation() {
        rotation = (rotation + 1) % 4;
        PlayerControll.rotation = rotation;
        pivotTransform.rotation = Quaternion.Euler(0.0f, (rotation + bias_rotation) * 90.0f, 0.0f);
    }

    public override void SetRotation(int i) {
        rotation = (i + 4) % 4;
        pivotTransform.rotation = Quaternion.Euler(0.0f, (rotation + bias_rotation) * 90.0f, 0.0f);
    }

    public override void SetBuildingType(PlayGroundPlatform pgp) {

        ResetAllDirection();
        FactorioPlatformBuilding[] buildings = pgp.GetNeiborBuilding(this);
        Vector3Int localPos = pgp.GetBuildingLocalPosition(this);
        (int sender, int num) = pgp.IsExits(localPos);
        if (sender != -1) {
            if (TrySpawnSender(sender, num)) {
                SetBuildingTypeSender(sender);
                TrySpawnReceiver(sender, num);
            } else {
                SetBuildingTypeReceiver(R(sender, 2));
            }
            return;
        }

        for (int i = 0; i < buildings.Length; i++) {
            if (!buildings[i]) continue;
            Vector3Int dir = FactorioData.direction[i];
            beltDirections[i] = buildings[i].GetDirectionType(localPos + dir, R(i , 2));
        }

        beltDirections[rotation] = BuildingDirection.OUPUT;
        if (CheckAllDirectionNoInput()) {
            beltDirections[R(rotation, 2)] = BuildingDirection.INPUT;
        }

        SetBeltType();
        SetRotation(rotation);
        meshFilter.mesh = MeshType;
        ApplyMeshTransform();
     
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
        Belt nbelt = (Belt)neibor.GetBuilding(pos);
        if (!nbelt) return true;
        if (nbelt.type == BeltType.SENDER) return false;        
        return true;
    }

    public void TrySpawnReceiver(int rot, int num) {
        PlayGroundPlatform neibor = GalaxyManager.GetNeiborPlayGroundPlatform(playGroundPlatform, rot, num);
        if (!neibor) return;
        Vector3 pos = transform.position + FactorioData.direction[rot] * 3;
        Belt belt = Instantiate(Clone().object_prefab).GetComponent<Belt>();
        belt.UpdateBlueprintState(pos, neibor);
        belt.SetBuildingTypeReceiver(rot);
        PlayerControll.bluePrintBuildings.Add(belt);
    }

    public void SetBuildingTypeForce(PlayGroundPlatform pgp, int dirI) {

        ResetAllDirection();
        FactorioPlatformBuilding[] buildings = pgp.GetNeiborBuilding(this);
        Vector3Int localPos = pgp.GetBuildingLocalPosition(this);
        (int sender, int num) = pgp.IsExits(localPos);
        if (sender != -1) {
            if (TrySpawnSender(sender, num)) {
                SetBuildingTypeSender(sender);
                TrySpawnReceiver(sender, num);
            } else {
                SetBuildingTypeReceiver(R(sender, 2));
            }
            return;
        }

        for (int i = 0; i < buildings.Length; i++) {
            if (!buildings[i]) continue;
            Vector3Int dir = FactorioData.direction[i];
            beltDirections[i] = buildings[i].GetDirectionType(localPos + dir, R(i, 2));
        }


        beltDirections[rotation] = BuildingDirection.OUPUT;
        if (CheckAllDirectionNoInput()) {
            beltDirections[R(rotation, 2)] = BuildingDirection.INPUT;
        }
        
        beltDirections[R(dirI, 0)] = BuildingDirection.OUPUT;
        beltDirections[R(dirI, 2)] = BuildingDirection.INPUT;

        

        SetBeltType();
        SetRotation(rotation);
        meshFilter.mesh = MeshType;
        ApplyMeshTransform();

    }


    protected virtual void SetBeltType() {
        List<int> outputDirs = new();
        List<int> inputDirs = new();

        // 收集輸入與輸出方向
        for (int i = 0; i < beltDirections.Length; i++) {
            if (beltDirections[i] == BuildingDirection.OUPUT)
                outputDirs.Add(i);
            else if (beltDirections[i] == BuildingDirection.INPUT)
                inputDirs.Add(i);
        }

        int outCount = outputDirs.Count;
        int inCount = inputDirs.Count;

        // 十字型：三輸出（自動假設單一輸入）
        if (outCount == 3) {
            type = BeltType.CROSS;
            bias_rotation = inputDirs[0] + 2 - rotation + 1;
        }
        // 三叉型：兩輸出一輸入
        else if (outCount == 2 && inCount == 1) {
            int directionSum = outputDirs[0] + outputDirs[1] - 2 * inputDirs[0];
            int n = R(directionSum, 8); // 方向旋轉量
            bias_rotation = inputDirs[0] + 2 - rotation;
            switch (n) {
                case 0:
                    type = BeltType.ONETO2T; break;
                case 1:
                    type = BeltType.ONETO2R; break;
                case 3:
                    type = BeltType.ONETO2L; break;
            }
        } else if (outCount == 1 && inCount == 2) {
            int directionSum = inputDirs[0] + inputDirs[1] - 2 * outputDirs[0];
            int n = R(directionSum, 8); // 方向旋轉量
            bias_rotation = outputDirs[0]  - rotation;
            switch (n) {
                case 0:
                    type = BeltType.TWO2ONET; bias_rotation = outputDirs[0] + 2 - rotation; break;
                case 3:
                    type = BeltType.TWO2ONER; bias_rotation = outputDirs[0] - rotation; break;
                case 1:
                    type = BeltType.TWO2ONEL; bias_rotation = outputDirs[0] - rotation; break;
            }
        }
        // 單輸出單輸入：直線 / 彎道
        else if (outCount == 1 && inCount == 1) {
            int n = R(outputDirs[0] - inputDirs[0], 4);

            switch (n) {
                case 1:
                    type = BeltType.LEFT; bias_rotation = 1; break;
                case 2:
                    type = BeltType.Straight; bias_rotation = 0; break;
                case 3:
                    type = BeltType.RIGHT; bias_rotation = 3; break;
                default:
                    type = BeltType.Straight; bias_rotation = 0; break;
            }
        } else {
            // 預設 fallback
            type = BeltType.Straight;
            bias_rotation = 0;
        }

    }

    private bool CheckAllDirectionNoInput() {
        
        for (int i = 0; i < beltDirections.Length; i++) {
            if (beltDirections[i] == BuildingDirection.INPUT) return false;
        }
        return true;
    }

    private void ResetAllDirection() {
        for (int i = 0; i < beltDirections.Length; i++) {
            beltDirections[i] = BuildingDirection.NONE;
        }
    }


    public override BuildingDirection GetDirectionType(Vector3Int pos, int dir) {

        if (beltDirections[dir] == BuildingDirection.OUPUT) return BuildingDirection.INPUT;
        if (beltDirections[dir] == BuildingDirection.INPUT) return BuildingDirection.OUPUT;
        return BuildingDirection.NONE;
    }

   
    public static int R(int baseDir, int rot) => (baseDir + rot) % 4;

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("Belt");
    }

    public enum BeltType {
        Straight,
        LEFT,
        RIGHT,
        ONETO2L,
        ONETO2R,
        ONETO2T,
        CROSS,
        TWO2ONEL,
        TWO2ONER,
        TWO2ONET,
        CROSS2ONE,
        UPSTRAIGHT,
        UPLEFT,
        UPBACK,
        UPRIGHT,
        DOWNSTRAIGHT,
        DOWNLEFT,
        DOWNBACK,
        DOWNRIGHT,
        SENDER,
        RECEIVER
    }
    


}
