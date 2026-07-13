using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactorioPlanetShape : MonoBehaviour{
    public ComputeShader terrainCompute;

    abstract public void CalcHeight();
    abstract public void Release();
}
