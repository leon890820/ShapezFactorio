using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorioUIManager : MonoBehaviour{
    public FactorioPrimitiveUI normalUI;
    public FactorioPrimitiveUI galaxyUI;


    private void Update() {


        if (CameraControl.galaxy) {
            bool galaxy = CameraControl.galaxyMode;

            normalUI.SetActive(!galaxy);
            galaxyUI.SetActive(galaxy);

            CameraControl.galaxy = false;
        }


    }


}
