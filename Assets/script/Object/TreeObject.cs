using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : SelectableObject{
    private void Start() {
        resources.Add(new Wood(500));       
    }
}


