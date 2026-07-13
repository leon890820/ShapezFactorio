using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSciencePack : SciencePack {
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab(FactorioId.RedSciencePack);
    }

    public override List<FactorioGameObjectBasePacket> GetItemMaterial() {
        return new List<FactorioGameObjectBasePacket>() {
            new FactorioGameObjectBasePacket(PrefabManager.Instance.GetPrefab(FactorioId.Gear), 1),
            new FactorioGameObjectBasePacket(PrefabManager.Instance.GetPrefab(FactorioId.CopperPlate), 1),
        };
    }
}
