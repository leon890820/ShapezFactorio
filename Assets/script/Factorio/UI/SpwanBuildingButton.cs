using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpwanBuildingButton : FactorioButtonBase {
    public string spwanName;

    public override void OnClick() {

        FactorioPrefabBaseObject gameObject = PrefabManager.Instance.GetPrefab(spwanName);
        PlayerControll.SpawnBuilding(gameObject);

        base.OnClick();
    }

    public void SetImage() {

        if (!image) image = GetComponent<Image>();
        image.sprite = PrefabManager.Instance.GetSprite(spwanName); ;

    }

}
