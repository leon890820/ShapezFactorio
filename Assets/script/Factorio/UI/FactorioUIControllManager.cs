using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorioUIControllManager : MonoBehaviour{
    public static FactorioUIControllManager Instance { get; private set; }

    [SerializeField] private FactorioBaseSetting baseSetting;
    [SerializeField] private Transform uiRoot;

    private Dictionary<string, FactorioUIControlBase> uiInstanceMap = new ();
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

        currentUI = GetOrCreateUI(target.GetType().ToString());        
        if (currentUI == null) return;
        currentUI.InitUI();
        currentUI.SetActive(true);
    }

    public void CloseCurrent() {
        if (currentUI == null)
            return;
        currentUI.SetActive(false);
        currentUI = null;
    }

    private FactorioUIControlBase GetOrCreateUI(string uiType) {
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
