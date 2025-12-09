using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerBuilding : FactorioPlatformBuilding{
    HashSet<PowerBuilding> connectPowerBuildings = new();
    protected PowerGrid powerGrid;
    protected PowerGridUIController powerGridUIController;

    [HideInInspector]
    public bool through = false;


    



    public void AddConnectPowerBuilding(PowerBuilding powerBuilding) { 
        connectPowerBuildings.Add(powerBuilding);
    }

    public PowerGrid GetPowerGrid() {
        return powerGrid;
    }

    public void SetPowerGrid(PowerGrid powerGrid) {
        this.powerGrid = powerGrid;
    }

    public IEnumerable<PowerBuilding> GetConnectPowerBuilding() { 
        return connectPowerBuildings;
    }
}
