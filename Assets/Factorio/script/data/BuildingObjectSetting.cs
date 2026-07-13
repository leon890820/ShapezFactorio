using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingObjectSetting", menuName = "Factrio/Building Attribute")]
public class FactorioObjectSetting : ScriptableObject {

    public FactorioPrefabObject[] factorioObjects;

}

[System.Serializable]
public class FactorioPrefabObject {
    public string name;
    public GameObject object_prefab;
    public Sprite info;
}