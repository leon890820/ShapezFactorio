using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FactorioPrefabEntry {
    public string key;
    public FactorioPrefabBaseObject prefab;
}


[CreateAssetMenu(fileName = "FactorioBaseSetting", menuName = "Factorio/BaseSetting")]
public class FactorioBaseSetting : ScriptableObject {

    [SerializeField]
    private List<FactorioPrefabEntry> prefabEntries = new List<FactorioPrefabEntry>();

    private Dictionary<string, FactorioPrefabBaseObject> _dict;

    public FactorioPrefabBaseObject GetPrefab(string key) {
        if (_dict == null) {
            BuildDictionary();
        }

        _dict.TryGetValue(key, out var result);
        return result;
    }

    private void BuildDictionary() {
        _dict = new Dictionary<string, FactorioPrefabBaseObject>();
        foreach (var entry in prefabEntries) {
            if (!string.IsNullOrEmpty(entry.key) && entry.prefab != null) {
                _dict[entry.key] = entry.prefab;
            }
        }
    }
}

[System.Serializable]
public class FactorioPrefabBaseObject {
    public string name;
    public GameObject object_prefab;
    public Sprite info;
}