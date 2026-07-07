using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FactorioPlatformBuilding : FactorioBuilding{

    public BuildingStatusController buildingStatusController;
    public FactorioBackpad backpad;
    protected int backpadMax = 1;
    protected PlayGroundPlatform playGroundPlatform;
    

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

    public override bool UpdateAnchor() {
        if (!TryGetPlatformUnderMouse(out var hit, out var pgp)) return false;

        var anchor = PlayerControll.Instance.GetAnchor();
        Vector3 pos = Floor(hit.point);
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
            TryGetPlatformUnderMouse(out var hit, out var pgp, anchor[0]);
            FactorioPlatformBuilding fb = Instantiate(Clone().object_prefab) as FactorioPlatformBuilding;
            fb.SetRotation(PlayerControll.Instance.rotation);
            fb.UpdateBlueprintState(anchor[0], pgp);                  
            result.Add(fb);
        }
        return result;
    }

    public override void UpdateBehavior() {
        if (Input.GetMouseButtonDown(0)) {
            PlayerControll.Instance.PutBuildings();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            PlayerControll.Instance.rotation += 1;
            foreach (FactorioPlatformBuilding fb in PlayerControll.Instance.GetBluePrintBuildings()) {
                fb.TryGetPlatformUnderMouse(out var hit, out var pgp, fb.transform.position);
                fb.SetRotation(PlayerControll.Instance.GetRotation());
                fb.SetBuildingType(pgp);
            }
        }
    }

    public override bool TryPutBuilding() {
        bool hasPlatform = TryGetPlatformUnderMouse(out var hit, out var playGroundPlatform, transform.position);
        return hasPlatform && playGroundPlatform.SetBuilding(this);
    }

    public bool TryGetPlatformUnderMouse(out RaycastHit hit, out PlayGroundPlatform playGroundPlatform) {
        if (!main_camera) main_camera = Camera.main;
        int mask = LayerMask.GetMask("playground");

        Ray ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.MaxValue, mask)) {
            playGroundPlatform = hit.collider.transform.parent.GetComponent<PlayGroundPlatform>();
            return playGroundPlatform != null;
        }

        playGroundPlatform = null;
        return false;
    }

    public bool TryGetPlatformUnderMouse(out RaycastHit hit, out PlayGroundPlatform playGroundPlatform, Vector3 point) {
        int mask = LayerMask.GetMask("playground");
        Ray ray = new Ray(main_camera.transform.position, (point - main_camera.transform.position).normalized);

        if (Physics.Raycast(ray, out hit, float.MaxValue, mask)) {
            playGroundPlatform = hit.collider.transform.parent.GetComponent<PlayGroundPlatform>();
            return playGroundPlatform != null;
        }

        playGroundPlatform = null;
        return false;
    }

    public virtual void UpdateBlueprintState(Vector3 hitPoint, PlayGroundPlatform playGroundPlatform) {
        SetRimMaterial();
        SetPosition(hitPoint);
        SetValidColor(playGroundPlatform.IsValid(this) ? 1 : 0);
        SetBuildingType(playGroundPlatform);
        SetPlayGroundPlatform(playGroundPlatform);
    }

    public void SetPlayGroundPlatform(PlayGroundPlatform playGroundPlatform) {
        this.playGroundPlatform = playGroundPlatform;
    }

    public virtual bool TryInput(FactorioGameObjectBase factorioResource,Vector3Int pos, int i,bool mid = false) {
        return false;
    }

    public virtual FactorioGameObjectBase TryBeGrab() { 
        return null;
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab(GetType().Name);
    }

    public virtual void SetBuildingType(PlayGroundPlatform pgp) { }
    public override void CloneBuilding(FactorioBuilding bulding) {
        SetRotation(bulding.GetRotation());
    }

    public override void SaveToBlueprint(string path) {

    }

    public override BlueprintData GetBlueprintData() { 
        var bias = playGroundPlatform.platformSize * 10;
        return new PlatFormBuildingBlueprintData() {
            name = GetType().Name,
            x = Mathf.FloorToInt(transform.localPosition.x) + bias.x, 
            y = Mathf.FloorToInt(transform.localPosition.y), 
            z = Mathf.FloorToInt(transform.localPosition.z) + bias.y, 
            rotation = GetRotation() 
        };
    }
    public override FactorioBuilding LoadBlueprint(BlueprintData data) {
        var building = Instantiate(Clone().object_prefab) as FactorioPlatformBuilding;
        building.SetPosition(new Vector3(data.x, data.y, data.z));
        building.SetRotation(data.rotation);
        building.gameObject.SetActive(false);
        return building;
    }

    public virtual BuildingDirection GetDirectionType(Vector3Int pos, int dir) {        
        return BuildingDirection.NONE;
    }

    public enum BuildingDirection {
        NONE,
        INPUT,
        OUPUT,
    }

}

[Serializable]
public class PlatFormBuildingBlueprintData : BlueprintData {
    
}
