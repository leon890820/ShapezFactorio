using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IronChestUIControll : FactorioUIControlBase {
    public ClickButton buttonPrefab;
    public GameObject ItemUI;

    private ClickButton[] buttons;
    [SerializeField] private int slotCount = 30;
    private int row = 10;
    private FactorioBackpad chestBackpad;

    public override void InitUI() {
        buttons = new ClickButton[slotCount];
        Vector3 origin = new Vector3(-290, 100, 0);
        for (int i = 0; i < slotCount; i++) {
            int rowIndex = i / row;
            int columnIndex = i % row;
            ClickButton buttonObject = Instantiate(buttonPrefab);
            buttonObject.transform.SetParent(ItemUI.transform, false);
            buttonObject.GetComponent<RectTransform>().localPosition = origin + new Vector3(60 * columnIndex, -60 * rowIndex, 0);
            buttons[i] = buttonObject;
        }
    }

    public override void BindFactorioGameObject(FactorioGameObjectBase factorioGameObjectBase) {
        var ic = factorioGameObjectBase as IronChest;
        chestBackpad = ic.backpad;
    }

    public override void UpdateUI() {
        if (chestBackpad == null || buttons == null) return;

        int count = Mathf.Min(chestBackpad.backpad.Length, buttons.Length);
        for (int i = 0; i < count; i++) {
            if (chestBackpad.IsEmpty(i)) {
                buttons[i].SetImage(basic);
                buttons[i].SetText(String.Empty);
            } else {
                (var factorioObject, int c) = chestBackpad.GetBackpadIndexInfo(i);
                buttons[i].SetImage(factorioObject.factorioSprite);
                buttons[i].SetText(c.ToString());
            }
        }
    
    }
}
