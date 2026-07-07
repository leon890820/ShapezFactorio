using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactorioGameObjectBase : MonoBehaviour,IAssembled{
    public Sprite factorioSprite;

    protected virtual void Awake() {

    }

    protected virtual void Start() {        

    }
    protected virtual void Update() {

    }

    public abstract FactorioPrefabBaseObject Clone();

    public virtual List<FactorioGameObjectBasePacket> GetItemMaterial() {
        return null;
    }
    public void SetSprite(Sprite sprite) {
        factorioSprite = sprite;
    }

}

public class FactorioGameObjectBasePacket {
    public FactorioPrefabBaseObject factorioPrefab;
    public int number;

    public FactorioGameObjectBasePacket(FactorioPrefabBaseObject factorioPrefab, int number) { 
        this.factorioPrefab = factorioPrefab;
        this.number = number;
    }

    public Sprite GetSprite() {
        return factorioPrefab?.info;
    }
}
