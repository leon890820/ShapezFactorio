using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopperPlate : FactorioGameObjectBase {
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("CopperPlate");
    }
}
