using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorioUIControlBase : MonoBehaviour{
    public Sprite basic;

    public void SetActive(bool active) { 
        gameObject.SetActive(active);   
    }
}
