using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronChest : FactorioPlatformBuilding{
    
    List<FactorioGameObjectBase>[] chestBackpad;
    public int chestSize = 30;
    private IronChestUIControll ironChestUIControll;

    protected override void Start() {
        base.Start();
        backpadMax = 30;
        chestBackpad = new List<FactorioGameObjectBase>[chestSize];
        for (int i = 0; i < chestSize; i++) {
            chestBackpad[i] = new List<FactorioGameObjectBase>();
        }
        ironChestUIControll = factorioUIControlBase as IronChestUIControll;
        ironChestUIControll.InitItemUI(this);
        ironChestUIControll.UpdateUI(chestBackpad);
    }
    
    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f, 0.0f, 0.5f);
    }

    public override bool TryInput(FactorioGameObjectBase factorioResource, Vector3Int pos, int i, bool mid = false) {
        for (int index = 0; index < chestBackpad.Length; index++) {
            if (chestBackpad[index].Count == 0) {
                AddFactorioGameObjectToBackpad(index, factorioResource);
                return true;
            } else if (chestBackpad[index].Count < backpadMax) {
                if (factorioResource.GetType() == chestBackpad[index][0].GetType()) {
                    AddFactorioGameObjectToBackpad(index, factorioResource);
                    return true;
                }
            }
        }
        return false;
    }

    private void AddFactorioGameObjectToBackpad(int index, FactorioGameObjectBase factorioResource) {
        factorioResource.transform.SetParent(transform);
        factorioResource.transform.localPosition = new Vector3();
        chestBackpad[index].Add(factorioResource);
        ironChestUIControll.UpdateUI(chestBackpad);
    }

    public override FactorioGameObjectBase TryBeGrab() {
        for (int index = chestBackpad.Length - 1; index >= 0; index--) {
            if (chestBackpad[index].Count > 0) {
                FactorioGameObjectBase grabbedObject = chestBackpad[index][chestBackpad[index].Count - 1];
                chestBackpad[index].RemoveAt(chestBackpad[index].Count - 1);
                ironChestUIControll.UpdateUI(chestBackpad);
                grabbedObject.transform.SetParent(null);
                return grabbedObject;
            }
        }
        return null;
    }

}
