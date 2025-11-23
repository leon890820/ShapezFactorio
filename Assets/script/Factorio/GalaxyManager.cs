using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class GalaxyManager : MonoBehaviour{


    public static Dictionary<ChunkCoord, PlayGroundPlatform> playgrounds;
    public static Dictionary<ChunkCoord, FactorioPlanet> planets;

    // Start is called before the first frame update
    void Awake(){
        playgrounds = new Dictionary<ChunkCoord, PlayGroundPlatform>();
        planets = new Dictionary<ChunkCoord, FactorioPlanet>();
    }

    // Update is called once per frame
    void Update(){
        
    }

    public static FactorioPlanet GetFactorioPlanet(ChunkCoord cc) {
        if (planets.TryGetValue(cc, out FactorioPlanet planet)) {
            return planet;
        }
        return null;
    }


    public static ChunkCoord PositionToChunkCoord(Vector3 pos) {
        int texelSize = FactorioData.platformTexelSize;
        int gridX = Mathf.FloorToInt(pos.x / texelSize);
        int gridZ = Mathf.FloorToInt(pos.z / texelSize);

        return new(gridX, gridZ);
    }


    public static bool AddPlayground(PlayGroundPlatform pgp) {
        if(IsValid(pgp)) return false;
 
        ChunkCoord[] chunkCoord = GetPlatFormCoordPositions(pgp);

        foreach (ChunkCoord cc in chunkCoord) {
            playgrounds[cc] = pgp;
        }

        return true;

    }

    public static bool IsValid(PlayGroundPlatform pgp) {
        ChunkCoord[] chunkCoord = GetPlatFormCoordPositions(pgp);

        foreach (ChunkCoord cc in chunkCoord) {
            if (playgrounds.ContainsKey(cc)) return true;
        }
        return false;

    }

    public static ChunkCoord[] GetPlatFormCoordPositions(PlayGroundPlatform pgp) {
        
        Vector2Int platformSize = pgp.platformSize;
        ChunkCoord[] result = new ChunkCoord[platformSize.x * platformSize.y];
        ChunkCoord oc = PositionToChunkCoord(pgp.transform.position);
        Vector2Int offset = platformSize / 2;

        int originX = oc.x - offset.x;
        int originY = oc.y - offset.y;

        for (int y = 0; y < platformSize.y; y++) {
            for (int x = 0; x < platformSize.x; x++) {
                result[y * platformSize.x + x] = new ChunkCoord(originX + x, originY + y);
            }
        }

        return result;
    }

    public static ChunkCoord GetPlatFormCoordPosition(PlayGroundPlatform pgp) {

        Vector2Int platformSize = pgp.platformSize;
        ChunkCoord oc = PositionToChunkCoord(pgp.transform.position);
        Vector2Int offset = platformSize / 2;

        int originX = oc.x - offset.x;
        int originY = oc.y - offset.y;

        return new ChunkCoord(originX , originY);
    }

    public static PlayGroundPlatform GetNeiborPlayGroundPlatform(PlayGroundPlatform pgp, int rot, int num) {
        ChunkCoord cp = GetPlatFormCoordPosition(pgp);
        Vector2Int dir = FactorioData.direction2D[rot] + num * (rot % 2 == 0 ? Vector2Int.up : Vector2Int.right)
                                                       + new Vector2Int(rot == 0 ? pgp.platformSize.x - 1 : 0, rot == 3 ? pgp.platformSize.y - 1 : 0);
        ChunkCoord nc = new ChunkCoord(dir.x + cp.x, dir.y + cp.y);
        return playgrounds.GetValueOrDefault(nc);
    }

    public void SetGroundPlatformLlayer(int n) {
       
        foreach (var value in playgrounds.Values) { 
            value.SetLayer(n);
        }
    }

    public static void AddPlanet(FactorioPlanet factorioPlanet) {
        Vector3 radius = Vector3.one * factorioPlanet.radius;
        Vector3 center = factorioPlanet.transform.position;
        Vector3 minPos = center - radius;
        Vector3 maxPos = center + radius;

        for (int x = (int)minPos.x; x <= maxPos.x; x += 20) {
            for (int z = (int)minPos.x; z <= maxPos.z; z += 20) {
                ChunkCoord cc = new ChunkCoord(x / 20, z / 20);
                if (x * x + z * z > factorioPlanet.radius * factorioPlanet.radius) continue;
                planets[cc] = factorioPlanet;
            }
        }
    }

}


