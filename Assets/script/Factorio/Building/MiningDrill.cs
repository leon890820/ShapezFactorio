using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningDrill : FactorioPlatformBuilding {

    public Animator animator1;
    public Animator animator2;

    float mining_time = 8f;
    float mining_speed = 1f;
    float mining_count = 0f;


    protected override void Awake() {
        base.Awake();
        
    }

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        backpadMax = 50;
       
    }

    // Update is called once per frame
 
    protected override void Update() {
        base.Update();
        SetAnimation();
        
    }

    public override void Run() {
        if (bluePrintMode) return;

        TryOutput();

        if (mining_count > mining_time) {
            mining_count = 0;
            TryMining();
            ResetAnimation();
        }


        mining_count += Time.deltaTime * mining_speed;
    }

    public void TryMining() {
        if (backpad.Count >= backpadMax) return;

        FactorioPrefabBaseObject prefab =  PrefabManager.Instance.GetPrefab("IronOre");
        GameObject go = Instantiate(prefab.object_prefab);

        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        FactorioGameObjectBase factorioGameObjectBase = go.GetComponent<FactorioGameObjectBase>();
        factorioGameObjectBase.SetSprite(prefab.info);
        backpad.Add(factorioGameObjectBase);
    
    }


    public void TryOutput() {
        if (backpad.Count <= 0) return;
        Vector3Int dir = FactorioData.direction[(rotation + 1) % 4] * 2;
        FactorioPlatformBuilding factorioPlatformBuilding = playGroundPlatform.GetBuilding(this , dir);

        //Debug.Log(factorioPlatformBuilding + " " + FactorioData.direction[(rotation + 1) % 4] * 2);
        if (!factorioPlatformBuilding) return;

        FactorioGameObjectBase factorioResource = backpad[backpad.Count - 1];
        Vector3Int pos = playGroundPlatform.GetLocalPositions(transform.position) + dir;


        if (factorioPlatformBuilding.TryInput(factorioResource , pos, 0, true)) {
            backpad.Remove(factorioResource);
        }

    }


    //
    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos , int i, bool mid) {
        return false;
    }


    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f , 0.0f, 0.5f);
    }


    public void SetAnimation() {
        if (bluePrintMode) {
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
