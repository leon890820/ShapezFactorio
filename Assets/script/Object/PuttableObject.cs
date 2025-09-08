using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuttableObject : SelectableObject {
    protected GameObject puttable_object;
    protected int type, rotation;
    public abstract void CreatObjectOnTheWorld(Vector3 p, Quaternion q);
        
    

    public virtual void ChangeTypeAndRotation(int t, int r) { 
        type = t;
        rotation = r;
    }

    public virtual void CheckSurround() {

    }
    
}
