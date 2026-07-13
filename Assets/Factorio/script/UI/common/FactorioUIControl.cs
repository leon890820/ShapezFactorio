using UnityEngine;
using UnityEngine.UI;

public abstract class FactorioUIControlBase : MonoBehaviour {
    public Sprite basic;
    public Slider progress;

    private bool initialized;
    private bool bound;
    protected FactorioGameObjectBase target;

    public void Open(FactorioGameObjectBase factorioBuilding) {
        if (!initialized) {
            InitUI();
            initialized = true;
        }

        target = factorioBuilding;
        BindFactorioGameObject(factorioBuilding);
        bound = true;

        SetActive(true);
        UpdateUI();
    }

    private void Update() {
        if (!bound || !gameObject.activeSelf) return;
        UpdateUI();
    }

    public virtual void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    public virtual void UpdateUI() {

    }

    public virtual void SetValue(float value) {
        progress.value = value;
    }

    public virtual void Close() {
        bound = false;
        target = null;
        SetActive(false);
    }

    public virtual void InitUI() {
    
    }

    public abstract void BindFactorioGameObject(FactorioGameObjectBase factorioBuilding);
}
