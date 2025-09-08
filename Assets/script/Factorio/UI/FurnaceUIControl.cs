using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUIControl : FactorioUIControlBase {
    public Image backpadImage;
    public Image productImage;

    public TextMeshProUGUI backpadText;
    public TextMeshProUGUI productText;

    public Slider progress;

    

    public void SetValue(float value) { 
        progress.value = value;
    }

    public void SetProductImage(Sprite sprite, int number) {
        productImage.sprite = sprite ?? basic;
        productText.text = number.ToString();
        productText.gameObject.SetActive(number > 0);
    }

    public void SetbackpadImage(Sprite sprite, int number) {
        backpadImage.sprite = sprite ?? basic;
        backpadText.text = number.ToString();
        backpadText.gameObject.SetActive(number > 0);
    }

}
