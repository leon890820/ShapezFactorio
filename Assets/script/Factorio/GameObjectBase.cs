using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactorioGameObjectBase : MonoBehaviour{
    // Start is called before the first frame update

    public GameObject UIPrefab;
    public FactorioUIControlBase factorioUIControlBase;

    public Sprite factorioSprite;

    protected virtual void Awake() {
        if (UIPrefab) {
            UIPrefab  = Instantiate(UIPrefab);
            GameObject canvas = GameObject.Find("Canvas");
            UIPrefab.transform.SetParent(canvas.transform,false);
            factorioUIControlBase = UIPrefab.GetComponent<FactorioUIControlBase>();
        }
    }

    protected virtual void Start() { 
    
    }

    // Update is called once per frame
    protected virtual void Update() {

    }

    public abstract FactorioPrefabBaseObject Clone();

    public void SetSprite(Sprite sprite) {
        factorioSprite = sprite;
    }


    public virtual void SetUIEnable() {
        if (!UIPrefab) return;
        UIPrefab.SetActive(!UIPrefab.activeSelf);
        if(UIPrefab.activeSelf) FactorioGameObjectUIManager.AddUI(factorioUIControlBase);
        
    }
}
