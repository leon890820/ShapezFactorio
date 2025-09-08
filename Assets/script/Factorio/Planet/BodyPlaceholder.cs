using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BodyPlaceholder : MonoBehaviour {

    public int terrainResolution = 200;
    public Material material;

    Mesh mesh;


    void Awake() {

        ResetMesh();

    }

    public void ResetMesh() {
        const int vertexLimit16Bit = 1 << 16 - 1; // 65535
        if (mesh == null) {
            mesh = new Mesh();
        } else {
            mesh.Clear();
        }

        if (!material) {
            material = new Material(Shader.Find("Standard"));
        }

        SphereMesh s = new SphereMesh(terrainResolution);

        mesh.indexFormat = (s.Vertices.Length < vertexLimit16Bit) ? UnityEngine.Rendering.IndexFormat.UInt16 : UnityEngine.Rendering.IndexFormat.UInt32;


        mesh.vertices = s.Vertices;
        mesh.triangles = s.Triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        var g = GetOrCreateMeshObject("Mesh", mesh, material);

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
        MeshFilter filter;
        if (!child.TryGetComponent<MeshFilter>(out filter)) {
            filter = child.gameObject.AddComponent<MeshFilter>();
        }
        filter.sharedMesh = mesh;

        MeshRenderer renderer;
        if (!child.TryGetComponent<MeshRenderer>(out renderer)) {
            renderer = child.gameObject.AddComponent<MeshRenderer>();
        }
        renderer.sharedMaterial = material;

        return child.gameObject;
    }


    public int GetVertexCount() {
        if (mesh) { 
            return mesh.vertices.Length;
        }
        return 0;
    }

    public Vector3[] GetMeshVertexData(){
        if (mesh){
            return mesh.vertices;
        }
        return null;
    }


    public void SetVertexData(Vector3[] data) { 
        mesh.vertices = data;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void SetMaterial(Material mat) {
        var child = transform.Find(name);
        if (!child) {
            child = new GameObject(name).transform;
            child.parent = transform;
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            child.gameObject.layer = gameObject.layer;
        }

        material = mat;

        MeshRenderer renderer;
        if (!child.TryGetComponent<MeshRenderer>(out renderer)) {
            renderer = child.gameObject.AddComponent<MeshRenderer>();
        }

        

        renderer.sharedMaterial = mat;

    }

}