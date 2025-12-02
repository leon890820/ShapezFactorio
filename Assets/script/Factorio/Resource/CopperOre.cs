using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopperOre : FactorioResource,IBurnable {
    public FactorioGameObjectBasePacket GetBurnProduct() {
        return new FactorioGameObjectBasePacket( PrefabManager.Instance.GetPrefab("CopperPlate") , 1);
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("CopperOre");
    }
}
