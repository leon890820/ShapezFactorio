using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChest : IronChest{
    string factorioObject = "RedSciencePack";

    protected override void Start() {
        base.Start();
        AddObjectToChest();
    }

    void AddObjectToChest() {
        for (int i = 0; i < backpadMax; i++) {
            var gameObjectBase = Instantiate(PrefabManager.Instance.GetPrefab(factorioObject).object_prefab);
            gameObjectBase.transform.parent = transform;
            gameObjectBase.transform.localPosition = Vector3.zero;
            backpad.TryInput(gameObjectBase,0);
        }
    }
}
