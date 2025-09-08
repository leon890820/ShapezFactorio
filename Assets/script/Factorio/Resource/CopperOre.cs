using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopperOre : FactorioResource,IBurnable {
    public FactorioPrefabBaseObject GetBurnProduct() {
        return PrefabManager.Instance.GetPrefab("CopperPlate");
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("CopperOre");
    }
}
