using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButton : FactorioButtonBase{

    protected bool active;

    public FactorioSecondUI factorioSecondUI;



    public override void OnClick() {      
        base.OnClick();
        active = !active;
        factorioSecondUI.gameObject.SetActive(active);
    }

    public void SetGameObjectInActive() {
        active = false;
        factorioSecondUI.gameObject.SetActive(active);
    }


}
