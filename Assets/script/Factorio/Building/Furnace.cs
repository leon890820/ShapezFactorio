using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Furnace : FactorioPlatformBuilding {

    FurnaceUIControl furnaceUIControl;
    List<FactorioGameObjectBase> productBackpad = new List<FactorioGameObjectBase>();
    int completeMax = 5;

    public float furnaceSpeed = 1f;
    public float furnaceCount = 0f;

    // Start is called before the first frame update
    protected override void Awake(){
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        backpadMax = 50;
        furnaceUIControl = factorioUIControlBase as FurnaceUIControl;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();        
    }

    public override void UpdateUI() {
      
        furnaceUIControl.SetProductImage(productBackpad.Count > 0 ? productBackpad[0].factorioSprite : null, productBackpad.Count);
        furnaceUIControl.SetbackpadImage(backpad.Count > 0 ? backpad[0].factorioSprite : null , backpad.Count);
        furnaceUIControl.SetValue(furnaceCount);

    }

    public override void Run() {
        if (productBackpad.Count >= completeMax) return;
        if (backpad.Count == 0) return;

        FactorioGameObjectBase lastObject = backpad[^1];

        if (lastObject is IBurnable burnable) {
            furnaceCount += Time.deltaTime * furnaceSpeed;

            if (furnaceCount > 1f) {
                furnaceCount = 0f;
                FactorioGameObjectBasePacket productPrefabPacket = burnable.GetBurnProduct();
                FactorioPrefabBaseObject productPrefab = productPrefabPacket.factorioPrefab;

                FactorioGameObjectBase factorioGameObject = Instantiate(productPrefab.object_prefab);
                factorioGameObject.transform.SetParent(transform);
                factorioGameObject.transform.localPosition = Vector3.zero;
                factorioGameObject.SetSprite(productPrefab.info);
                productBackpad.Add(factorioGameObject);

                backpad.RemoveAt(backpad.Count - 1);
                Destroy(lastObject.gameObject);

            }
        }

    }

    public override void SetStatus() {
        if (backpad.Count == 0) {
            buildStatus = BuildStatus.Idle;
            return;
        }
        FactorioGameObjectBase lastObject = backpad[^1];
        if (lastObject is IBurnable) {
            buildStatus = BuildStatus.Working;
        } else { 
            buildStatus= BuildStatus.FalseInput;
        }
    }


    public override bool TryInput(FactorioGameObjectBase factorioResource,Vector3Int pos, int i, bool mid = false) {

        if (backpad.Count == 0) {
            PutResourceToBackpad(factorioResource);
            return true;
        }

        if (backpad.Count >= backpadMax) {
            return false;
        }

        if (backpad[0].GetType() != factorioResource.GetType()) {
            return false;
        }  
        
        PutResourceToBackpad(factorioResource);
        return true;
        
    }

    void PutResourceToBackpad(FactorioGameObjectBase factorioResource) {
        backpad.Add(factorioResource);
        factorioResource.transform.SetParent(transform);
        factorioResource.transform.localPosition = Vector3.zero;
    }

    public override FactorioGameObjectBase TryBeGrab() {
        if (productBackpad.Count == 0) return null;
        var resource = productBackpad[^1];
        productBackpad.RemoveAt(productBackpad.Count - 1);
        resource.transform.SetParent(null);
        return resource;
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("Furnace");
    }

}
