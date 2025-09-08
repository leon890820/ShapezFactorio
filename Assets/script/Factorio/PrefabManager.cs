using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {
    public static PrefabManager Instance { get; private set; }

    public FactorioBaseSetting factorioBaseSetting;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public FactorioPrefabBaseObject GetPrefab(string name) {
        return factorioBaseSetting.GetPrefab(name);
    }

    public Sprite GetSprite(string name) {
        return factorioBaseSetting.GetPrefab(name).info;
    }
}
