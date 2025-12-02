using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MiningUIControl : FactorioUIControlBase {
    public Image backpadImage;
    public TextMeshProUGUI backpadText;

    public Slider progress;

    public GameObject workingUI;
    public GameObject ItemUI;

    public GameObject buttonPrefab;
    private MiningDrill minedrill;

    public void InitItemUI(MiningDrill md,ChunkCoord cc) { 
        FactorioPlanet planet = GalaxyManager.GetFactorioPlanet(cc);
        minedrill = md;
        if (!planet) return;
        float space = 60f;
        int resourceCount = planet.minableResource.Count;
        float startX = -(resourceCount * 0.5f * space) + (space * 0.5f);

        for (int i = 0; i < planet.minableResource.Count; i++) { 
            GameObject buttonUI = Instantiate(buttonPrefab);
            buttonUI.transform.SetParent(ItemUI.transform, false);
            buttonUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, 0);
            startX += space;

            Image image = buttonUI.GetComponent<Image>();
            image.sprite = PrefabManager.Instance.GetSprite(planet.minableResource[i]);

            Button button = buttonUI.GetComponent<Button>();
            string resourceName = planet.minableResource[i];
            button.onClick.AddListener(() => SetMiningResource(resourceName));
        }
    }

    public void SetMiningResource(string name) {
        FactorioPrefabBaseObject fgob = PrefabManager.Instance.GetPrefab(name);
        minedrill.SetResource(fgob);
    }

    public void SetValue(float value) { 
        progress.value = value;
    }

    public void SetWorking(bool work) { 
        workingUI.SetActive(work);
        ItemUI.SetActive(!work);
    }

    public void SetItem() {
        SetWorking(false);
        minedrill.ResetBuilding();
    }

    public void Close() { 
        this.gameObject.SetActive(false);
    }

    public void SetbackpadImage(Sprite sprite, int number) {
        backpadImage.sprite = sprite ?? basic;
        backpadText.text = number.ToString();
        backpadText.gameObject.SetActive(number > 0);
    }

}
