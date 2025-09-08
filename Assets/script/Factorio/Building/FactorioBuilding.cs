using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactorioBuilding : FactorioGameObjectBase {

    public Transform pivotTransform;

    protected int rotation = 0;
    public Vector3Int buildingSize = new Vector3Int(1, 1 , 1);

    public Color[] tintColor;

    protected Dictionary<Renderer, Material[]> originalMaterials = new();
    protected Renderer[] meshRenderers;
    protected bool bluePrintMode = true;


    public Material rimMaterial;

    protected Camera main_camera;

    // Start is called before the first frame update
    protected override void Awake(){

        base.Awake();

        tintColor = new Color[] { new(0.0f, 1.0f, 0.85f), new(1.0f, 0.16f, 0.0f) };

        var rendererList = new List<Renderer>();
        rendererList.AddRange(GetComponentsInChildren<MeshRenderer>(true));
        rendererList.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>(true));

        meshRenderers = rendererList.ToArray();

       
        foreach (var renderer in meshRenderers) {
            originalMaterials[renderer] = renderer.materials;
        }
        main_camera = GameObject.FindAnyObjectByType<Camera>();

    }

    protected override void Start() { 
        base.Start();
        
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        Run();
    }


    public virtual void Run() { 
        
    
    }

    public abstract bool UpdateAnchor();
    public abstract void UpdateBehavior();
    public abstract List<FactorioBuilding> GetMultiMuilding(List<Vector3> anchor);
    public abstract bool TryPutBuilding();


    public void SetValidColor(int c) {
        rimMaterial.SetColor("_RimColor", tintColor[c]);
    }




    public virtual void SetRimMaterial() {
        foreach (var renderer in meshRenderers) {
            Material[] newMats = new Material[renderer.materials.Length];
            for (int i = 0; i < newMats.Length; i++) {
                newMats[i] = rimMaterial; 
            }
            renderer.materials = newMats;
        }
    }

    public virtual void SetOriginalMaterial() {
        foreach (var renderer in meshRenderers) {
            renderer.materials = originalMaterials[renderer];
        }
    }

    

    public virtual void AddRotation() {
        rotation = (rotation + 1) % 4;
        PlayerControll.rotation = rotation;
        pivotTransform.rotation = Quaternion.Euler(0.0f, rotation * 90.0f, 0.0f);
    }

    public virtual void SetRotation(int i) {
        rotation = i;
        pivotTransform.rotation = Quaternion.Euler(0.0f, rotation * 90.0f, 0.0f);
    }

    public void SetBluePrintMode(bool b) { 
        bluePrintMode = b;
    }


    public virtual void SetPosition(Vector3 pos) {
        pivotTransform.position =  Floor(pos);
    }

    
    


    public static Vector3 Floor(Vector3 v) {
        return new Vector3(
            Mathf.FloorToInt(v.x),
            v.y,
            Mathf.FloorToInt(v.z)
            );
        }

    }
