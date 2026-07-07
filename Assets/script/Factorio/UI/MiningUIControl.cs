using UnityEngine;


public class MiningUIControl : FactorioUIControlBase {
    public FactorioBackpadUIManager product;    

    public GameObject workingUI;
    public GameObject ItemUI;

    public ClickButton buttonPrefab;

    private MiningDrill minedrill;
    private FactorioBackpad backpad;

    public override void BindFactorioGameObject(FactorioGameObjectBase factorioGameObjectBase) {
        minedrill = factorioGameObjectBase as MiningDrill;
        var cc = minedrill.GetChunkCoord();
        FactorioPlanet planet = GalaxyManager.Instance.GetFactorioPlanet(cc);
        if (!planet) return;
        float space = 60f;
        int resourceCount = planet.minableResource.Count;
        float startX = -(resourceCount * 0.5f * space) + (space * 0.5f);

        for (int i = 0; i < planet.minableResource.Count; i++) {
            ClickButton button = Instantiate(buttonPrefab);
            button.transform.SetParent(ItemUI.transform, false);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, 0);
            startX += space;

            button.SetImage(PrefabManager.Instance.GetSprite(planet.minableResource[i]));            
            string resourceName = planet.minableResource[i];
            button.AddAction(() => SetMiningResource(resourceName));
        }

        backpad = minedrill.backpad;
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
        var (backpadObj, backpadCount) = backpad.GetBackpadIndexInfo(0);
        SetbackpadImage(backpadObj?.factorioSprite, backpadCount);
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
