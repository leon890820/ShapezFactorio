using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class FactorioObject {
    protected GenerateWorld world;
    public FactorioObject() {
        world = GameObject.Find("world").GetComponent<GenerateWorld>();
    }

}
