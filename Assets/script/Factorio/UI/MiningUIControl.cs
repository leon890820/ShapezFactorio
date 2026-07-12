using System.Collections.Generic;
using UnityEngine;


public class MiningUIControl : FactorioUIControlBase {
    public FactorioBackpadUIManager product;    

    public GameObject workingUI;
    public GameObject ItemUI;

    public ClickButton buttonPrefab;

    private MiningDrill minedrill;
    private FactorioBackpad backpad;
    private readonly Dictionary<string, ClickButton> resourceButtons = new();

    public override void InitUI() {
        string[] resourceNames = FactorioLibrary.BasicResources;
        float space = 60f;
        int resourceCount = resourceNames.Length;
        float startX = -(resourceCount * 0.5f * space) + (space * 0.5f);
        for (int i = 0; i < resourceCount; i++) {
            string resourceName = resourceNames[i];
            ClickButton button = Instantiate(buttonPrefab);
            button.transform.SetParent(ItemUI.transform, false);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, 0);
            startX += space;

            button.SetImage(PrefabManager.Instance.GetSprite(resourceName));
            button.AddAction(() => SetMiningResource(resourceName));
            button.gameObject.SetActive(false);
            resourceButtons[resourceName] = button;
        }

    }

    public override void BindFactorioGameObject(FactorioGameObjectBase factorioGameObjectBase) {
        minedrill = factorioGameObjectBase as MiningDrill;
        backpad = minedrill.backpad;

        var cc = minedrill.GetChunkCoord();
        FactorioPlanet planet = GalaxyManager.Instance.GetFactorioPlanet(cc);

        foreach (var resourceButton in resourceButtons.Values) {
            resourceButton.gameObject.SetActive(false);
        }

        if (!planet) return;

        List<ClickButton> availableButtons = new();
        foreach (string resourceName in planet.minableResource) {
            if (!FactorioLibrary.IsBasicResource(resourceName)) continue;
            if (!resourceButtons.TryGetValue(resourceName, out ClickButton button)) continue;

            availableButtons.Add(button);
        }

        float space = 60f;
        float startX = -(availableButtons.Count * 0.5f * space) + (space * 0.5f);
        for (int i = 0; i < availableButtons.Count; i++) {
            ClickButton button = availableButtons[i];
            button.gameObject.SetActive(true);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + space * i, 0);
        }
    }

    public void SetMiningResource(string name) {
        FactorioPrefabBaseObject fgob = PrefabManager.Instance.GetPrefab(name);
        minedrill.SetResource(fgob);
    }

    public void SetWorking(bool work) { 
        workingUI.SetActive(work);
        ItemUI.SetActive(!work);
    }

    public void SetItem() {
        SetWorking(false);
        minedrill.ResetBuilding();
    }

    public override void UpdateUI() {
        if (!minedrill || backpad == null) return;

        var (backpadObj, backpadCount) = backpad.GetBackpadIndexInfo(0);
        SetbackpadImage(backpadObj?.factorioSprite, backpadCount);
        progress.value = minedrill.mining_count;
        if (minedrill.buildStatus == BuildStatus.NoRecipe) {
            SetWorking(false);
        } else {
            SetWorking(true);
        }
    }

    public void SetbackpadImage(Sprite sprite, int number) {
        product.SetbackpadImage(sprite ?? basic, number);
    }

}
