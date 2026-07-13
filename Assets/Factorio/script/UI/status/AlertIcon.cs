using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlertIcon", menuName = "Factorio/AlertIcon")]
public class AlertIcon : ScriptableObject {
    public ItemData[] items;
}

[System.Serializable]
public class ItemData {
    public string name;
    public Sprite sprite;
}
