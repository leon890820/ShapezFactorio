using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : FactorioObject{
    public int number;
    public string name;
    public Sprite info_image;   

    public Resource() : base() {
        number = 0;
    }

    public Resource(int num) : base() {
        number = num;
    }

    public Resource(string _name,Sprite s) : base() { 
        name = _name;
        info_image = s;
    }
}


public class Wood : Resource{

    public Wood() : base() {
        info_image = world.resource_setting.wood.info;
        name = world.resource_setting.wood.name;
    }
    public Wood(int num) : base(num) {
        info_image = world.resource_setting.wood.info;
        name = world.resource_setting.wood.name;
    }
    public Wood(string _name, Sprite s) : base(_name, s) {    
    }
}

public class Stone : Resource{
    public Stone() : base() {
        info_image = world.resource_setting.stone.info;
        name = world.resource_setting.stone.name;
    }
    public Stone(int num) : base(num) {
        info_image = world.resource_setting.stone.info;
        name = world.resource_setting.stone.name;
    }
    public Stone(string _name, Sprite s) : base(_name, s) { 
    
    }

}
