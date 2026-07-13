using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FactorioPrefabEntry {
    public FactorioId id;
    public FactorioPrefabBaseObject prefab;
}


[CreateAssetMenu(fileName = "FactorioBaseSetting", menuName = "Factorio/BaseSetting")]
public class FactorioBaseSetting : ScriptableObject {

    [SerializeField]
    private List<FactorioPrefabEntry> prefabEntries = new List<FactorioPrefabEntry>();

    private Dictionary<FactorioId, FactorioPrefabBaseObject> _dict;

    public FactorioPrefabBaseObject GetPrefab(FactorioId id) {
        if (_dict == null) {
            BuildDictionary();
        }

        _dict.TryGetValue(id, out var result);
        return result;
    }

    private void BuildDictionary() {
        _dict = new Dictionary<FactorioId, FactorioPrefabBaseObject>();
        foreach (var entry in prefabEntries) {
            if (entry.id != FactorioId.None && entry.prefab != null) {
                entry.prefab.object_prefab?.InitId(entry.id);
                _dict[entry.id] = entry.prefab;
            }
        }
    }
}

[System.Serializable]
public class FactorioPrefabBaseObject {
    public FactorioGameObjectBase object_prefab;
    public FactorioUIControlBase ui_prefab;
    public Sprite info;
}
