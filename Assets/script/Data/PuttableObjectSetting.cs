using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PuttableObjectSetting", menuName = "Factrio/Puttable Attribute")]
public class PuttableObjectSetting : ScriptableObject {
  
    public ObjectPack[] objectPack;

}

[System.Serializable]
public class ObjectPack {
    public string name;
    public GameObject object_prefab;
    public Sprite info;
    public Mesh[] mesh;
}


