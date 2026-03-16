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
    private FactorioBackpad chestBackpad;

    public override void InitItemUI(FactorioGameObjectBase factorioGameObjectBase) {
        var ic = factorioGameObjectBase as IronChest;
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
        chestBackpad = ic.backpad;
    }

    public override void UpdateUI() {
        for (int i = 0; i < chestBackpad.backpad.Length; i++) {
            if (chestBackpad.IsEmpty(i)) {
                buttons[i].SetImage(basic);
                buttons[i].SetText(String.Empty);
            } else {
                (var factorioObject, int count) = chestBackpad.GetBackpadIndexInfo(i);
                buttons[i].SetImage(factorioObject.factorioSprite);
                buttons[i].SetText(count.ToString());
            }
        }
    
    }
}
