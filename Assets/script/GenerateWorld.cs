using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GenerateWorld : MonoBehaviour
{
    public Material material;
    public PuttableObjectSetting puttable_object_setting;
    public ResourceSetting resource_setting;
    List<Chunk> chunks;
    private int[,] dir = { { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };
    // Start is called before the first frame update
    void Start() {
        chunks = new List<Chunk>();
        //CreateInitScene();
        ChunkCoord now_chunkcoord = new ChunkCoord(new Vector3(0,0,0));
        UpdateChunk(now_chunkcoord);

    }



    //public GameObject CreateTreeObject() {
        //return Instantiate(puttable_object_setting.tree.object_prefab, new Vector3(0, 0, 0), Quaternion.identity);
    //}
    //public GameObject CreateRockObject() {
        //return Instantiate(puttable_object_setting.rock.object_prefab, new Vector3(0, 0, 0), Quaternion.identity);
    //}

    public void UpdateChunk(ChunkCoord cc) {
        for (int i = -ChunkData.view_distance; i <= ChunkData.view_distance; i += 1) {
            for (int j = -ChunkData.view_distance; j <= ChunkData.view_distance; j += 1) {
                ChunkCoord ccd = new(cc.x + j, cc.y + i);
                bool flag = false;
                for (int c = 0; c < chunks.Count; c += 1) {
                    if (ccd.Equals(chunks[c].chunkCoord)) {
                        flag = true;
                        break;
                    }
                }
                if (flag) continue;
                chunks.Add(new Chunk(this, new Vector3(ccd.x, 0, ccd.y)));
            }
        }
    }


    private void CreateInitScene() {
        GameObject house = new GameObject();
        house.name = "House";
        //GameObject h = Instantiate(puttable_object_setting.house.object_prefab, new Vector3(-1, 0, -1), Quaternion.Euler(0,0,0));
        //h.transform.parent = house.transform;
        GameObject road = new GameObject();
        road.name = "Road";
        for (int x = -4; x < 4; x += 1) {
            Vector3 p = new Vector3(x, 0, -2);
            //GameObject go = Instantiate(puttable_object_setting.road.object_prefab, p , Quaternion.Euler(0,0,0));
            ChunkCoord c = new(p);
            //go.GetComponentInChildren<PuttableObject>().ChangeTypeAndRotation(0, 1);
            //go.transform.parent = road.transform;
            //go.name = "Road : [" + c.x + "," + c.y + "] (" + c.z + "," + c.w + ")";
            //AddObjectToChunk(p,go);

        }
        GameObject worker = new GameObject();
        worker.name = "Worker";
        for (int i = 0; i < 4; i += 1) {
            //GameObject go = Instantiate(puttable_object_setting.people.object_prefab, new Vector3(UnityEngine.Random.Range(-5.0f,5.0f), 0, -2+ UnityEngine.Random.Range(0.0f,2.0f)), Quaternion.Euler(0, 180, 0));
            //go.transform.parent = worker.transform;
        }


    }

    // Update is called once per frame
    void Update() {
        

    }


    public void AddObjectToChunk(Vector3 p,GameObject go) { 

        ChunkCoord cc = new(p);
        Chunk c = FindChunk(cc);
        if (c!=null) c.PutObject(cc, go);
    }

    public bool HasObject(Vector3 p) {
        ChunkCoord cc = new(p);       
        Chunk c = FindChunk(cc);       
        if(c.GetVoxelGameObject(cc)) return true;
        return false;
    }

    public Chunk FindChunk(ChunkCoord cc) {
        for (int i = 0; i < chunks.Count; i += 1) {
            if (chunks[i].chunkCoord.Equals(cc)) return chunks[i];
        }
        return null;
    }

    public GameObject GetObjectFromChunk(ChunkCoord cc) {
        Chunk c = FindChunk(cc);
        if (c != null) return c.GetVoxelGameObject(cc);
        return null;
    }

    public GameObject[] GetSurroundObject(ChunkCoord center) {
        GameObject[] result = new GameObject[4];      
        for (int i = 0; i < result.Length; i += 1) {
            ChunkCoord cc = new(0, 0, dir[i, 0], dir[i, 1]);
            result[i] = GetObjectFromChunk(center + cc);
        }
        return result;
    }
}

public class ChunkCoord {
    public int x;
    public int y;
    public int z;
    public int w;

    public ChunkCoord() {
        x = 0;
        y = 0;
        z = 0;
        w = 0;

    }

    public ChunkCoord(int _x, int _y) {
        x = _x;
        y = _y;
    }

    public ChunkCoord(int _x, int _y,int _z,int _w) {
        x = _x;
        y = _y;
        z = _z;
        w = _w;

    }
    public ChunkCoord(Vector3 pos) {
        
        x = Mathf.FloorToInt(pos.x / ChunkData.chunkWidth);
        y = Mathf.FloorToInt(pos.z/ ChunkData.chunkHeight);
        z = Mathf.FloorToInt(pos.x % ChunkData.chunkWidth);
        w = Mathf.FloorToInt(pos.z % ChunkData.chunkHeight);
        if (z < 0) z += ChunkData.chunkWidth;
        if (w < 0) w += ChunkData.chunkHeight;

    }

    

    public static ChunkCoord Vector3ToChunkCoord(Vector3 pos) {
        ChunkCoord r = new() {
            x = Mathf.FloorToInt(pos.x / ChunkData.chunkWidth),
            y = Mathf.FloorToInt(pos.z / ChunkData.chunkHeight),
            z = Mathf.FloorToInt(pos.x % ChunkData.chunkWidth),
            w = Mathf.FloorToInt(pos.z % ChunkData.chunkHeight)
        };
        return r;
    }

    override public string ToString() {
        return "Chunk : " + x + " , " + y +" , "+ z + " , " + w;
    }


    public override bool Equals(object obj) {
        return obj is ChunkCoord other && this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode() {
        return HashCode.Combine(x, y); // .NET Core / C# 8+
    }


    public static ChunkCoord operator +(ChunkCoord a, ChunkCoord b) {
        ChunkCoord c = new();
        c.x = a.x + b.x;
        c.y = a.y + b.y;
        c.z = a.z + b.z;
        c.w = a.w + b.w;
        int cx = Mathf.FloorToInt((float)c.z / (float)ChunkData.chunkWidth);
        int cy = Mathf.FloorToInt((float)c.w / (float)ChunkData.chunkHeight);
        c.x += cx; c.y += cy;
        c.z = Mathf.FloorToInt(c.z % ChunkData.chunkWidth);
        c.w = Mathf.FloorToInt(c.w % ChunkData.chunkHeight);
        if (c.z < 0) c.z += ChunkData.chunkWidth;
        if (c.w < 0) c.w += ChunkData.chunkHeight;
        return c;
    
    }


}
