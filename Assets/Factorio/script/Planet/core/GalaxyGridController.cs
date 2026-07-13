using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyGridController : MonoBehaviour{

    public MeshRenderer meshRenderer;
    public Material[] materials;


    public void SetHasPlanet(bool hasPlanet) { 
        if(hasPlanet) meshRenderer.material = materials[1];
        else meshRenderer.material = materials[0];
    }

}
