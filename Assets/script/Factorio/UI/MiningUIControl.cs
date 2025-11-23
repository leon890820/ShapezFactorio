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

    public void InitItemUI(MiningDrill md,ChunkCoord cc) { 
        FactorioPlanet planet = GalaxyManager.GetFactorioPlanet(cc);
        if(!planet) return;
        float space = 60f;
        int resourceCount = planet.minableResource.Count;
        float startX = -(resourceCount / 2 * space) + (resourceCount % 2 == 1 ? 0 : space / 2); 
        for (int i = 0; i < planet.minableResource.Count; i++) { 
            GameObject buttonUI = Instantiate(buttonPrefab);
            buttonUI.transform.SetParent(ItemUI.transform);
            buttonUI.transform.localPosition = new Vector3(startX , 0);
            startX += space;

            Image image = buttonUI.GetComponent<Image>();
            image.sprite = PrefabManager.Instance.GetSprite(planet.minableResource[i]);

            int index = i;
            Button button = buttonUI.GetComponent<Button>();
            button.onClick.AddListener(() => {
                SetMiningResource(md, planet.minableResource[index]);
            });
        }
    }

    public void SetMiningResource(MiningDrill md,string name) {
        FactorioPrefabBaseObject fgob = PrefabManager.Instance.GetPrefab(name);
        md.SetResource(fgob);
    }

    public void SetValue(float value) { 
        progress.value = value;
    }

    public void SetWorking(bool work) { 
        workingUI.SetActive(work);
        ItemUI.SetActive(!work);
    }

    public void SetbackpadImage(Sprite sprite, int number) {
        backpadImage.sprite = sprite ?? basic;
        backpadText.text = number.ToString();
        backpadText.gameObject.SetActive(number > 0);
    }

}
