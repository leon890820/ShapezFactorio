using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour{
    public static GameStats Instance { get; private set; }

    private Dictionary<string, int> stats = new Dictionary<string, int>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void IncrementStat(string statName, int amount = 1) {
        if (stats.ContainsKey(statName)) {
            stats[statName] += amount;
        } else {
            stats[statName] = amount;
        }
    }


}
