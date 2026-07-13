using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ProductionUnlockCondition : MonoBehaviour, IUnlockCondition{
    [SerializeField] private ProductionUnlockConditionData[] conditions;
    public bool IsUnlocked() {
        return true;
    }

    public FactorioGameObjectBasePacket[] GetUnlockDescription() {
        FactorioGameObjectBasePacket[] packet = new FactorioGameObjectBasePacket[conditions.Length];
        for(int i = 0; i < conditions.Length; i++) {
            Debug.Log("conditions[i].name : " + PrefabManager.Instance);
            FactorioPrefabBaseObject factorioPrefabBaseObject = PrefabManager.Instance.GetPrefab(conditions[i].id);
            packet[i] = new FactorioGameObjectBasePacket(factorioPrefabBaseObject, conditions[i].requiredCount);
        }
        return packet;
    }

    public ProductionUnlockConditionData[] GetUnlockConditionData() {
        return conditions;
    }
}

[Serializable]
public class ProductionUnlockConditionData {
    public FactorioId id;
    public int requiredCount;
}
