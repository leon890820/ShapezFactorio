using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUIControl : FactorioUIControlBase {
    [SerializeField] private FactorioBackpadUIManager backpadUIManager;
    [SerializeField] private FactorioBackpadUIManager productUIManager;

    private FactorioBackpad backpad;
    private FactorioBackpad product;

    public override void InitItemUI(FactorioGameObjectBase factorioGameObjectBase) {
        var furnace = factorioGameObjectBase as Furnace;
        backpad = furnace.backpad;
        product = furnace.productBackpad;
    }

    public override void UpdateUI() {
        var (backpadObj, backpadCount) = backpad.GetBackpadIndexInfo(0);
        var (productObj, productCount) = product.GetBackpadIndexInfo(0);
        backpadUIManager.SetbackpadImage(backpadObj?.factorioSprite ?? basic, backpadCount);
        productUIManager.SetbackpadImage(productObj?.factorioSprite ?? basic, productCount);
    }

}
