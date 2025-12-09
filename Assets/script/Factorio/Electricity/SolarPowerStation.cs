using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPowerStation : PowerStation{
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("SolarPowerStation");
    }
}
