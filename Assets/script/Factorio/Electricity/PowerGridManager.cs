using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerGridManager : MonoBehaviour{
    public static PowerGridManager Instance { get; private set; }

    public HashSet<PowerGrid> powerGrids = new HashSet<PowerGrid>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SplitePowerGrid(PowerBuilding building) { 
        // TODO
    }

    public void MergePowerGrid(HashSet<PowerGrid> neighborPowerGrids,PowerBuilding building) {
        if (neighborPowerGrids.Count == 0) {
            PowerGrid powerGrid = new PowerGrid(building);
            powerGrids.Add(powerGrid);
        } else if (neighborPowerGrids.Count == 1) {
            PowerGrid first = neighborPowerGrids.First();
            first.AddPowerBuilding(building);
        } else {
            PowerGrid powerGrid = new PowerGrid(building);
            foreach (var power in neighborPowerGrids) {
                powerGrids.Remove(power);
                powerGrid.AddPowerBuilding(power.GetPowerBuilding());
            }
            powerGrids.Add(powerGrid);
        }
    }

    public void MergePowerGrid(HashSet<PowerGrid> neighborPowerGrids) {
        if (neighborPowerGrids.Count <= 1) return;
        PowerGrid powerGrid = new PowerGrid();
        foreach (var power in neighborPowerGrids) {
            powerGrids.Remove(power);
            powerGrid.AddPowerBuilding(power.GetPowerBuilding());
        }
        powerGrids.Add(powerGrid);
    }

    private void OnDrawGizmos() {
        PowerGrid powerGrid = powerGrids.FirstOrDefault();
        if (powerGrid == null) return;
        Queue<PowerBuilding> queue = new Queue<PowerBuilding>();
        powerGrid.SetAllThroughFalse();
        queue.Enqueue(powerGrid.GetFirshPowerBuilding());

        
        while (queue.Count > 0) { 
            PowerBuilding building = queue.Dequeue();
            building.through = true;
            foreach (PowerBuilding nextBuilding in building.GetConnectPowerBuilding()) {
                if (nextBuilding.through) continue;
                queue.Enqueue(nextBuilding);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(building.transform.position + Vector3.up, nextBuilding.transform.position + Vector3.up);
            }
        }
        

    }

}
