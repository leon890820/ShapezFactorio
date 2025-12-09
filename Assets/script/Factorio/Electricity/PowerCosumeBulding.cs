using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PowerCosumeBulding : PowerBuilding{
    public override void InitBuilding() {
        CreatePowerGrid();
        playGroundPlatform.UpdatePowerGrid();
    }


    private void CreatePowerGrid() {
        HashSet<PowerGrid> powerGrids = new HashSet<PowerGrid>();
        PowerGridManager.Instance.MergePowerGrid(powerGrids, this);
    }
}
