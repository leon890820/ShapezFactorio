using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour{
    List<Planet> planets;
    public int numberOfPlanets = 1;

    private void Awake(){
        planets = new List<Planet>();

        for (int i = 0; i < numberOfPlanets; i++) {
            planets.Add(CreatePlanet(new Vector3(),"Planet" + i));
        }

    }



    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    

    Planet CreatePlanet(Vector3 pos,string name) {
        GameObject go = new GameObject(name);
        Planet planet = go.AddComponent<Planet>();
        go.transform.SetParent(transform);
        go.transform.position = pos;
        return planet;
    }
}
