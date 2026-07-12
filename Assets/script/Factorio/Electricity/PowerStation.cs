using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerStation : PowerBuilding {
    public float capacity;

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
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
            var grid = powerBuilding.GetPowerGrid();
            if (grid == null) continue;
            powerGrids.Add(grid);
        }
        PowerGridManager.Instance.MergePowerGrid(powerGrids, this);
    }
}
