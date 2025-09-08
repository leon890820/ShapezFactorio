using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorioPrimitiveUI : MonoBehaviour{
    
    public SwitchButton[] switchButtons;



    // Start is called before the first frame update
    void Start() {
        foreach (var switchButton in switchButtons) {
            switchButton.AddAction(SetGameObjectInActive);
        }
    }

    void SetGameObjectInActive() {
        foreach (SwitchButton button in switchButtons) { 
            button.SetGameObjectInActive();
        }
    
    }

    public void SetActive(bool active) { 
        SetGameObjectInActive();
        gameObject.SetActive(active);
    }



}
