using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObject : SelectableObject {
    private void Start() {
        resources.Add(new Stone(10000));
    }

}
