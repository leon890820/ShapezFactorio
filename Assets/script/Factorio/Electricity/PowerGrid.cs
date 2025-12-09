using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerGrid{
    private HashSet<PowerStation> powerStations = new();
    private HashSet<TeleGraphPole> teleGraphPoles = new();
    private HashSet<PowerCosumeBulding> powerCosumeBulding = new();
    public PowerGrid() { 
    
    }

    public PowerGrid(PowerBuilding powerBuilding) {
        AddPowerBuilding(powerBuilding);
    }

    public PowerGrid(HashSet<PowerBuilding> powerBuilding) {
        AddPowerBuilding(powerBuilding);
    }

    public void AddPowerBuilding(HashSet<PowerBuilding> powerBuilding) {
        foreach (PowerBuilding building in powerBuilding) {
            AddPowerBuilding(building);
        }
    }

    public void AddPowerBuilding(PowerBuilding powerBuilding) {
        switch (powerBuilding) {
            case PowerStation station:
                powerStations.Add(station);
                station.SetPowerGrid(this);
                break;
            case TeleGraphPole pole:
                teleGraphPoles.Add(pole);
                pole.SetPowerGrid(this);
                break;
            case PowerCosumeBulding cosumeBulding:
                powerCosumeBulding.Add(cosumeBulding);
                cosumeBulding.SetPowerGrid(this);
                break;
            default:
                Debug.LogWarning($"未知的 PowerBuilding 類型：{powerBuilding.GetType()}");
                break;
        }
    }

    public PowerBuilding GetFirshPowerBuilding() {
        return powerStations.FirstOrDefault();
    }

    public HashSet<PowerBuilding> GetPowerBuilding() {
        HashSet<PowerBuilding> result = new();
        foreach (var s in powerStations)
            result.Add(s);
        foreach (var p in teleGraphPoles)
            result.Add(p);
        foreach (var c in powerCosumeBulding)
            result.Add(c);
        return result;
    }

    public void SetAllThroughFalse() {
        foreach (var station in powerStations)
            station.through = false;
        foreach (var pole in teleGraphPoles)
            pole.through = false;
        foreach (var cosume in powerCosumeBulding)
            cosume.through = false;
    }


    public int GetPowerCapacity() {
        int count = 0;
        foreach (var building in powerStations) { 
            count += building.capacity;
        }
        return count;
    }

}

