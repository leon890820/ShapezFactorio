using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour{
    List<FactorioPlanet> planets;
    public int numberOfPlanets = 1;

    public FactorioPlanet EarthPrefab;

    private void Awake(){
        planets = new List<FactorioPlanet>();

        for (int i = 0; i < numberOfPlanets; i++) {
            planets.Add(CreatePlanet(new Vector3()));
        }

    }



    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    

    FactorioPlanet CreatePlanet(Vector3 pos) {
        FactorioPlanet planet = Instantiate(EarthPrefab, transform);
        planet.transform.position = pos;
        return planet;
    }
}
