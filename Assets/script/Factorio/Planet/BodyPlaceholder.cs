using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class BodyPlaceholder : MonoBehaviour {

    int terrainResolution = 200;
    Material material;
    GameObject body;
    Mesh mesh;


    void Awake() {
        ResetMesh();
    }

    public void ResetMesh() {
        const int vertexLimit16Bit = 1 << 16 - 1; // 65535
        if (mesh == null) mesh = new Mesh();
        else mesh.Clear();
        
        if (!material) material = new Material(Shader.Find("Standard"));
        
        SphereMesh s = new SphereMesh(terrainResolution);

        mesh.indexFormat = (s.Vertices.Length < vertexLimit16Bit) ? UnityEngine.Rendering.IndexFormat.UInt16 : UnityEngine.Rendering.IndexFormat.UInt32;
        SetVertexData(s.Vertices, s.Triangles);

        body = GetOrCreateMeshObject("Mesh", mesh, material);

    }



    GameObject GetOrCreateMeshObject(string name, Mesh mesh, Material material) {
        // Find/create object
        var child = transform.Find(name);
        if (!child) {
            child = new GameObject(name).transform;
            child.parent = transform;
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            child.gameObject.layer = gameObject.layer;
        }

        // Add mesh components

        if (!child.TryGetComponent<MeshFilter>(out MeshFilter filter)) {
            filter = child.gameObject.AddComponent<MeshFilter>();
        }
        filter.sharedMesh = mesh;

        if (!child.TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) {
            renderer = child.gameObject.AddComponent<MeshRenderer>();
        }
        renderer.sharedMaterial = material;

        return child.gameObject;
    }


    public int GetVertexCount() {
        return mesh ? mesh.vertices.Length : 0;
    }

    public Vector3[] GetMeshVertexData(){
        return mesh ? mesh.vertices : null;
    }


    public void SetVertexData(Vector3[] data) { 
        mesh.vertices = data;
        MeshRecalculate();
    }

    public void SetVertexData(Vector3[] vertData, int[] triData) {
        mesh.vertices = vertData;
        mesh.triangles = triData;       
        MeshRecalculate();
    }

    public void MeshRecalculate() {
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void SetMaterial(Material mat) {       
        material = mat;
        if (!body.TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) {
            renderer = body.gameObject.AddComponent<MeshRenderer>();
        }
        renderer.sharedMaterial = mat;
    }
}