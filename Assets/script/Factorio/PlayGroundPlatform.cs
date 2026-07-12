using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class PlayGroundPlatform : FactorioBuilding {

    public Vector2Int platformSize = new Vector2Int(1, 1);
    public int platformLayer = 10;
    public Transform wallTransform;
    public GameObject notch;
    public GameObject corner;
    public GameObject wall6m;
    public GameObject wall1m;

    static readonly Plane GroundPlane = new Plane(Vector3.up, Vector3.zero);


    private FactorioPlatformBuilding[,,] buildings;
    private FactorioPlatformBuilding[,,] scaffoldings;
    private MeshFilter meshFilter;
    private GameObject colliderGameObject;
    private Vector2Int scale;

    public List<FactorioPlatformBuilding> factorioPlatformBuildings = new List<FactorioPlatformBuilding>();

    // Start is called before the first frame update

    protected override void Awake() {
        InitBuildingAppearance();
        buildings = new FactorioPlatformBuilding[scale.x, platformLayer, scale.y];
        scaffoldings = new Scaffolding[scale.x, platformLayer, scale.y];
        base.Awake();
    }

    void InitBuildingAppearance() {
        InitPlatformMesh();
        InitNotch();
        InitWall();
    }


    protected override void Start(){        
        base.Start();
    }


    public override bool UpdateAnchor() {
        if (!TryGetGroundHit(out var hit)) return false;
        var anchor = PlayerControll.Instance.GetAnchor();
        Vector3 pos = GetPosition(hit);
        if (anchor.Count == 0) {
            PlayerControll.Instance.AddAnchor(pos);
            return true;
        }
        if (anchor[0].Equals(pos)) {
            return false;
        }

        PlayerControll.Instance.ClearAnchor();
        PlayerControll.Instance.AddAnchor(pos);
        return true;

    }

    public override List<FactorioBuilding> GetMultiMuilding(List<Vector3> anchor) {
        List<FactorioBuilding> result = new List<FactorioBuilding>(); ;
        if (anchor.Count == 1) {
            PlayGroundPlatform fb = Instantiate(Clone().object_prefab) as PlayGroundPlatform;
            fb.SetPosition(anchor[0]);
            fb.SetRimMaterial();
            fb.SetValidColor(GalaxyManager.Instance.IsValid(fb) ? 1 : 0);
            fb.CloneBuilding(this);
            result.Add(fb);
        }

        return result;
        
    }


    public override void UpdateBehavior() {
        if (Input.GetMouseButtonDown(0)) {
            PlayerControll.Instance.PutBuildings();
        }
    }

    public override bool TryPutBuilding() {
        return GalaxyManager.Instance.AddPlayground(this);
 
    }


    bool TryGetGroundHit(out Vector3 hitPoint) {
        if (!main_camera) main_camera = Camera.main;
        var ray = main_camera.ScreenPointToRay(Input.mousePosition);
        if (GroundPlane.Raycast(ray, out var dist)) {
            hitPoint = ray.GetPoint(dist);
            return true;
        }
        hitPoint = default;
        return false;
    }

    public ChunkCoord GetChunkCoord() { 
        return GalaxyManager.Instance.GetPlatFormCoordPosition(this);
    }



    public void SetLayer(int n) {
        Collider collider = colliderGameObject.GetComponent<Collider>();
        collider.transform.localPosition = new Vector3(0, n, 0);
        
    }


    public Vector3 GetPosition(Vector3 pos) {
        int texelSize = FactorioData.platformTexelSize;
        int halfTexel = FactorioData.platformHalfTexelSize;

        int gridX = Mathf.FloorToInt(pos.x / texelSize);
        int gridZ = Mathf.FloorToInt(pos.z / texelSize);

        float centerX = gridX * texelSize + halfTexel * (platformSize.x % 2);
        float centerZ = gridZ * texelSize + halfTexel * (platformSize.y % 2);

        return new Vector3(centerX, 0f, centerZ);
    }

    public override void SetPosition(Vector3 pos) { 
        transform.position = GetPosition(pos);
    }

    public bool HasScanffolding(FactorioPlatformBuilding building,Vector3Int[] localPos) {
        if (localPos[0].y == 0) return true;
        for (int i = 0; i < building.buildingSize.x * building.buildingSize.z; i++) {
            //if (!scaffoldings[localPos[i].x, localPos[i].y, localPos[i].z]) return false;
        }
        return true;
    }


    public bool SetBuilding(FactorioPlatformBuilding building) {
        Vector3Int[] localPos = GetBuildingLocalPositions(building);
        FactorioPlatformBuilding[,,] builds = building is Scaffolding ? scaffoldings : buildings;
        if (OutOfBoundary(localPos)) return false;
        if (building is Belt or TeleGraphPole) {
            if (HasBulding(localPos[0], builds)) return false;
        } else {
            if (HasBulding(localPos, builds)) return false;            
        }
        
        if (building is not Scaffolding) { 
            if(!HasScanffolding(building, localPos)) return false;
        }

        for (int i = 0; i < localPos.Length; i++) {
            builds[localPos[i].x, localPos[i].y, localPos[i].z] = building;
        }
        building.transform.parent = transform;
        factorioPlatformBuildings.Add(building);
        building.SetPlayGroundPlatform(this);
        return true;
    }

    public FactorioPlatformBuilding[] GetNeiborBuilding(FactorioPlatformBuilding factorioBuilding) {
        FactorioPlatformBuilding[] result = new FactorioPlatformBuilding[4];
        for (int i = 0; i < result.Length; i++) {
            Vector3Int dir = FactorioData.direction[i];
            Vector3Int pos = GetBuildingLocalPosition(factorioBuilding) + dir;
            if (OutOfBoundary(pos)) continue;
            if (buildings[pos.x, pos.y, pos.z]) {
                result[i] = buildings[pos.x, pos.y, pos.z];
            }

        }

        return result;
    }

    public HashSet<FactorioPlatformBuilding> GetSurroundBuilding(FactorioPlatformBuilding center) {
        var result = new HashSet<FactorioPlatformBuilding>();

        Vector3Int origin = GetBuildingLocalPosition(center);
        // 計算出「外圈起始點」的 offset（假設 direction[0]/[3] 是 +x、-z 或類似）
        Vector3Int offset =
            FactorioData.direction[0] * Mathf.CeilToInt(center.buildingSize.x / 2f) +
            FactorioData.direction[3] * Mathf.CeilToInt(center.buildingSize.z / 2f);
        for (int side = 0; side < 4; side++) {
            int length = (side % 2 == 0 ? center.buildingSize.z : center.buildingSize.x) + 1;
            Vector3Int step = FactorioData.direction[(side + 1) % 4];
            for (int i = 0; i < length; i++) {
                offset += step;
                Vector3Int pos = origin + offset;
                FactorioPlatformBuilding building = GetBuilding(pos);
                if (building != null && building != center) {
                    result.Add(building);
                }
            }
        }

        return result;
    }

    public bool OutOfBoundary(Vector3Int pos) {
        if (pos.x < 0 || pos.x >= scale.x || pos.z < 0 || pos.z >= scale.y || pos.y < 0 || pos.y >= platformLayer) {
            return true;
        }
        return false;
    }
    public bool OutOfBoundary(Vector3Int[] pos) { 
        for (int i = 0; i < pos.Length; i++) {
            if (OutOfBoundary(pos[i])) return true;       
        }
        return false;   
    }

    public bool OnBoundary(Vector3Int pos) { 
        if(pos.x == 0 || pos.x == scale.x - 1 || pos.z == 0 || pos.z == scale.y - 1) return true;
        return false;
    }


    public FactorioPlatformBuilding GetBuilding(Vector3Int pos) {
        if (OutOfBoundary(pos)) return null;
        return buildings[pos.x, pos.y, pos.z];    
    }

    public FactorioPlatformBuilding GetBuilding(Vector3 pos) {
        Vector3Int posi = GetLocalPositions(pos);
        return GetBuilding(posi);
    }

    public FactorioPlatformBuilding GetBuilding(FactorioPlatformBuilding fpb,Vector3Int dir) {
        Vector3Int pos = GetBuildingLocalPosition(fpb) + dir;       
        return GetBuilding(pos);
    }

    public bool HasBulding(Vector3Int[] pos) {

        if (OutOfBoundary(pos)) return true;

        for (int i = 0; i < pos.Length; i++) {
            if (buildings[pos[i].x, pos[i].y, pos[i].z]) {
                return true;
            }
            if (OnBoundary(pos[i])) return true;
        }

        return false;
    }

    public bool HasBulding(Vector3Int pos) {
        if (OutOfBoundary(pos)) return true;
        if (OnBoundary(pos)) return true;
        if (buildings[pos.x, pos.y, pos.z]) return true;        
        return false;
    }

    public bool HasBulding(Vector3Int[] pos, FactorioPlatformBuilding[,,] builds) {
        if (OutOfBoundary(pos)) return true;
        for (int i = 0; i < pos.Length; i++) {
            if (OnBoundary(pos[i])) return true;
            if (builds[pos[i].x, pos[i].y, pos[i].z]) {
                return true;
            }           
        }
        return false;
    }

    public bool HasBulding(Vector3Int pos, FactorioPlatformBuilding[,,] builds) {
        if (OutOfBoundary(pos)) return true;       
        if (OnBoundary(pos) && IsExits(pos) == (-1,-1)) return true;
        if (builds[pos.x, pos.y, pos.z]) return true;
        return false;
    }

    public bool IsValid(FactorioPlatformBuilding building) {
        Vector3Int[] localPos = GetBuildingLocalPositions(building);
        if (building is not Scaffolding) {
            if (!HasScanffolding(building, localPos)) return true;
        }

        if (building is not (Belt or TeleGraphPole)) return HasBulding(localPos, building is Scaffolding ? scaffoldings : buildings);
        else return HasBulding(localPos[0], buildings);        
    }

    public Vector3Int GetLocalPositions(Vector3 position) {
        Vector3 positionBias = position - transform.position;

        Vector2Int gridOffset = scale / 2;

        int originX = Mathf.FloorToInt(positionBias.x) + gridOffset.x;
        int originZ = Mathf.FloorToInt(positionBias.z) + gridOffset.y;

        return new Vector3Int(originX, (int)position.y, originZ);
    }

    public Vector3Int[] GetBuildingLocalPositions(FactorioBuilding building) {
        Vector3Int buildingSize = AbsVector3Int(building.buildingSize);
        Vector3Int[] result = new Vector3Int[buildingSize.x * buildingSize.y * buildingSize.z];
        Vector3 positionBias = building.transform.position - transform.position;
        Vector3Int halfSize = buildingSize / 2;

        Vector2Int gridOffset = scale / 2;

        int originX = Mathf.FloorToInt(positionBias.x) - halfSize.x + gridOffset.x;
        int originZ = Mathf.FloorToInt(positionBias.z) - halfSize.z + gridOffset.y;
        int originY = Mathf.FloorToInt(positionBias.y);

        for (int y = 0; y < buildingSize.y; y++) {
            for (int z = 0; z < buildingSize.z; z++) {
                for (int x = 0; x < buildingSize.x; x++) {
                    result[y * buildingSize.x * buildingSize.z + z * buildingSize.x + x] = new Vector3Int(originX + x * (int)Mathf.Sign(building.buildingSize.x), 
                                                                                                          originY + y * (int)Mathf.Sign(building.buildingSize.y), 
                                                                                                          originZ + z * (int)Mathf.Sign(building.buildingSize.z));
                }
            }
        }

        return result;
    }

    public Vector3 GetOriginalPosition() { 
        Debug.Log( transform.position - new Vector3(platformSize.x * 10, 0, platformSize.y * 10));
        return transform.position - new Vector3(platformSize.x * 10, 0 , platformSize.y * 10);
    }
        
    public Vector3Int AbsVector3Int(Vector3Int v) { 
        return new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));    
    }
    public Vector3Int GetBuildingLocalPosition(FactorioBuilding building) {
        
        Vector3 positionBias = building.transform.position - transform.position;

        Vector2Int gridOffset = scale / 2;

        int originX = Mathf.FloorToInt(positionBias.x)  + gridOffset.x;
        int originZ = Mathf.FloorToInt(positionBias.z)  + gridOffset.y;


        return new Vector3Int(originX, (int)building.transform.position.y , originZ);
    }

    public (int,int) IsExits(Vector3Int pos) {
        if (pos.z == 0 || pos.z == scale.y - 1) {
            for (int i = 0; i < platformSize.x; i++) {
                if (pos.x > 6 + i * 20 && pos.x <= i * 20 + 10) return (pos.z == 0 ? 1 : 3, i);
            }
        }
        if (pos.x == 0 || pos.x == scale.x - 1) {
            for (int i = 0; i < platformSize.y; i++) {
                if (pos.z > 6 + i * 20 && pos.z <= i * 20 + 10) return (pos.x == 0 ? 2 : 0, i);
            }
        }
        return (-1, -1);
    }

    public Vector3Int GetExitsPosition(int rot, int num, Vector2Int bias) {
        int b = rot % 2 == 0 ? bias.y : bias.x;
        int count = 7 + b * 20 + num;
        return rot switch {
            0 => new Vector3Int(scale.x - 1, 0, count),
            1 => new Vector3Int(count, 0, 0),
            2 => new Vector3Int(0, 0, count),
            3 => new Vector3Int(count, 0, scale.y - 1),
            _ => new Vector3Int(0, 0, 0),
        };
    }

    public void UpdatePowerGrid() {
        foreach (var building in buildings) {
            if (building is TeleGraphPole teleGraphPole) {
                teleGraphPole.ReBuildPowerGrid();
            }
        }
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab(GetPlatformId());
    }

    public override void CloneBuilding(FactorioBuilding bulding) {
        var buildings = (bulding as PlayGroundPlatform).factorioPlatformBuildings;
        foreach (var building in buildings) {
            FactorioPlatformBuilding factorioPlatformBuilding = Instantiate(building.Clone().object_prefab) as FactorioPlatformBuilding;
            factorioPlatformBuilding.CloneBuilding(building);
            factorioPlatformBuilding.SetPosition(building.transform.localPosition + transform.position);
            SetBuilding(factorioPlatformBuilding);
        }
    }

    public override void PutBulding() {
        base.PutBulding();
        foreach (var building in factorioPlatformBuildings) {
            building.PutBulding();            
        }
    }

    public override void SaveToBlueprint(string path) {
        var data = GetBlueprintData();
        string json = JsonUtility.ToJson(data, true);

        string folderPath = Path.Combine(Application.dataPath, "Save");

        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = string.IsNullOrEmpty(path) ? "blueprint.json" : path;
        string fullPath = Path.Combine(folderPath, fileName);

        File.WriteAllText(fullPath, json);

        Debug.Log("Saved to: " + fullPath);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public override BlueprintData GetBlueprintData() {
        FactorioId platformId = GetPlatformId();
        var data = new PlayGroundBuildingBlueprintData() {
            id = platformId,
            name = platformId.ToString(),
            x = 0,
            y = 0,
            z = 0,
            rotation = 0,
            buildings = factorioPlatformBuildings.Select(b => b.GetBlueprintData()).ToArray()
        };
        return data;
    }

    public override FactorioBuilding LoadBlueprint(BlueprintData data) {
        var playGroundData = data as PlayGroundBuildingBlueprintData;
        var playGroundBuliding = Instantiate(Clone().object_prefab) as PlayGroundPlatform;
        playGroundBuliding.SetPosition(new Vector3(playGroundData.x, playGroundData.y, playGroundData.z));
        for (int i = 0; i < playGroundData.buildings.Length; i++) { 
            var buildingData = playGroundData.buildings[i];
            var buildingPrefab = PrefabManager.Instance.GetPrefab(buildingData.GetId()).object_prefab as FactorioBuilding;
            var building = buildingPrefab.LoadBlueprint(buildingData) as FactorioPlatformBuilding;
            playGroundBuliding.SetBuilding(building);
        }
        return playGroundBuliding;
        
    }

    private FactorioId GetPlatformId() {
        if (platformSize == new Vector2Int(1, 1)) return FactorioId.PlayerGround1x1;
        if (platformSize == new Vector2Int(2, 1)) return FactorioId.PlayerGround2x1;
        return FactorioId.None;
    }

    private void InitPlatformMesh() {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        scale = platformSize * FactorioData.platformTexelSize - new Vector2Int(2 * 1, 2 * 1);

        Vector3[] vertices = new Vector3[4]{
            new Vector3(-0.5f * scale.x, 0f, -0.5f * scale.y),
            new Vector3(-0.5f * scale.x, 0f,  0.5f * scale.y),
            new Vector3( 0.5f * scale.x, 0f,  0.5f * scale.y),
            new Vector3( 0.5f * scale.x, 0f, -0.5f * scale.y)
        };

        int[] triangles = new int[6]{
            0, 1, 2,
            0, 2, 3
        };

        Vector2[] uvs = new Vector2[4]{
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        colliderGameObject = new GameObject();
        colliderGameObject.layer = 6;
        colliderGameObject.transform.SetParent(transform, false);

        MeshCollider collider = colliderGameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.convex = false;


    }


    void InitNotch() {
        for (int dirIndex = 0; dirIndex < 4; dirIndex++) { 

            Vector3Int mainDir = FactorioData.direction[dirIndex];
            Vector3Int perpDir = FactorioData.direction[(dirIndex + 3) % 4];
            
            int ms = (int) Mathf.Max(Mathf.Abs(perpDir.x * platformSize.x) , Mathf.Abs(perpDir.z * platformSize.y));
            int ss = (int)Mathf.Max(Mathf.Abs(mainDir.x * platformSize.x), Mathf.Abs(mainDir.z * platformSize.y));
            int s = FactorioData.platformHalfTexelSize * ss - 2;
            for (int y = 0; y < ms; y++) {
                                
                int b = -FactorioData.platformHalfTexelSize * (ms - 1) + FactorioData.platformTexelSize * y;
                Vector3 localPos = new Vector3(mainDir.x, 0.0f, mainDir.z) * s + new Vector3(perpDir.x, 0, perpDir.z) * b + Vector3.up * 0.01f;
                Quaternion localRot = Quaternion.Euler(0f, 90f * (dirIndex + 1), 0f);
                PlaceWallPrefab(notch, localPos, localRot);
            }
        }    
    }


    private void PlaceWallPrefab(GameObject prefab, Vector3 localPos, Quaternion localRot) {
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(wallTransform, false);
        go.transform.SetLocalPositionAndRotation(localPos, localRot);
    }
    private void InitWall() {
        int halfTexel = FactorioData.platformHalfTexelSize;
        int fullTexel = FactorioData.platformTexelSize;

        // ======= 放置四個角落 corner ========
        for (int i = 0; i < 4; i++) {
            Vector3Int dir = FactorioData.direction[i] + FactorioData.direction[(i + 3) % 4];

            Vector3 localPos = new Vector3(
                dir.x * (halfTexel * platformSize.x - 2),
                0.01f,
                dir.z * (halfTexel * platformSize.y - 2)
            );

            Quaternion localRot = Quaternion.Euler(0f, 90f * i, 0f);

            PlaceWallPrefab(corner, localPos, localRot);
        }

        // ======= 每個邊方向分別建立牆面 ========
        for (int dirIndex = 0; dirIndex < 4; dirIndex++) {
            Vector3 mainDir = FactorioData.direction[dirIndex];
            Vector3 perpDir = FactorioData.direction[(dirIndex + 3) % 4];

            int lengthAlongEdge = (int)Mathf.Max(Mathf.Abs(perpDir.x * platformSize.x), Mathf.Abs(perpDir.z * platformSize.y));
            int depthOffset = (int)Mathf.Max(Mathf.Abs(mainDir.x * platformSize.x), Mathf.Abs(mainDir.z * platformSize.y));
            int forwardOffset = halfTexel * depthOffset - 2;

            float wall1mOffsetBase = -halfTexel * lengthAlongEdge + 3.5f;

            Quaternion localRot = Quaternion.Euler(0f, 90f * (dirIndex + 1), 0f);

            
            for (int y = 0; y < lengthAlongEdge; y++) {
                // === 放短牆（wall1m）===
                for (int k = 0; k < 3; k++) {
                    Vector3 pos = mainDir * forwardOffset + perpDir * (wall1mOffsetBase + k) ;
                    PlaceWallPrefab(wall1m, new Vector3(pos.x, 0.01f, pos.z), localRot);
                }

                for (int k = -2; k < 1; k++) {
                    Vector3 pos = mainDir * forwardOffset + perpDir * (-wall1mOffsetBase + k);
                    PlaceWallPrefab(wall1m, new Vector3(pos.x, 0.01f, pos.z), localRot);
                }

                // === 放長牆（wall6m）===
                for (int k = 0; k < lengthAlongEdge - 1; k++) {
                    int baseOffset = -halfTexel * (lengthAlongEdge - 1) + fullTexel * k;

                    for (int l = 0; l < 2; l++) {
                        float offset = baseOffset + 7 + 6 * l;
                        Vector3 pos = mainDir * forwardOffset + perpDir * offset;
                        PlaceWallPrefab(wall6m, new Vector3(pos.x, 0.01f, pos.z), localRot);
                    }
                }
            }
        }
    }
}

public class PlayGroundBuildingBlueprintData : BlueprintData {
    public BlueprintData[] buildings;
    public override string ToString() {
        int count = buildings != null ? buildings.Length : 0;

        return $"[PlayGround] name: {name}, pos: ({x},{y},{z}), rot: {rotation}, buildings count: {count}";
    }
}
