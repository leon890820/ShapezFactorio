using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactorioPlanetShading : MonoBehaviour {
    public Material material;
    private BodyPlaceholder bodyPlaceholder;

    void Awake() {
        bodyPlaceholder = GetComponent<BodyPlaceholder>();
        bodyPlaceholder.SetMaterial(material);
    }

    abstract public void CalcShading();

    
}
