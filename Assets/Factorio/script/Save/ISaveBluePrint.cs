using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveBluePrint {
    void SaveToBlueprint(string path);
    BlueprintData GetBlueprintData();
    FactorioBuilding LoadBlueprint(BlueprintData data);
}
    

[System.Serializable]
public class BlueprintData {
    public FactorioId id;
    public string name;
    public int x;
    public int y;
    public int z;
    public int rotation;
    public string extraJson;

    public FactorioId GetId() {
        if (id != FactorioId.None) return id;
        return System.Enum.TryParse(name, out FactorioId parsedId) ? parsedId : FactorioId.None;
    }
}

