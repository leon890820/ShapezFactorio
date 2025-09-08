using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        furnaceUIControl = UIPrefab.GetComponent<FurnaceUIControl>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        UpdateUI();

    }

    public void UpdateUI() {
      
        furnaceUIControl.SetProductImage(productBackpad.Count > 0 ? productBackpad[0].factorioSprite : null, productBackpad.Count);
        furnaceUIControl.SetbackpadImage(backpad.Count > 0 ? backpad[0].factorioSprite : null , backpad.Count);
        furnaceUIControl.SetValue(furnaceCount);

    }

    public override void Run() {
        if (productBackpad.Count >= completeMax) return;
        if (backpad.Count == 0) return;

        FactorioGameObjectBase lastObject = backpad[backpad.Count - 1];

        if (lastObject is IBurnable burnable) {
            furnaceCount += Time.deltaTime * furnaceSpeed;

            if (furnaceCount > 1f) {
                furnaceCount = 0f;
                FactorioPrefabBaseObject productPrefab = burnable.GetBurnProduct();

                GameObject product = Instantiate(productPrefab.object_prefab);
                product.transform.SetParent(transform, false);
                product.transform.localPosition = Vector3.zero;
                FactorioGameObjectBase factorioGameObject = product.GetComponent<FactorioGameObjectBase>();
                factorioGameObject.SetSprite(productPrefab.info);
                productBackpad.Add(factorioGameObject);

                backpad.Remove(lastObject);
                Destroy(lastObject.gameObject);

            }

        }

    }


    public override bool TryInput(FactorioGameObjectBase factorioResource,Vector3Int pos, int i, bool mid = false) {

        if (backpad.Count == 0) { 
            backpad.Add(factorioResource);
            factorioResource.transform.SetParent(transform);
            factorioResource.transform.localPosition = Vector3.zero;
            return true;
        }

        if (backpad.Count >= backpadMax) {
            return false;
        }


        if (backpad[0].GetType() != factorioResource.GetType()) {
            return false;
        } else {
            backpad.Add(factorioResource);
            factorioResource.transform.SetParent(transform);
            return true;
        }
    }

    public override FactorioGameObjectBase TryBeGrab() {

        if (productBackpad.Count == 0) return null;
        var fgo = productBackpad[productBackpad.Count - 1];
        productBackpad.RemoveAt(productBackpad.Count - 1);
        fgo.transform.SetParent(null);
        return fgo;
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("Furnace");
    }




}
