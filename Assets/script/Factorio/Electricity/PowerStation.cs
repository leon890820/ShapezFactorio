using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerStation : PowerBuilding {
    public int capacity;

    protected override void Start() {
        base.Start();
        powerGridUIController = factorioUIControlBase as PowerGridUIController;
    }

    protected override void Update() {
        base.Update();
        SetPowerGridUI();
    }

    void SetPowerGridUI() {
        if (powerGrid == null) return;
        powerGridUIController.SetTotalPoewrText(powerGrid.GetPowerCapacity());
    }

    public override void InitBuilding() {
        CreatePowerGrid();
    }



    private void CreatePowerGrid() {
        HashSet<PowerGrid> powerGrids = new HashSet<PowerGrid>();
        var neighbor = playGroundPlatform.GetSurroundBuilding(this);

        foreach (var building in neighbor) {
            if (building is not PowerBuilding powerBuilding) continue;
            AddConnectPowerBuilding(powerBuilding);
            powerBuilding.AddConnectPowerBuilding(this);

            powerGrids.Add(powerBuilding.GetPowerGrid());
        }
        PowerGridManager.Instance.MergePowerGrid(powerGrids, this);
    }
}
