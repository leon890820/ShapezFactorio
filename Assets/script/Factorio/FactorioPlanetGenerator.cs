using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorioPlanetGenerator : MonoBehaviour{

    public FactorioPlanet PlanetPrefab;
    public static Dictionary<ChunkCoord, FactorioPlanet> Planets = new();

    public Vector2 planetDensity = new Vector2(5,5);

    private Camera main_cam;
   
    

    // Start is called before the first frame update
    void Start(){
        main_cam = GameObject.FindAnyObjectByType<Camera>();
        CreatePlanets();
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void CreatePlanets() {
        PRNG prng = new PRNG(0);
        int size = 3;
        float galaxyGrid = FactorioData.platformTexelSize * FactorioData.galaxyGridSize;
        
        for (int y = -size; y <= size; y++) {
            for (int x = -size; x <= size; x++) {
                FactorioPlanet planet = Instantiate(PlanetPrefab);
                float radius = 10 + prng.Range(0f, 5f);
                Vector3 position = new Vector3(x, 0, y) * galaxyGrid
                                 + new Vector3(0, -27, 0) + new Vector3(prng.Range(radius, galaxyGrid - radius), 0f, prng.Range(radius, galaxyGrid - radius));
                planet.SetPosition(position);
                planet.SetRadius(radius * 2);
            }
        }
    
    }
}
