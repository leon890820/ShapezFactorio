using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControll : MonoBehaviour {

    public static bool buildingMode = false;


    [HideInInspector]
    public static FactorioBuilding bluePrintBuilding;
    [HideInInspector]
    public Vector3 hitPosition = new Vector3();

    public static List<FactorioBuilding> bluePrintBuildings;

    public Camera main_camera;
    public RaycastHit hit;

    public GalaxyManager galaxyManager;

    public static int BuildingLayer = 0;
    public static int rotation = 0;

    public FactorioPlatformBuilding selectedBuilding;
    public static List<Vector3> anchor = new List<Vector3>();

    // Start is called before the first frame update
    void Start(){
    }

    // Update is called once per frame
    void Update() {

        if (!bluePrintBuilding) {
            SelectBuilding();
        }

        if (CameraControl.galaxyMode) {

        } else {          
            if (Input.GetKeyDown(KeyCode.E)) {
                BuildingLayer += 1;
                BuildingLayer = Mathf.Min(10, BuildingLayer);
                galaxyManager.SetGroundPlatformLlayer(BuildingLayer);
            }
            if (Input.GetKeyDown(KeyCode.Q)) {
                BuildingLayer -= 1;
                BuildingLayer = Mathf.Max(0, BuildingLayer);
                galaxyManager.SetGroundPlatformLlayer(BuildingLayer);
            }
        }


        if (bluePrintBuilding == null) return;
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
      

        if (bluePrintBuilding.UpdateAnchor()) {
            ClearBuildings();
            bluePrintBuildings = bluePrintBuilding.GetMultiMuilding(anchor);
        }
        bluePrintBuilding.UpdateBehavior();


        if (Input.GetMouseButtonDown(1)) {
            DisableBlueprintBuilding();
        }
    }

    public void SelectBuilding() {
        Ray ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.MaxValue)) {

            if (Input.GetMouseButtonDown(0)) {
                FactorioGameObjectBase factorioObject = hit.collider.GetComponent<FactorioGameObjectBase>()
                          ?? hit.collider.GetComponentInParent<FactorioGameObjectBase>();
                factorioObject?.SetUIEnable();
            }
        }
    }


    public static void SpawnBuilding(FactorioPrefabBaseObject prefab) {
        if (bluePrintBuilding != null) {
            Destroy(bluePrintBuilding.gameObject);
        }

        bluePrintBuilding = Instantiate(prefab.object_prefab).GetComponent<FactorioBuilding>();
        bluePrintBuilding.gameObject.SetActive(false);     
    }

    

    public void DisableBlueprintBuilding() {
        if (bluePrintBuilding == null) return;
        Destroy(bluePrintBuilding.gameObject);
        bluePrintBuilding = null;

        ClearBuildings();

    }


    public static void AddAnchor(Vector3 pos) {
        anchor.Add(pos);
    }

    public static void PopAnchor() {
        if (anchor.Count == 0) return;
        anchor.RemoveAt(anchor.Count - 1);
    }

    public static void ClearAnchor() { 
        anchor.Clear();
    }

    public static void ClearBuildings() {
        if (bluePrintBuildings == null) return;
        foreach (FactorioBuilding fb in bluePrintBuildings) {
            Destroy(fb.gameObject);
        }
        bluePrintBuildings.Clear();
    }

    public static void PutBuildings() {
        if (bluePrintBuildings == null) return;
        foreach (FactorioBuilding factorioBuilding in bluePrintBuildings) {
            if (!factorioBuilding.TryPutBuilding()) {
                Destroy(factorioBuilding.gameObject);
                continue;
            }
            factorioBuilding.SetOriginalMaterial();
            factorioBuilding.SetBluePrintMode(false);
        }
        bluePrintBuildings.Clear();
        anchor.Clear();
    }


}
