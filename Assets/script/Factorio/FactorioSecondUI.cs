using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FactorioSecondUI : MonoBehaviour{
    public SpwanBuildingButton[] spwanBuildingButtons;

    void Start(){
        foreach (var spwanBuildingButton in spwanBuildingButtons) {
            spwanBuildingButton.SetImage();
        }  
    }  
}
