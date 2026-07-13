using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorioUIControllManager : MonoBehaviour{
    public static FactorioUIControllManager Instance { get; private set; }

    [SerializeField] private FactorioBaseSetting baseSetting;
    [SerializeField] private Transform uiRoot;

    private Dictionary<FactorioId, FactorioUIControlBase> uiInstanceMap = new ();
    private FactorioUIControlBase currentUI;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Open(FactorioGameObjectBase target) {
        CloseCurrent();

        if (target == null) return;

        currentUI = GetOrCreateUI(target.GetId());
        if (currentUI == null) return;
        currentUI.Open(target);
    }

    public void CloseCurrent() {
        if (currentUI == null)
            return;
        currentUI.Close();
        currentUI = null;
    }

    private FactorioUIControlBase GetOrCreateUI(FactorioId uiType) {
        if (uiType == FactorioId.None) return null;

        if (uiInstanceMap.TryGetValue(uiType, out var instance)) {
            return instance;
        }

        var prefab = baseSetting.GetPrefab(uiType)?.ui_prefab;
        if (!prefab) return null;

        FactorioUIControlBase newUI = Instantiate(prefab, uiRoot);
        uiInstanceMap[uiType] = newUI;

        return newUI;
    }

}
