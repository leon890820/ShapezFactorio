using System;
using UnityEngine;
using UnityEngine.UI;

abstract public class FactorioButton<T> : FactorioButtonBase {


    protected new Action<T> OnButtonClick;


    // Start is called before the first frame update
    protected override void Start() {
        image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(() => {
            OnClick(GetValue());
        });

    }


    public virtual void OnClick(T value) {

        OnButtonClick?.Invoke(value);
    }


    public void AddAction(Action<T> action) {
        OnButtonClick += action;
    }

    public void RemoveAction(Action<T> action) {
        OnButtonClick -= action;
    }

    protected abstract T GetValue();


}


public abstract class FactorioButtonBase : MonoBehaviour {

    public string ButtonName = "button";
    protected Action OnButtonClick;
    protected Image image;
    protected virtual void Start() {
        image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void AddAction(Action action) {
        OnButtonClick += action;
    }

    public void RemoveAction(Action action) {
        OnButtonClick -= action;
    }

    public virtual void OnClick() {
        OnButtonClick?.Invoke();
    }

    public void SetImage(Sprite sprite) {
        if (!image) image = GetComponent<Image>();
        image.sprite = sprite;

    }

}