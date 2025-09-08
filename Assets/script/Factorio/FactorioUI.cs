using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFactorioUI{
    GameObject UI { get; set; }
    virtual void CreateUI() {
        
    }
}
