using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorioUIControlBase : MonoBehaviour{
    public Sprite basic;

    protected readonly Dictionary<string, int> productPair = new Dictionary<string, int> {
        { "Gear", 1 }
    };

    public void SetActive(bool active) { 
        gameObject.SetActive(active);   
    }
}
