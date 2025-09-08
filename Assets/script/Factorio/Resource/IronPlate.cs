using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronPlate : FactorioGameObjectBase{
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("IronPlate");
    }
}
