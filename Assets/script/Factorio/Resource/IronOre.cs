using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOre : FactorioResource,IBurnable{
    public FactorioGameObjectBasePacket GetBurnProduct() {
        return new FactorioGameObjectBasePacket(PrefabManager.Instance.GetPrefab("IronPlate"), 1);
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("IronOre");
    }

}
