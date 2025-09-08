using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuttableObjectSetting", menuName = "Factrio/Resource Attribute")]
public class ResourceSetting : ScriptableObject {
    public ResourcePack wood;
    public ResourcePack stone;

}
[System.Serializable]
public class ResourcePack {
    public string name;
    public Sprite info;
}
