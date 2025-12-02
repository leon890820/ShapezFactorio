using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : FactorioItem{
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("Gear");
    }
    public override List<FactorioGameObjectBasePacket> GetItemMaterial() {
        return new List<FactorioGameObjectBasePacket>() {
            new FactorioGameObjectBasePacket(PrefabManager.Instance.GetPrefab("IronPlate"), 2),
            null,
        };
    }

}
