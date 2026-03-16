using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FactorioUIControlBase : MonoBehaviour {
    public Sprite basic;
    public Slider progress;

    protected readonly Dictionary<string, int> productPair = new Dictionary<string, int> {
        { "Gear", 1 },
        { "RedSciencePack", 1}
    };

    public virtual void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    public virtual void UpdateUI() {

    }

    public virtual void SetValue(float value) {
        progress.value = value;
    }
    public void Close() {
        this.gameObject.SetActive(false);
    }

    abstract public void InitItemUI(FactorioGameObjectBase factorioBuilding);
}
