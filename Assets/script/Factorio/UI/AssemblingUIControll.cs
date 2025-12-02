using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssemblingUIControll : FactorioUIControlBase {
    public Image backpadImage;
    public TextMeshProUGUI backpadText;
    public Image backpad2Image;
    public TextMeshProUGUI backpad2Text;
                                
    public Image productImage;
    public TextMeshProUGUI productText;

    public Slider progress;

    public GameObject workingUI;
    public GameObject ItemUI;

    public GameObject buttonPrefab;

    Assembling assembling;

    public void InitItemUI(Assembling ab) {
        assembling = ab;
        Vector3 origin = new Vector3(-350, 80, 0);

        int index = 0;
        foreach (var pair in productPair) {
            GameObject buttonObject = Instantiate(buttonPrefab);
            buttonObject.transform.SetParent(ItemUI.transform, false);
            buttonObject.GetComponent<RectTransform>().localPosition = origin + new Vector3(60 * index, 0, 0);

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = PrefabManager.Instance.GetSprite(pair.Key);

            Button button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(() => SetAssemblingProduct(pair.Key, pair.Value));
            

            index++;
        }


    }

    public void SetValue(float value) {
        progress.value = value;
    }

    private void SetAssemblingProduct(string product, int number) {
        FactorioPrefabBaseObject fpbo = PrefabManager.Instance.GetPrefab(product);
        assembling.SetProduct(new FactorioGameObjectBasePacket(fpbo, number));
    }

    public void SetbackpadImage1(Sprite sprite, int number) {
        backpadImage.sprite = sprite ?? basic;
        backpadImage.color = number > 0 ? Color.white : sprite ? Color.gray : new Color(1, 1, 1, 0);
        backpadText.text = number.ToString();
        backpadText.gameObject.SetActive(number > 0);
    }

    public void SetbackpadImage2(Sprite sprite, int number) {
        backpad2Image.sprite = sprite ?? basic;
        backpad2Image.color = number > 0 ? Color.white : sprite ? Color.gray : new Color(1, 1, 1, 0);
        backpad2Text.text = number.ToString();
        backpad2Text.gameObject.SetActive(number > 0);
    }

    public void SetProductImage(Sprite sprite, int number) {

        productImage.sprite = sprite ?? basic;
        productImage.color = number > 0 ? Color.white : sprite ? Color.gray : new Color(1, 1, 1, 0);
        productText.text = number.ToString();
        productText.gameObject.SetActive(number > 0);
    }


    public void SetWorking(bool work) {
        workingUI.SetActive(work);
        ItemUI.SetActive(!work);
    }

    public void SetItem() {
        SetWorking(false);
        assembling.ResetProduct();
    }

    public void Close() {
        this.gameObject.SetActive(false);
    }
}
