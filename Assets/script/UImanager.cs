using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UImanager : MonoBehaviour{
    public Image info_image;

    public void SetInfoImage(Sprite sprite) {
        info_image.color = new Color(255, 255, 255, 255);
        info_image.sprite = sprite;
    }

    public void SetTransparent() {
        info_image.color = new Color(255,255,255,0);
    }

}
