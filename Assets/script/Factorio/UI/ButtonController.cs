using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ButtonController : MonoBehaviour{
    public Button button;
    public Image image;
    public TextMeshProUGUI text;


    public void SetText(string str) {
        text.text = str;
    }

    public void SetImage(Sprite sprite) {
        image.sprite = sprite;
    }

    public void AddListener(Action action) {
        button.onClick.AddListener(() => action());
    }

}
