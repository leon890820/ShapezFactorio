using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



public class FactorioPlanet : MonoBehaviour{
    private Material _material;
    private float radius;


    private void Awake() {
        if (!_material) {
            _material = new Material(Shader.Find("Standard"));
            GetComponent<Renderer>().material = _material;
        }
    }

    public void SetRadius(float r) { 
        radius = r;
        transform.localScale = Vector3.one * r;
    }

    public void SetPosition(Vector3 pos) { 
        transform.position = pos;   
    }
}
