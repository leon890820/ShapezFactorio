using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactorioBuilding : FactorioGameObjectBase {

    public Transform pivotTransform;    
    public Vector3Int buildingSize = new Vector3Int(1, 1, 1);
    public Material rimMaterial;


    protected Dictionary<Renderer, Material[]> originalMaterials = new();
    protected Renderer[] meshRenderers;
    protected bool bluePrintMode = true;
    protected int rotation = 0;

    

    protected Camera main_camera;
    protected BuildStatus buildStatus;

    private Color[] tintColor;

    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();      
        InitOrigonalRendererList();
        tintColor = new Color[] { new(0.0f, 1.0f, 0.85f), new(1.0f, 0.16f, 0.0f) };
        main_camera = FindAnyObjectByType<Camera>();
    }

    private void InitOrigonalRendererList() {
        var rendererList = new List<Renderer>();
        rendererList.AddRange(GetComponentsInChildren<MeshRenderer>(true));
        rendererList.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>(true));

        meshRenderers = rendererList.ToArray();
        foreach (var renderer in meshRenderers) {
            originalMaterials[renderer] = renderer.materials;
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        SetStatus();
        Run();        
    }


    public virtual void Run() {}

    public abstract bool UpdateAnchor();
    public abstract void UpdateBehavior();
    public abstract List<FactorioBuilding> GetMultiMuilding(List<Vector3> anchor);
    public abstract bool TryPutBuilding();

    public virtual void InitBuilding() { }

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
        pivotTransform.position = Floor(pos);
    }

    
    public virtual void SetStatus() { 
        
    }



    public static Vector3 Floor(Vector3 v) {
        return new Vector3(
            Mathf.FloorToInt(v.x),
            v.y,
            Mathf.FloorToInt(v.z)
            );
    }

}

public enum BuildStatus {
    None,             // 初始狀態 / 未定義
    Idle,             // 閒置中 (有電，有設定，但沒事做)
    Working,          // 正常運作中 (Producing/Operating)
    Paused,           // 玩家手動暫停
    NoPower,          // 沒有電力 (PowerOff)
    NoRecipe,         // 沒有設定配方 (NotConfigured)
    NoInput,          // 缺少原料 (Starved)
    OutputFull,       // 背包/輸出滿了 (Blocked/Jammed)
    FalseInput,        // 錯誤輸入
    NoSelected
}
