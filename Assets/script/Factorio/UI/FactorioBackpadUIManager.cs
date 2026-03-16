using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactorioBackpadUIManager : MonoBehaviour{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI backpadText;

    public void SetbackpadImage(Sprite sprite, int number) {
        image.sprite = sprite;
        image.color = number > 0 ? Color.white : sprite ? Color.gray : new Color(1, 1, 1, 0);
        backpadText.text = number.ToString();
        backpadText.gameObject.SetActive(number > 0);
    }

}
