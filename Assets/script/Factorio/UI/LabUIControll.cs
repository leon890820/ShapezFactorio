using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabUIControll : FactorioUIControlBase{
    public FactorioBackpadUIManager[] backpadsUI;

    private Lab lab;
    private FactorioBackpad backpad;

    public override void InitItemUI(FactorioGameObjectBase factorioGameObjectBase) { 
        lab = factorioGameObjectBase as Lab;  
        backpad = lab.backpad;
    }

    public void SetbackpadImage(Sprite sprite, int number, int index) {
        backpadsUI[index].SetbackpadImage(sprite ?? basic, number);
    }

    public override void UpdateUI() {
        for (int i = 0; i < backpad.Count(); i++) {
            var (backpadObj, backpadCount) = backpad.GetBackpadIndexInfo(i);
            Sprite sprite = backpadCount > 0 ? backpadObj.factorioSprite : null;

            SetbackpadImage(sprite, backpadCount, i);
        }
    }

}
