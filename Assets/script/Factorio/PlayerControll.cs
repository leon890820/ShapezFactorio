using System.Collections;
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

    public void SelectBuilding() {
        Ray ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue)) {
            if (Input.GetMouseButtonDown(0)) {
                FactorioGameObjectBase factorioObject = hit.collider.GetComponent<FactorioGameObjectBase>() ??
                                                        hit.collider.GetComponentInParent<FactorioGameObjectBase>();
                factorioObject?.SetUIEnable();
            }
        }
    }


    public void SpawnBuilding(FactorioPrefabBaseObject prefab) {
        if (bluePrintBuilding != null) {
            Destroy(bluePrintBuilding.gameObject);
        }
        bluePrintBuilding = Instantiate(prefab.object_prefab) as FactorioBuilding;
        bluePrintBuilding.gameObject.SetActive(false);     
    }

    

    public void DisableBlueprintBuilding() {
        if (bluePrintBuilding == null) return;
        Destroy(bluePrintBuilding.gameObject);
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
            factorioBuilding.SetOriginalMaterial();
            factorioBuilding.SetBluePrintMode(false);
            factorioBuilding.InitBuilding();
        }
        bluePrintBuildings.Clear();
        anchor.Clear();
    }


}
