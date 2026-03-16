using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : FactorioPlatformBuilding {
    private FactorioBackpad labBackpad;
    private LabUIControll labUIControll;

    protected override void Start() {
        base.Start();
        backpadMax = 50;
        labBackpad = new FactorioBackpad(5, backpadMax);
        labUIControll = factorioUIControlBase as LabUIControll;
        
        labUIControll.UpdateUI(labBackpad.backpad);
    }
    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos, int i, bool mid = false) {
        if (factorioResource is not SciencePack) return false;
        
        if (labBackpad.TryInput(factorioResource)) {
            AddFactorioGameObjectToBackpad(factorioResource);
            return true;
        }
        return false;
    }

    private void AddFactorioGameObjectToBackpad(FactorioGameObjectBase factorioResource) {
        factorioResource.transform.SetParent(transform);
        factorioResource.transform.localPosition = new Vector3();
        labUIControll.UpdateUI(labBackpad.backpad);
    }

    public override FactorioGameObjectBase TryBeGrab() {
        var grabbedObject = labBackpad.Pop();
        if (grabbedObject) {
            labUIControll.UpdateUI(labBackpad.backpad);
            grabbedObject.transform.SetParent(null);
        }
        return grabbedObject;
    }

    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }
}
