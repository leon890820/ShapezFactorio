using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpwanBuildingButton : FactorioButtonBase {
    public FactorioId spwanId;

    public override void OnClick() {
        FactorioPrefabBaseObject gameObject = PrefabManager.Instance.GetPrefab(spwanId);
        if (gameObject == null) return;
        PlayerControll.Instance.SpawnBuilding(gameObject);
        base.OnClick();
    }

    public void SetImage() {
        if (!image) image = GetComponent<Image>();
        image.sprite = PrefabManager.Instance.GetSprite(spwanId);
    }

}
