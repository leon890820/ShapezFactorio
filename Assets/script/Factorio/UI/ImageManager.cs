using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour{
    public Image image;

    public void SetImageColor(Color color) { 
        image.color = color;
    }

}
