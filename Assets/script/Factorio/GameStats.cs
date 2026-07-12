using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour{
    public static GameStats Instance { get; private set; }
    public Action OnStatsUpdated;
    private Dictionary<FactorioId, int> stats = new Dictionary<FactorioId, int>();


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void IncrementStat(FactorioId id, int amount = 1) {
        if (id == FactorioId.None) return;

        if (stats.ContainsKey(id)) {
            stats[id] += amount;
        } else {
            stats[id] = amount;
        }
        Debug.Log("IncrementStat");
        OnStatsUpdated?.Invoke();
    }

    public int GetItemAmount(FactorioId id) {
        if (stats.ContainsKey(id)) {
            return stats[id];
        }
        return 0;
    }

}
