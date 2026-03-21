using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Assembling : PowerCosumeBulding {
    public Animator animator;

    public FactorioBackpad productBackpad;

    public List<FactorioGameObjectBasePacket> productIngredient;
    public FactorioGameObjectBasePacket product;

    private float assembling_time = 1f;
    private float assembling_speed = 1f;
    private float assembling_count = 0f;

    protected override void Awake() {
        base.Awake();
        backpadMax = 50;
        backpad = new FactorioBackpad(2, backpadMax);
        productBackpad = new FactorioBackpad(1, backpadMax);
    }
    protected override void Start() {
        base.Start();
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
        for (int i = 0; i < productIngredient.Count; i++) {
            for (int count = 0; count < productIngredient[i].number; count++) {
                FactorioGameObjectBase lastObject = backpad.Pop(count);
                Destroy(lastObject.gameObject);
            }
        }

        for (int i = 0; i < product.number; i++) {
            FactorioGameObjectBase factorioGameObject = Instantiate(product.factorioPrefab.object_prefab);
            GameStats.Instance.IncrementStat(factorioGameObject.GetType().Name, 1);
            factorioGameObject.transform.SetParent(transform);
            factorioGameObject.transform.localPosition = Vector3.zero;

            productBackpad.TryInput(factorioGameObject);
        }

        assembling_count = 0f;
    }

    public void SetProduct(FactorioGameObjectBasePacket product) {
        IAssembled assembled = product.factorioPrefab.object_prefab.GetComponent<IAssembled>();
        productIngredient = assembled.GetItemMaterial();
        this.product = product;
    }

    public void ResetProduct() {
        productIngredient = null;
        product = null;
        backpad.Clear();
        productBackpad.Clear();
    }

    public override void UpdateUI() {
        base.UpdateUI();
        factorioUIControlBase.SetValue(assembling_count);
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
        } else if (productBackpad.IsFull()) {
            return BuildStatus.OutputFull;
        } else if (CanAssemble()) {
             return BuildStatus.NoInput;
        }  
        return BuildStatus.Working;
    }

    private bool CanAssemble() {
        for (int i = 0; i < productIngredient.Count; i++) {
            if (productIngredient[i] == null) return false;
            if (backpad.backpad[i].Count < productIngredient[i].number) return false;
        }
        return true;
    }

    public override float GetCosumePower() {
        return cosumePower;
    }

    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }

    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos, int i, bool mid = false) {
        for (int index = 0; index < productIngredient.Count; index++) {
            FactorioGameObjectBase ingredient = productIngredient[i].factorioPrefab.object_prefab;
            if (ingredient.GetType() == factorioResource.GetType()) {
                backpad.TryInput(factorioResource, index);
                factorioResource.transform.SetParent(transform);
                factorioResource.transform.localPosition = Vector3.zero;
                return true;
            }
        }

        return false;
    }

    public override FactorioGameObjectBase TryBeGrab() {
        if(productBackpad.IsEmpty()) return null;
        FactorioGameObjectBase lastObject = productBackpad.Pop();
        lastObject.transform.SetParent(null);
        return lastObject;
    }
}
