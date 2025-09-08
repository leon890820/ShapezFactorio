using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.Image;

public class PlayGroundPlatform : FactorioBuilding {

    public Vector2Int platformSize = new Vector2Int(1, 1);

    public int platformLayer = 10;


    FactorioPlatformBuilding[,,] buildings;
    FactorioPlatformBuilding[,,] scaffoldings;

    MeshFilter meshFilter;

    GameObject colliderGameObject;

    Vector2Int scale;

    public GameObject notch;
    public GameObject corner;
    public GameObject wall6m;
    public GameObject wall1m;

    static readonly Plane GroundPlane = new Plane(Vector3.up, Vector3.zero);

    // Start is called before the first frame update

    protected override void Awake() {
        
        //gameObject.layer = 6;
        InitPlatformMesh();
        InitNotch();
        InitWall();
        base.Awake();
    }




    protected override void Start(){
       
        

        buildings = new FactorioPlatformBuilding[scale.x , platformLayer , scale.y];
        scaffoldings = new Scaffolding[scale.x, platformLayer, scale.y];
        base.Start();
    }


    public override bool UpdateAnchor() {
        if (!TryGetGroundHit(out var hit)) return false;
        Vector3 pos = GetPosition(hit);
        if (PlayerControll.anchor.Count == 0) {
            PlayerControll.AddAnchor(pos);
            return true;
        }
        if (PlayerControll.anchor[0].Equals(pos)) {
            return false;
        }

        PlayerControll.ClearAnchor();
        PlayerControll.AddAnchor(pos);
        return true;

    }

    public override List<FactorioBuilding> GetMultiMuilding(List<Vector3> anchor) {
        List<FactorioBuilding> result = new List<FactorioBuilding>(); ;

        if (anchor.Count == 1) {
            PlayGroundPlatform fb = Instantiate(Clone().object_prefab).GetComponent<PlayGroundPlatform>();
            fb.SetPosition(anchor[0]);
            fb.SetRimMaterial();
            fb.SetValidColor(GalaxyManager.IsValid(fb) ? 1 : 0);
            result.Add(fb);
        }

        return result;
        
    }

    public override void UpdateBehavior() {
        if (Input.GetMouseButtonDown(0)) {
            PlayerControll.PutBuildings();
        }
    }

    public override bool TryPutBuilding() {
        return GalaxyManager.AddPlayground(this);
 
    }


    bool TryGetGroundHit(out Vector3 hitPoint) {
        var ray = main_camera.ScreenPointToRay(Input.mousePosition);
        if (GroundPlane.Raycast(ray, out var dist)) {
            hitPoint = ray.GetPoint(dist);
            return true;
        }
        hitPoint = default;
        return false;
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


    public bool SetBulding(FactorioPlatformBuilding building) {
        Vector3Int[] localPos = GetBuildingLocalPositions(building);
        FactorioPlatformBuilding[,,] builds = building is Scaffolding ? scaffoldings : buildings;

        if (HasBulding(localPos, builds)) return false;
        if (OutOfBoundary(localPos))  return false;
        if (building is not Scaffolding) { 
            if(!HasScanffolding(building, localPos)) return false;
        }

        for (int i = 0; i < localPos.Length; i++) {
            builds[localPos[i].x, localPos[i].y, localPos[i].z] = building;
        }


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





    public bool HasBulding(Vector3Int[] pos, FactorioPlatformBuilding[,,] builds) {

        if (OutOfBoundary(pos)) return true;

        for (int i = 0; i < pos.Length; i++) {
            if (builds[pos[i].x, pos[i].y, pos[i].z]) {
                return true;
            }
        }

        return false;
    }

    public bool IsValid(FactorioPlatformBuilding building) {
        Vector3Int[] localPos = GetBuildingLocalPositions(building);
        if (building is not Scaffolding) {
            if (!HasScanffolding(building, localPos)) return true;
        }

        return HasBulding(localPos, building is Scaffolding ? scaffoldings : buildings);
    }

    public Vector3Int GetLocalPositions(Vector3 position) {
        Vector3 positionBias = position - transform.position;

        Vector2Int gridOffset = scale / 2;

        int originX = Mathf.FloorToInt(positionBias.x) + gridOffset.x;
        int originZ = Mathf.FloorToInt(positionBias.z) + gridOffset.y;


        return new Vector3Int(originX, (int)position.y, originZ);
    }

        public Vector3Int[] GetBuildingLocalPositions(FactorioBuilding building) {
        Vector3Int buildingSize = building.buildingSize;
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
                    result[y * buildingSize.x * buildingSize.z + z * buildingSize.x + x] = new Vector3Int(originX + x, originY + y, originZ + z);
                }
            }
        }

        return result;
    }

    public Vector3Int GetBuildingLocalPosition(FactorioBuilding building) {
        
        Vector3 positionBias = building.transform.position - transform.position;

        Vector2Int gridOffset = scale / 2;

        int originX = Mathf.FloorToInt(positionBias.x)  + gridOffset.x;
        int originZ = Mathf.FloorToInt(positionBias.z)  + gridOffset.y;


        return new Vector3Int(originX, (int)building.transform.position.y , originZ);
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("PlayerGround");
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
                GameObject go = Instantiate(notch);
                go.transform.SetParent(pivotTransform, false);
                go.transform.SetLocalPositionAndRotation(localPos, localRot);
            }
        }    
    }


    private void PlaceWallPrefab(GameObject prefab, Vector3 localPos, Quaternion localRot) {
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(pivotTransform, false);
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
