using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Assembling : PowerCosumeBulding {
    public Animator animator;


    private List<FactorioGameObjectBase> inputBackpad1 = new();
    private List<FactorioGameObjectBase> inputBackpad2 = new();
    private List<FactorioGameObjectBase> productBackpad1 = new();

    private FactorioGameObjectBasePacket productMaterial1;
    private FactorioGameObjectBasePacket productMaterial2;
    private FactorioGameObjectBasePacket product;

    private AssemblingUIControll assemblingUIControl;

    private float assembling_time = 1f;
    private float assembling_speed = 1f;
    private float assembling_count = 0f;


    protected override void Start() {
        base.Start();
        assemblingUIControl = factorioUIControlBase as AssemblingUIControll;
        assemblingUIControl.InitItemUI(this);
        backpadMax = 50;
    }

    protected override void Update() {
        base.Update();
        SetAnimation();
    }


    public override void Run() {
        if (buildStatus != BuildStatus.Working) return;
        assembling_count += Time.deltaTime * assembling_speed;
        if (assembling_count > assembling_time) {
            TryAssembling();
        }
    }

    private void TryAssembling() {
        if (productBackpad1.Count >= backpadMax) return;
        for (int i = 0; i < (productMaterial1?.number ?? 0); i++) {
            FactorioGameObjectBase lastObject = inputBackpad1[^1];
            inputBackpad1.Remove(lastObject);
            Destroy(lastObject);
        }
        for (int i = 0; i < (productMaterial2?.number ?? 0); i++) {
            FactorioGameObjectBase lastObject = inputBackpad2[^1];
            inputBackpad2.Remove(lastObject);
            Destroy(lastObject);
        }

        for (int i = 0; i < product.number; i++) {
            FactorioGameObjectBase factorioGameObject = Instantiate(product.factorioPrefab.object_prefab);
            GameStats.Instance.IncrementStat(factorioGameObject.GetType().Name, 1);
            factorioGameObject.transform.SetParent(transform);
            factorioGameObject.transform.localPosition = Vector3.zero;

            productBackpad1.Add(factorioGameObject);
        }

        assembling_count = 0f;
    }

    public void SetProduct(FactorioGameObjectBasePacket product) {
        IAssembled assembled = product.factorioPrefab.object_prefab.GetComponent<IAssembled>();
        List<FactorioGameObjectBasePacket> materials = assembled.GetItemMaterial();
        productMaterial1 = materials[0];
        productMaterial2 = materials[1];
        this.product = product;
    }

    public void ResetProduct() {
        productMaterial1 = null;
        productMaterial2 = null;
        product = null;
        inputBackpad1.Clear();
        inputBackpad2.Clear();
        productBackpad1.Clear();
    }

    public override void UpdateUI() {
        assemblingUIControl.SetbackpadImage1(productMaterial1?.factorioPrefab.info, inputBackpad1.Count);
        assemblingUIControl.SetbackpadImage2(productMaterial2?.factorioPrefab.info, inputBackpad2.Count);
        assemblingUIControl.SetProductImage(product?.factorioPrefab.info, productBackpad1.Count);

        assemblingUIControl.SetValue(assembling_count);

        if (buildStatus == BuildStatus.NoRecipe) {
            assemblingUIControl.SetWorking(false);
        } else {
            assemblingUIControl.SetWorking(true);
        }

    }

    public void SetAnimation() {
        if (bluePrintMode || buildStatus != BuildStatus.Working) {
            animator.SetBool("Assembling", false);
        } else {
            animator.SetBool("Assembling", true);
        }
    }


    public override BuildStatus EvaluateStatusWithoutPower() {
        if (product == null) {
             return BuildStatus.NoRecipe;
        } else if ((productMaterial1 != null && inputBackpad1.Count < productMaterial1.number) || (productMaterial2 != null && inputBackpad2.Count < productMaterial2.number)) {
             return BuildStatus.NoInput;
        } else if (productBackpad1.Count >= backpadMax) {
            return BuildStatus.OutputFull;
        } 
        return BuildStatus.Working;
    }

    public override float GetCosumePower() {
        return cosumePower;
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("Assembling");
    }



    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }

    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos, int i, bool mid = false) {
        FactorioGameObjectBase material1 = productMaterial1?.factorioPrefab.object_prefab;
        FactorioGameObjectBase material2 = productMaterial2?.factorioPrefab.object_prefab;
        if (material1 != null && material1.GetType() == factorioResource.GetType()) {
            inputBackpad1.Add(factorioResource);
            factorioResource.transform.SetParent(transform);
            factorioResource.transform.localPosition = Vector3.zero;
            return true;
        } else if (material2 != null && material2.GetType() == factorioResource.GetType()) {
            inputBackpad2.Add(factorioResource);
            factorioResource.transform.SetParent(transform);
            factorioResource.transform.localPosition = Vector3.zero;
            return true;
        }

        return false;
    }

    public override FactorioGameObjectBase TryBeGrab() {
        if(productBackpad1.Count <= 0) return null;
        FactorioGameObjectBase lastObject = productBackpad1[^1];
        productBackpad1.Remove(lastObject);
        return lastObject;
    }
}
