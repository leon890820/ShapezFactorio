using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssemblingUIControll : FactorioUIControlBase {
    public FactorioBackpadUIManager[] backpadUIManager;
    public FactorioBackpadUIManager productBackpadUIManager;

    public GameObject workingUI;
    public GameObject itemUI;

    public GameObject buttonPrefab;

    Assembling assembling;

    public override void InitItemUI(FactorioGameObjectBase factorioGameObjectBase) {
        assembling = factorioGameObjectBase as Assembling;
        Vector3 origin = new Vector3(-350, 80, 0);

        int index = 0;
        foreach (var pair in productPair) {
            GameObject buttonObject = Instantiate(buttonPrefab);
            buttonObject.transform.SetParent(itemUI.transform, false);
            buttonObject.GetComponent<RectTransform>().localPosition = origin + new Vector3(60 * index, 0, 0);

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = PrefabManager.Instance.GetSprite(pair.Key);

            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(() => SetAssemblingProduct(pair.Key, pair.Value));
            

            index++;
        }


    }

    public override void UpdateUI() {
        for (int i = 0; i < backpadUIManager.Length; i++) {
            var ingredient = i < assembling.productIngredient?.Count ? assembling.productIngredient[i]: null;
            backpadUIManager[i].SetbackpadImage(ingredient?.GetSprite() ?? basic, assembling.backpad.GetBackpadCount(i));
        }
        productBackpadUIManager.SetbackpadImage(assembling.product?.GetSprite() ?? basic, assembling.productBackpad.GetBackpadCount(0));
       
        if (assembling.buildStatus == BuildStatus.NoRecipe) {
            SetWorking(false);
        } else {
            SetWorking(true);
        }
    }

    private void SetAssemblingProduct(string product, int number) {
        FactorioPrefabBaseObject fpbo = PrefabManager.Instance.GetPrefab(product);
        assembling.SetProduct(new FactorioGameObjectBasePacket(fpbo, number));
    }


    public void SetWorking(bool work) {
        workingUI.SetActive(work);
        itemUI.SetActive(!work);
    }

    public void SetItem() {
        SetWorking(false);
        assembling.ResetProduct();
    }

    
}
