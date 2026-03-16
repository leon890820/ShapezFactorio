using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSciencePack : SciencePack {
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("RedSciencePack");
    }

    public override List<FactorioGameObjectBasePacket> GetItemMaterial() {
        return new List<FactorioGameObjectBasePacket>() {
            new FactorioGameObjectBasePacket(PrefabManager.Instance.GetPrefab("Gear"), 1),
            new FactorioGameObjectBasePacket(PrefabManager.Instance.GetPrefab("CopperPlate"), 1),
        };
    }
}
