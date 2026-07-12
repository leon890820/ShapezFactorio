using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTelePole : TeleGraphPole{
    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab(FactorioId.SmallTelePole);
    }

    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }
}
