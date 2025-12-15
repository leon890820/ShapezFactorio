using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

abstract public class PowerCosumeBulding : PowerBuilding {

    public float cosumePower = 10f;

    public override void InitBuilding() {
        CreatePowerGrid();
        playGroundPlatform.UpdatePowerGrid();
    }


    private void CreatePowerGrid() {
        HashSet<PowerGrid> powerGrids = new HashSet<PowerGrid>();
        PowerGridManager.Instance.MergePowerGrid(powerGrids, this);
    }

    public virtual float GetCosumePower() {
        return cosumePower;
    }

    public override void SetStatus() {
        buildStatus = EvaluateStatusWithoutPower();
        if (powerGrid == null || (buildStatus != BuildStatus.NoRecipe && !powerGrid.GetAffordPower())) {
            buildStatus = BuildStatus.NoPower;
        }
        buildingStatusController?.SetAlertIcon(buildStatus);
    }

    public virtual BuildStatus EvaluateStatusWithoutPower() {
        return BuildStatus.Working;
    }

}
