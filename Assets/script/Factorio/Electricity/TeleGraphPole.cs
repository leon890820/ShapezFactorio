using GogoGaga.OptimizedRopesAndCables;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TeleGraphPole : PowerBuilding{

    [Header("TeleGraphPole")]
    public Transform wireTransform;
    public Rope ropePrefab;
    
    public int connectionRange = 5;
    public int powerRange = 2;
    public float ropeWide = 0.03f;
    public float ropeLengthMult = 1.2f;

    protected override void Start() {
        base.Start();
        powerGridUIController = factorioUIControlBase as PowerGridUIController;
    }

    public override void InitBuilding() {
        CreatePowerGrid();
    }


    private void CreatePowerGrid() {
        HashSet<PowerGrid> powerGrids = new HashSet<PowerGrid>();
        var connectionNeighbor = GalaxyManager.Instance.FindSurroundPlatformBuildings<TeleGraphPole>(this, connectionRange);
        var powerNeighbor = GalaxyManager.Instance.FindSurroundPlatformBuildings<PowerBuilding>(this, powerRange);

        AddPowerBuildingInPowerGrid(powerGrids, connectionNeighbor);
        AddPowerBuildingInPowerGrid(powerGrids, powerNeighbor);
        PowerGridManager.Instance.MergePowerGrid(powerGrids, this);
    }

    public void ReBuildPowerGrid() {
        HashSet<PowerGrid> powerGrids = new HashSet<PowerGrid> {powerGrid};
        var powerNeighbor = GalaxyManager.Instance.FindSurroundPlatformBuildings<PowerBuilding>(this, powerRange);
        AddPowerBuildingInPowerGrid(powerGrids, powerNeighbor);
        PowerGridManager.Instance.MergePowerGrid(powerGrids);
    }

    private void AddPowerBuildingInPowerGrid<T>(HashSet<PowerGrid> powerGrids,HashSet<T> neighbor) where T : PowerBuilding{
        foreach (var building in neighbor) {
            AddConnectPowerBuilding(building);
            building.AddConnectPowerBuilding(this);
            powerGrids.Add(building.GetPowerGrid());
        }

    }

    public void CreateWire() {
        var neighborBuildings = GalaxyManager.Instance.FindSurroundPlatformBuildings<TeleGraphPole>(this, connectionRange);
        foreach (var building in neighborBuildings) {
            InitRope(building);
        }
    }

    public void InitRope(TeleGraphPole teleGraph) {
        Rope rope = Instantiate(ropePrefab);
        rope.transform.SetParent(transform);
        rope.SetStartPoint(wireTransform);
        rope.SetEndPoint(teleGraph.wireTransform);
        rope.ropeLength = Vector3.Distance(transform.position, teleGraph.transform.position) * ropeLengthMult;
        RopeMesh mesh = rope.AddComponent<RopeMesh>();
        mesh.ropeWidth = ropeWide;
    }

    public override List<FactorioBuilding> GetMultiMuilding(List<Vector3> anchor) {
        List<FactorioBuilding> result = new List<FactorioBuilding>(); ;

        if (anchor.Count == 1) {
            TryGetPlatformUnderMouse(out var hit, out var pgp, anchor[0]);
            TeleGraphPole fb = Instantiate(Clone().object_prefab) as TeleGraphPole;
            fb.SetRotation(PlayerControll.Instance.rotation);
            fb.UpdateBlueprintState(anchor[0], pgp);
            fb.CreateWire();
            result.Add(fb);
        }
        return result;
    }

}
