using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiningDrill : PowerCosumeBulding {

    public Animator animator1;
    public Animator animator2;

    private float mining_time = 8f;
    private float mining_speed = 1f;
    private float mining_count = 0f;
    private FactorioPrefabBaseObject miningResource;

    protected override void Awake() {
        base.Awake();
        backpadMax = 50;
        backpad = new FactorioBackpad(1, backpadMax);
    }

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();        
    }

    // Update is called once per frame
 
    protected override void Update() {
        base.Update();
        SetAnimation();        
    }

    public ChunkCoord GetChunkCoord() {
        return GalaxyManager.Instance.PositionToChunkCoord(transform.position);
    }

    public void SetResource(FactorioPrefabBaseObject resource) { 
        miningResource = resource;
    }

    public void ResetBuilding() {
        miningResource = null;
        mining_count = 0f;
        backpad.Clear();
    }

    public override void SetStatus() {
        if (miningResource == null) {
            buildStatus = BuildStatus.NoRecipe;
        } else if (!powerGrid.GetAffordPower()) {
            buildStatus = BuildStatus.NoPower;
        } else {
            buildStatus = BuildStatus.Working;
        }
        buildingStatusController?.SetAlertIcon(buildStatus);
    }

    public override BuildStatus EvaluateStatusWithoutPower() {
        if (miningResource == null) {
            return BuildStatus.NoRecipe;
        } 
        return BuildStatus.Working;
        
    }





    public override void Run() {
        if (bluePrintMode) return;

        if (buildStatus != BuildStatus.Working) return;
        TryOutput();

        if (mining_count > mining_time) {            
            TryMining();            
        }


        mining_count += Time.deltaTime * mining_speed;
    }

    public void TryMining() {
        if (backpad.IsFull()) return;

        FactorioGameObjectBase factorioGameObjectBase = Instantiate(miningResource.object_prefab);
        factorioGameObjectBase.transform.SetParent(transform);
        factorioGameObjectBase.transform.localPosition = Vector3.zero;        
        factorioGameObjectBase.SetSprite(miningResource.info);
        GameStats.Instance.IncrementStat(factorioGameObjectBase.GetType().Name, 1);
        backpad.TryInput(factorioGameObjectBase);
        mining_count = 0;
        ResetAnimation();
    }


    public void TryOutput() {
        if (backpad.IsEmpty()) return;
        Vector3Int dir = FactorioData.direction[(rotation + 1) % 4] * 2;
        FactorioPlatformBuilding factorioPlatformBuilding = playGroundPlatform.GetBuilding(this , dir);
        if (!factorioPlatformBuilding) return;

        FactorioGameObjectBase factorioResource = backpad.Peak();
        Vector3Int pos = playGroundPlatform.GetLocalPositions(transform.position) + dir;


        if (factorioPlatformBuilding.TryInput(factorioResource , pos, 0, true)) {
            backpad.Pop();
        }

    }

    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos , int i, bool mid) {
        return false;
    }


    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f , 0.0f, 0.5f);
    }


    public void SetAnimation() {       
        if (bluePrintMode || buildStatus != BuildStatus.Working) {
            animator1.SetBool("Mining", false);
            animator2.SetBool("Mining", false);
        } else {
            animator1.SetBool("Mining", true);
            animator2.SetBool("Mining", true);
        }    
    }

    public void ResetAnimation() {
        animator1.Play("CINEMA_4D_Main", 0, 0f);
        animator1.Update(0f);
        animator2.Play("CINEMA_4D_Main", 0, 0f);
        animator2.Update(0f);
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("MiningDrill");
    }

}
