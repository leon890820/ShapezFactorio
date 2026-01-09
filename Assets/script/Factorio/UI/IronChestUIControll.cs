using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IronChestUIControll : FactorioUIControlBase {
    public ButtonController buttonPrefab;
    public GameObject ItemUI;

    private ButtonController[] buttons;
    private int row = 10;

    public void InitItemUI(IronChest ic) {
        buttons = new ButtonController[ic.chestSize];
        Vector3 origin = new Vector3(-290, 100, 0);
        for (int i = 0; i < ic.chestSize; i++) {
            int rowIndex = i / row;
            int columnIndex = i % row;
            ButtonController buttonObject = Instantiate(buttonPrefab);
            buttonObject.transform.SetParent(ItemUI.transform);
            buttonObject.GetComponent<RectTransform>().localPosition = origin + new Vector3(60 * columnIndex, -60 * rowIndex, 0);
            buttons[i] = buttonObject;
        }
    }

    public void UpdateUI(List<FactorioGameObjectBase>[] chestBackpad) {
        for (int i = 0; i < chestBackpad.Length; i++) {
            if (chestBackpad[i].Count <= 0) {
                buttons[i].SetImage(basic);
                buttons[i].SetText(String.Empty);
            } else {
                buttons[i].SetImage(chestBackpad[i][0].factorioSprite);
                buttons[i].SetText(chestBackpad[i].Count.ToString());
            }
        }
    
    }



    public void Close() {
        this.gameObject.SetActive(false);
    }
}
