using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerGridUIController : FactorioUIControlBase{
    public TextMeshProUGUI totalPowerText;

    public void SetTotalPoewrText(float power) { 
        totalPowerText.text = "Total Power : " + power.ToString();
    }

}
