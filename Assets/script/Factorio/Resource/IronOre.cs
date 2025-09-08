using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOre : FactorioResource,IBurnable{
    public FactorioPrefabBaseObject GetBurnProduct() {
        return PrefabManager.Instance.GetPrefab("IronPlate");
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("IronOre");
    }

}
