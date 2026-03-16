using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabUIControll : FactorioUIControlBase{
    public FactorioBackpadUIManager[] backpadsUI;
    public FactorioBackpadUIManager product;

    private Lab lab;

    public override void InitItemUI(FactorioGameObjectBase factorioGameObjectBase) { 
        lab = factorioGameObjectBase as Lab;     
    }

    public void SetbackpadImage(Sprite sprite, int number, int index) {
        backpadsUI[index].SetbackpadImage(sprite ?? basic, number);
    }

    public void UpdateUI(List<FactorioGameObjectBase>[] backpad) {
        for (int i = 0; i < backpad.Length; i++) {
            var list = backpad[i];
            int number = list?.Count ?? 0;
            Sprite sprite = number > 0 ? list[0].factorioSprite : null;

            SetbackpadImage(sprite, number, i);
        }
    }
}
