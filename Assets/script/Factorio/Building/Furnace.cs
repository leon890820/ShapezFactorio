using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Furnace : FactorioPlatformBuilding {

    public FactorioBackpad productBackpad;   
    int productMax = 5;

    public float furnaceSpeed = 1f;
    public float furnaceCount = 0f;

    // Start is called before the first frame update
    protected override void Awake(){
        base.Awake();
        backpadMax = 50;
        backpad = new FactorioBackpad(1, backpadMax);
        productBackpad = new FactorioBackpad(1, productMax);
    }

    protected override void Start() {
        base.Start();        
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();        
    }


    public override void Run() {
        if (productBackpad.IsFull()) return;
        if (backpad.IsEmpty()) return;
        if (!backpad.IsSomeType<IBurnable>()) return;
        furnaceCount += Time.deltaTime * furnaceSpeed;
        if (furnaceCount <= 1f) return;
        TryProduct();
    }

    private bool TryProduct() {
        FactorioGameObjectBase lastObject = backpad.Pop();
        IBurnable burnable = lastObject as IBurnable;
        FactorioGameObjectBasePacket productPrefabPacket = burnable.GetBurnProduct();
        FactorioPrefabBaseObject productPrefab = productPrefabPacket.factorioPrefab;
        FactorioGameObjectBase factorioGameObject = Instantiate(productPrefab.object_prefab);
        GameStats.Instance.IncrementStat(factorioGameObject.GetId(), 1);

        factorioGameObject.transform.SetParent(transform);
        factorioGameObject.transform.localPosition = Vector3.zero;
        factorioGameObject.SetSprite(productPrefab.info);
        productBackpad.TryInput(factorioGameObject);
        Destroy(lastObject.gameObject);

        furnaceCount = 0f;
        return true;
    }



    public override void SetStatus() {
        if (backpad.IsEmpty()) {
            buildStatus = BuildStatus.Idle;
            return;
        }
        if (backpad.IsSomeType<IBurnable>()) {
            buildStatus = BuildStatus.Working;
        } else { 
            buildStatus= BuildStatus.FalseInput;
        }
    }


    public override bool TryInput(FactorioGameObjectBase factorioResource,Vector3Int pos, int i, bool mid = false) {

        if (backpad.TryInput(factorioResource)) {
            PutResourceToBackpad(factorioResource);
            return true;
        }

        return false;
        
    }

    void PutResourceToBackpad(FactorioGameObjectBase factorioResource) {
        factorioResource.transform.SetParent(transform);
        factorioResource.transform.localPosition = Vector3.zero;
    }

    public override FactorioGameObjectBase TryBeGrab() {
        var resource = productBackpad.Pop();
        if(!resource) return null;
        resource.transform.SetParent(null);
        return resource;
    }

}
