using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class Chunk 
{
    private GenerateWorld world;
    private GameObject[,] voxelGameObject = new GameObject[ChunkData.chunkWidth, ChunkData.chunkHeight];
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private int[,] dir = { { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };
    private PuttableObjectSetting puttable_object_setting;
    int triangleIndex = 0;
    List<Vector3> verties = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    Vector3 position;
    public ChunkCoord chunkCoord;

    Transform chunk_object_transform;

    bool generate = false;
    public Chunk(GenerateWorld _world,Vector3 _position) {
        world = _world;
        position = _position;
        if (!generate) Init();
       
       
    }
    public void PutObject(ChunkCoord cc, GameObject go) {
        if (voxelGameObject[cc.z, cc.w]) return;
        voxelGameObject[cc.z, cc.w] = go;
    }

    private void Init() {
        generate = true;
        GameObject gameObject = new GameObject();
        gameObject.transform.position = new Vector3(position.x*ChunkData.voxel_chunk_width,0,position.z*ChunkData.voxel_chunk_height);
        gameObject.transform.SetParent(world.transform);
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;
        puttable_object_setting = world.puttable_object_setting;
        gameObject.name = "Chunk [" + position.x+" , " + position.z + "]";
        chunkCoord = new ChunkCoord(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        chunk_object_transform = gameObject.transform;
        GenerateMap();
        //GenerateTree();
        
    }

    private void GenerateMap() {
        ClaerMeshData();
        for (int i = 0; i < ChunkData.chunkHeight; i += 1) {
            for (int j = 0; j < ChunkData.chunkWidth; j += 1) {
                AddMeshData(new Vector3(j*ChunkData.voxelWidth,0,i*ChunkData.voxelHeight));
               
               
            }
        }

        CreateMesh();
    
    }

    private void  GenerateTree() {
        if (Random.Range(0.0f, 1.0f) > ChunkData.tree_chunk_probability) return;
        Vector3 center = new Vector3(Random.Range(0, ChunkData.chunkWidth), 0, Random.Range(0, ChunkData.chunkHeight));
        Vector3 range = new Vector2(Random.Range(2, 8), Random.Range(2, 8));
        //int type = Random.Range(0, puttable_object_setting.tree.mesh.Length);

        float n = Noise.CalculateNoise(center, new Vector3(0, 0, 0), 0.05f);
        for (int i = Mathf.Max(Mathf.FloorToInt(center.x - range.x), 0); i < Mathf.Min(center.x + range.x,ChunkData.chunkWidth); i += 1) {
            for (int j = Mathf.Max(Mathf.FloorToInt(center.z - range.y), 0); j < Mathf.Min(center.z + range.y, ChunkData.chunkWidth); j += 1) {
                if (Mathf.Abs(Noise.CalculateNoise(new Vector3(i,0,j),new Vector3(0,0,0),0.05f)-n)>0.055f)  continue;
                
                //GameObject tree_pivot = world.CreateTreeObject();
                //GameObject tree = tree_pivot.transform.GetChild(0).gameObject;
               // tree.GetComponent<MeshFilter>().mesh = puttable_object_setting.tree.mesh[type];
                //tree.AddComponent<BoxCollider>();               
                //tree_pivot.transform.SetParent(chunk_object_transform);
                //tree_pivot.transform.localPosition = new Vector3(i, 0, j);
                //tree.transform.localRotation = Quaternion.Euler(-90, 0, 0);

                //tree_pivot.name = "Tree (" + chunkCoord.x + "," + chunkCoord.z + ")" + "[" + i +"," + j +"]"; 
                //voxelGameObject[i, j] = tree;
            }
        }

        
    }

    public void GenerateRock() {
        if (Random.Range(0.0f, 1.0f) > ChunkData.rock_chunk_probability) return;
        //GameObject rock_pivot = world.CreateRockObject();
        //GameObject rock = rock_pivot.transform.GetChild(0).gameObject;
        //rock_pivot.transform.SetParent(chunk_object_transform);
        //rock_pivot.transform.localPosition = new Vector3(0, 0, 0);
        //rock_pivot.name = "Rock (" + chunkCoord.x + "," + chunkCoord.z + ")" + "[" + 0 + "," + 0 + "]";
        //SelectableObject rocksto = rock.GetComponent<SelectableObject>();
        //for (int y = 0; y < rocksto.voxel_side.y; y += 1) {
        //    for (int x = 0; x < rocksto.voxel_side.x; x += 1) {
                
        //        world.AddObjectToChunk(chunk_object_transform.position + new Vector3(x, 0, y), rock_pivot);
       //     }
       // }
    }

    public GameObject GetVoxelGameObject(ChunkCoord cc) { 
        return voxelGameObject[cc.z,cc.w];
    }

   



    private void AddMeshData(Vector3 position) {
        for (int i = 0; i < ChunkData.quad_normals_data.Length; i += 1) { 
            verties.Add(ChunkData.quad_vertices_data[i] + position);
        }
        triangles.Add(triangleIndex + 0);
        triangles.Add(triangleIndex + 1);
        triangles.Add(triangleIndex + 2);
        triangles.Add(triangleIndex + 0);
        triangles.Add(triangleIndex + 2);
        triangles.Add(triangleIndex + 3);
        triangleIndex += 4;

        uv.Add(ChunkData.quad_uvs_data[0]);
        uv.Add(ChunkData.quad_uvs_data[1]);
        uv.Add(ChunkData.quad_uvs_data[2]);
        uv.Add(ChunkData.quad_uvs_data[3]);
       
        

    }

    void ClaerMeshData() {
        triangleIndex = 0;
        verties.Clear();
        triangles.Clear();
        uv.Clear();

    }

    private void CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = verties.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
       
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;

    }





}
