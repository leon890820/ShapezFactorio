using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class FactorioPlanet : MonoBehaviour{


    private BodyPlaceholder bodyPlaceholder;
    private bool settingsChanged;

    public FactorioPlanetShading shading;
    public FactorioPlanetShape shape;
    public float radius = 200f;

    public List<string> minableResource; 

    // === 初始化 ===
    protected virtual void Awake() {
        bodyPlaceholder = GetComponent<BodyPlaceholder>();
        transform.localScale = Vector3.one * radius;
    }

    void Start() {
        GenerateTerrain();
    }

    void Update() {
        if (settingsChanged) {
            settingsChanged = false;
            GenerateTerrain();
        }
    }

    void OnValidate() {
        settingsChanged = true;
    }

    // === 主要地形生成流程 ===
    void GenerateTerrain() {
        bodyPlaceholder.ResetMesh();       
        shape.CalcHeight();
        shape.Release(); 
    }



}