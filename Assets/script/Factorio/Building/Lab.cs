using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lab : PowerCosumeBulding {

    private float researchCount = 0;
    private float researchSpeed = 1;
    private SciencePack currentSciencePack;

    protected override void Awake() {
        base.Awake();
        backpadMax = 50;
        backpad = new FactorioBackpad(5, backpadMax);
    }


    protected override void Start() {
        base.Start();        
    }
    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos, int i, bool mid = false) {
        if (factorioResource is not SciencePack) return false;
        
        if (backpad.TryInput(factorioResource)) {
            AddFactorioGameObjectToBackpad(factorioResource);
            return true;
        }
        return false;
    }

    private void AddFactorioGameObjectToBackpad(FactorioGameObjectBase factorioResource) {
        factorioResource.transform.SetParent(transform);
        factorioResource.transform.localPosition = new Vector3();
    }

    public override FactorioGameObjectBase TryBeGrab() {
        var grabbedObject = backpad.Pop();
        if (grabbedObject) {
            grabbedObject.transform.SetParent(null);
        }
        return grabbedObject;
    }

    public override void Run() {
        if (buildStatus != BuildStatus.Working)
            return;

        var researchManager = ResearchManager.Instance;
        if (researchManager == null)
            return;

        var scienceList = researchManager.GetResearchScienceList();
        if (scienceList == null || scienceList.Count == 0)
            return;

        factorioUIControlBase.SetValue(researchCount);
        for (int s = 0; s < scienceList.Count; s++) {
            var science = scienceList[s];
            if (science == null)
                continue;

            if (researchManager.IsResearchComplete(science.name))
                continue;

            if (!HasRequiredScienceInBackpad(science))
                continue;

            AddResearchProgress();
            return;
        }
    }

    private bool HasRequiredScienceInBackpad(FactorioGameObjectBase science) {
        if (science == null) return false;
        if (currentSciencePack) return true;

        for (int i = 0; i < backpad.Count(); i++) {
            if (backpad.IsSameType(science, i)) {
                
                currentSciencePack = backpad.Pop(i) as SciencePack;
                return true;
            }
                
        }

        return false;
    }

    private void AddResearchProgress() {
        researchCount += researchSpeed * Time.deltaTime;
        if (researchCount >= 1f) {
            ResearchManager.Instance.IncrementProgress(currentSciencePack.GetType().ToString());
            Destroy(currentSciencePack.gameObject);
            currentSciencePack = null;
            researchCount = 0f;
        }
    }

    public override BuildStatus EvaluateStatusWithoutPower() {
        var list = ResearchManager.Instance.GetResearchScienceList();
        if (list.Count == 0) return BuildStatus.Idle;
        return BuildStatus.Working;

    }

    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }
}
