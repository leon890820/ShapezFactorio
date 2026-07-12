using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerGridUIController : FactorioUIControlBase{
    public TextMeshProUGUI totalPowerText;
    private PowerBuilding powerBuilding;

    public override void BindFactorioGameObject(FactorioGameObjectBase factorioGameObjectBase) {
        powerBuilding = factorioGameObjectBase as PowerBuilding;
    }

    public override void UpdateUI() {
        if (!powerBuilding) return;
        var powerGrid = powerBuilding.GetPowerGrid();
        if (powerGrid == null) return;
        SetTotalPoewrText(powerGrid.GetPowerCapacity());
    }

    public void SetTotalPoewrText(float power) { 
        totalPowerText.text = "Total Power : " + power.ToString();
    }

}
