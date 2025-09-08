using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FactorioPlatformBuilding : FactorioBuilding{

    protected List<FactorioGameObjectBase> backpad = new List<FactorioGameObjectBase>();
    protected int backpadMax = 1;

    [HideInInspector]
    public PlayGroundPlatform playGroundPlatform;

    

    [HideInInspector]
    private bool beSelected = false;

    // Start is called before the first frame update

    protected override void Awake() {
        base.Awake();
        
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        
    }



    public void SetSelect(bool b) { 
        beSelected = b;
        
    }


    public override bool UpdateAnchor() {
        if (!TryGetPlatformUnderMouse(out var hit, out var pgp)) return false;
        Vector3 pos = Floor(hit.point);
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
            TryGetPlatformUnderMouse(out var hit, out var pgp, anchor[0]);
            FactorioPlatformBuilding fb = Instantiate(Clone().object_prefab).GetComponent<FactorioPlatformBuilding>();
            fb.SetRotation(PlayerControll.rotation);
            fb.UpdateBlueprintState(anchor[0], pgp);      
            
            result.Add(fb);
        }

        return result;

    }

    public override void UpdateBehavior() {
        if (Input.GetMouseButtonDown(0)) {
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

    public override bool TryPutBuilding() {
        TryGetPlatformUnderMouse(out var hit, out var pgp, transform.position);
        return pgp.SetBulding(this);

    }

    public bool TryGetPlatformUnderMouse(out RaycastHit hit, out PlayGroundPlatform pgp) {
        int mask = LayerMask.GetMask("playground");
        Ray ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.MaxValue, mask)) {
            pgp = hit.collider.transform.parent.GetComponent<PlayGroundPlatform>();
            return pgp != null;
        }

        pgp = null;
        return false;
    }

    public bool TryGetPlatformUnderMouse(out RaycastHit hit, out PlayGroundPlatform pgp, Vector3 point) {
        int mask = LayerMask.GetMask("playground");
        Ray ray = new Ray(main_camera.transform.position, (point - main_camera.transform.position).normalized);

        if (Physics.Raycast(ray, out hit, float.MaxValue, mask)) {
            pgp = hit.collider.transform.parent.GetComponent<PlayGroundPlatform>();
            return pgp != null;
        }

        pgp = null;
        return false;
    }

    public virtual void UpdateBlueprintState(Vector3 hitPoint, PlayGroundPlatform pgp) {
        SetRimMaterial();
        SetPosition(hitPoint);
        SetValidColor(pgp.IsValid(this) ? 1 : 0);
        SetBuildingType(pgp);
        playGroundPlatform = pgp;
    }


    public virtual bool TryInput(FactorioGameObjectBase factorioResource,Vector3Int pos, int i,bool mid = false) {
        return false;
    }

    public virtual FactorioGameObjectBase TryBeGrab() { 
        return null;
    }

    public override FactorioPrefabBaseObject Clone() {
        return null;
    }

    public virtual void SetBuildingType(PlayGroundPlatform pgp) { }

    public virtual BuildingDirection GetDirectionType(Vector3Int pos, int dir) {
        

        return BuildingDirection.NONE;
    }



    public enum BuildingDirection {
        NONE,
        INPUT,
        OUPUT,
    }

}





public static class FactorioGameObjectUIManager {
    public static List<FactorioUIControlBase> UIList = new List<FactorioUIControlBase>();

    public static void ClearAllUI() {
        foreach (FactorioUIControlBase controller in UIList) { 
            controller.SetActive(false);
        }
        UIList.Clear();
    }

    public static void AddUI(FactorioUIControlBase fui) {
        ClearAllUI();
        UIList.Add(fui);
        fui.SetActive(true);
    }

}