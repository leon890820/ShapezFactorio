using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControll : MonoBehaviour {

    public static PlayerControll Instance { get; private set; }
    public List<FactorioBuilding> bluePrintBuildings;
    public Camera main_camera;
    public GalaxyManager galaxyManager;

    [HideInInspector] public int rotation = 0;

    private int buildingLayer = 0;
    private List<Vector3> anchor = new List<Vector3>();
    private FactorioBuilding bluePrintBuilding;

    private List<FactorioBuilding> selectBuildings = new List<FactorioBuilding>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    // Start is called before the first frame update
    void Start() {
        bluePrintBuildings = new List<FactorioBuilding>();
    }

    // Update is called once per frame
    void Update() {

        if (!bluePrintBuilding) {
            SelectBuilding();
        }

        HandleLayerChangeInput();
        if (EventSystem.current.IsPointerOverGameObject()) return;        
        BluePrintBuildingUpdate();
        CopyBuilding();
    }

    private void HandleLayerChangeInput() {
        if (CameraControl.Instance.GalaxyMode) {

        } else {
            if (Input.GetKeyDown(KeyCode.E)) {
                buildingLayer += 1;
                buildingLayer = Mathf.Min(10, buildingLayer);
                galaxyManager.SetGroundPlatformLlayer(buildingLayer);
            }
            if (Input.GetKeyDown(KeyCode.Q)) {
                buildingLayer -= 1;
                buildingLayer = Mathf.Max(0, buildingLayer);
                galaxyManager.SetGroundPlatformLlayer(buildingLayer);
            }
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            SkillNodeManager.Instance.ToggleUI();
        }
    }

    public void BluePrintBuildingUpdate() {
        if (bluePrintBuilding == null) return;

        if (bluePrintBuilding.UpdateAnchor()) {
            ClearBuildings();
            var buildings = bluePrintBuilding.GetMultiMuilding(anchor);
            bluePrintBuildings.AddRange(buildings);
        }
        bluePrintBuilding.UpdateBehavior();

        if (Input.GetMouseButtonDown(1)) {
            DisableBlueprintBuilding();
        }

    }

    public void DevPutBlueBuilding(string buildingName, Vector3Int pos) { 
        var buildingPrefab = PrefabManager.Instance.GetPrefab(buildingName);
        SpawnBuilding(buildingPrefab);
        AddAnchor(pos);
        bluePrintBuildings = bluePrintBuilding.GetMultiMuilding(anchor);
        PutBuildings();
        DisableBlueprintBuilding();
    }

    public void SelectBuilding() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Ray ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue)) {
            if (Input.GetMouseButtonDown(0)) {
                FactorioGameObjectBase factorioObject = hit.collider.GetComponent<FactorioGameObjectBase>() ??
                                                        hit.collider.GetComponentInParent<FactorioGameObjectBase>();
                factorioObject?.SetUIEnable();
                if (factorioObject is FactorioBuilding build) {
                    selectBuildings.Clear();
                    selectBuildings.Add(build);
                }
            }
        }
    }

    public void CopyBuilding() {
        if (bluePrintBuilding) return;
        if (selectBuildings.Count == 0) return;
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C)) {
            bluePrintBuilding = selectBuildings[0];
            selectBuildings.Clear();            
        }
    }


    public void SpawnBuilding(FactorioPrefabBaseObject prefab) {
        bluePrintBuilding = (prefab.object_prefab) as FactorioBuilding;     
    }

    

    public void DisableBlueprintBuilding() {
        if (bluePrintBuilding == null) return;
        bluePrintBuilding = null;

        ClearBuildings();
        anchor.Clear();

    }

    public List<FactorioBuilding> GetBluePrintBuildings() { 
        return bluePrintBuildings;
    }

    public void SetRotation(int rot) { 
        rotation = rot;
    }
    public int GetRotation() {
        return rotation;
    }

    public int GetBuildingLayer() {
        return buildingLayer;
    }

    public List<Vector3> GetAnchor() {
        return anchor;
    
    }

    public void AddAnchor(Vector3 pos) {
        anchor.Add(pos);
    }

    public void PopAnchor() {
        if (anchor.Count == 0) return;
        anchor.RemoveAt(anchor.Count - 1);
    }

    public void ClearAnchor() { 
        anchor.Clear();
    }

    public void ClearBuildings() {
        if (bluePrintBuildings == null) return;
        foreach (FactorioBuilding factorioBuilding in bluePrintBuildings) {
            Destroy(factorioBuilding.gameObject);
        }
        bluePrintBuildings.Clear();
    }

    

    public void PutBuildings() {
        if (bluePrintBuildings == null) return;
        foreach (FactorioBuilding factorioBuilding in bluePrintBuildings) {
            if (!factorioBuilding.TryPutBuilding()) {
                Destroy(factorioBuilding.gameObject);
                continue;
            }
            factorioBuilding.PutBulding();
        }
        bluePrintBuildings.Clear();
        anchor.Clear();
    }


}
