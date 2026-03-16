using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronChest : FactorioPlatformBuilding{
    
    public int chestSize = 30;


    protected override void Awake() {
        base.Awake();
        backpadMax = 30;
        backpad = new FactorioBackpad(chestSize, backpadMax);
    }

    protected override void Start() {
        base.Start();        
    }
    
    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }

    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos, int i, bool mid = false) {
        if (backpad.TryInput(factorioResource)) {
            AddFactorioGameObjectToBackpad(factorioResource);
            return true;
        }
        return false;
    }

    private void AddFactorioGameObjectToBackpad(FactorioGameObjectBase factorioResource) {
        factorioResource.transform.SetParent(transform);
        factorioResource.transform.localPosition = new Vector3();        
        factorioUIControlBase.UpdateUI();
    }

    public override FactorioGameObjectBase TryBeGrab() {
        var grabbedObject = backpad.Pop();
        if (grabbedObject) {
            factorioUIControlBase.UpdateUI();
            grabbedObject.transform.SetParent(null);            
        }
        return grabbedObject;
    }

}
