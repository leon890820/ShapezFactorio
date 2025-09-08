using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FactorioSecondUI : MonoBehaviour{


    public SpwanBuildingButton[] spwanBuildingButtons;

   

    // Start is called before the first frame update
    void Start(){
        foreach (var spwanBuildingButton in spwanBuildingButtons) {
            spwanBuildingButton.SetImage();
        }  
    }

    



    
}
