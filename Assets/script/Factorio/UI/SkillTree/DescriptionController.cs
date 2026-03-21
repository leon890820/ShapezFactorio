using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionController : MonoBehaviour{

    public Image image;
    public TextMeshProUGUI text;

    public void SetSprite(Sprite sprite) { 
        image.sprite = sprite;
    }

    public void SetText(string text) {
        this.text.text = text;
    }

    public void SetPosition(Vector3 pos) {
        transform.localPosition = pos;
    }
    
}
