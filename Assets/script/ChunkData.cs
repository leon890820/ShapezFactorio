using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ChunkData {
    static public readonly Vector3[] quad_vertices_data = { 
        new Vector3 (0.0f,0.0f,0.0f),
        new Vector3 (0.0f,0.0f,1.0f),
        new Vector3 (1.0f,0.0f,1.0f),
        new Vector3 (1.0f,0.0f,0.0f)
    };

    static public readonly Vector3[] quad_normals_data = { 
        new Vector3 (0.0f,1.0f,0.0f),
        new Vector3 (0.0f,1.0f,0.0f),
        new Vector3 (0.0f,1.0f,0.0f),
        new Vector3 (0.0f,1.0f,0.0f)
    };

    public static readonly Vector2[] quad_uvs_data = {
        new Vector2 (0.0f,0.0f),
        new Vector2 (0.0f,1.0f),
        new Vector2 (1.0f,1.0f),
        new Vector2 (1.0f,0.0f)
    };

    public static int chunkWidth = 32;
    public static int chunkHeight = 32;
    public static float voxelWidth = 1.0f;
    public static float voxelHeight = 1.0f;

    public static float voxel_chunk_width = chunkWidth * voxelWidth;
    public static float voxel_chunk_height = chunkHeight * voxelHeight;

    public static int view_distance = 2;
    public static float tree_chunk_probability = 0.6f;
    public static float tree_spawn_probability = 0.8f;

    public static float rock_chunk_probability = 0.5f;




}

static public class FactorioData {
    public static readonly Vector3Int[] direction = {
        new  ( 1 ,0 , 0),
        new  ( 0 ,0 ,-1),
        new  (-1 ,0 , 0),
        new  ( 0 ,0 , 1)
    };

    public static readonly Vector2Int[] direction2D = {
        new  ( 1 , 0),
        new  ( 0 ,-1),
        new  (-1 , 0),
        new  ( 0 , 1)
    };

    public static readonly int platformTexelSize = 20;
    public static readonly int platformHalfTexelSize = platformTexelSize / 2;

    public static readonly int galaxyGridSize = 10;

    public static readonly FactorioObjectSetting ResourceObject =
        Resources.Load<FactorioObjectSetting>("ResourceObject");

}
