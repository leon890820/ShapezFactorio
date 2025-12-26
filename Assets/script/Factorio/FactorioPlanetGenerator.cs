using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FactorioPlanetGenerator : MonoBehaviour {

    public static Dictionary<ChunkCoord, FactorioPlanet> Planets = new();
    public FactorioPlanet EarthPrefab;

        // Start is called before the first frame update
    void Start() {
        FactorioPlanet earth = Instantiate(EarthPrefab);
        earth.transform.parent = transform;
        earth.transform.position = new Vector3(0, -300f, 0);
        GalaxyManager.Instance.AddPlanet(earth);
    }
      
}

