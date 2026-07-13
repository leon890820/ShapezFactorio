using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorioBackpad{
    public int backpadMax;
    public List<FactorioGameObjectBase>[] backpad;
    public FactorioBackpad(int backpadMax) {
        backpad = new List<FactorioGameObjectBase>[1];
        for (int i = 0; i < 1; i++) {
            backpad[i] = new();
        }
        this.backpadMax = backpadMax;
    }

    public FactorioBackpad(int chestSize, int backpadMax) {
        backpad = new List<FactorioGameObjectBase>[chestSize];
        for (int i = 0; i < chestSize; i++) {
            backpad[i] = new();
        }
        this.backpadMax = backpadMax;
    }

    public void AddFactorioGameObject(int index, FactorioGameObjectBase factorioResource) {
        backpad[index].Add(factorioResource);
    }

    public bool TryInput(FactorioGameObjectBase factorioResource) {
        for (int index = 0; index < backpad.Length; index++) {
            if (backpad[index].Count == 0) {
                AddFactorioGameObject(index, factorioResource);
                return true;
            } else if (backpad[index].Count < backpadMax) {
                if (factorioResource.GetType() == backpad[index][0].GetType()) {
                    AddFactorioGameObject(index, factorioResource);
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryInput(FactorioGameObjectBase factorioResource, int index) {
        if (backpad[index].Count == 0) {
            AddFactorioGameObject(index, factorioResource);
            return true;
        } else if (backpad[index].Count < backpadMax) {
            if (factorioResource.GetType() == backpad[index][0].GetType()) {
                AddFactorioGameObject(index, factorioResource);
                return true;
            }
        }        
        return false;
    }

    public bool IsSameType(FactorioGameObjectBase factorioResource, int index) {
        if (backpad[index].Count == 0) {
            return false;
        } else {
            if (factorioResource.GetType() == backpad[index][0].GetType()) {
                return true;
            }
        }
        return false;
    }

    public FactorioGameObjectBase Pop() {
        for (int index = backpad.Length - 1; index >= 0; index--) {
            if (backpad[index].Count > 0) {
                FactorioGameObjectBase grabbedObject = backpad[index][^1];
                backpad[index].RemoveAt(backpad[index].Count - 1);                
                return grabbedObject;
            }
        }
        return null;
    }

    public FactorioGameObjectBase Peak() {
        for (int index = backpad.Length - 1; index >= 0; index--) {
            if (backpad[index].Count > 0) {
                FactorioGameObjectBase grabbedObject = backpad[index][^1];
                return grabbedObject;
            }
        }
        return null;
    }

    public FactorioGameObjectBase Pop(int index) {

        if (backpad[index].Count > 0) {
            FactorioGameObjectBase grabbedObject = backpad[index][^1];
            backpad[index].RemoveAt(backpad[index].Count - 1);
            return grabbedObject;
        }
        
        return null;
    }

    public FactorioGameObjectBase Peak(int index) {
        if (backpad[index].Count > 0) {
            FactorioGameObjectBase grabbedObject = backpad[index][^1];
            return grabbedObject;
        }
        return null;
    }

    public bool IsFull() {
        for (int i = 0; i < backpad.Length; i++) {
            if (backpad[i].Count < backpadMax) return false;
        }
        return true;
    }

    public bool IsEmpty() {
        for (int i = 0; i < backpad.Length; i++) {
            if (backpad[i].Count > 0) return false;
        }
        return true;
    }

    public bool IsFull(int index) {
        if (backpad[index].Count < backpadMax) return false;
        return true;
    }

    public bool IsEmpty(int index) {
        if (backpad[index].Count > 0) return false;        
        return true;
    }

    public bool IsSomeType<T>() {
        for (int i = 0; i < backpad.Length; i++) {
            var list = backpad[i];
            if (list == null || list.Count == 0 || list[0] is not T)
                return false;
        }

        return true;
    }



    public bool IsSomeType<T>(int index) {

        var list = backpad[index];
        if (list == null || list.Count == 0 || list[0] is not T)
            return false;
        

        return true;
    }

    public (FactorioGameObjectBase, int) GetBackpadIndexInfo(int index) {
        if (backpad[index].Count > 0) {
            FactorioGameObjectBase grabbedObject = backpad[index][^1];
            return (grabbedObject, backpad[index].Count);
        }        
        return (null, 0);
    }

    public int GetBackpadCount(int index) {
        return backpad[index].Count;
    }

    public int Count() { 
        return backpad.Length;
    }

    public void Clear() {
        for (int i = 0; i < backpad.Length; i++) {
            var list = backpad[i];

            for (int j = 0; j < list.Count; j++) {
                Object.Destroy(list[j].gameObject);
            }

            list.Clear();
        }
    }

}
