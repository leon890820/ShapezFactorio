using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Noise 
{
    public static float CalculateNoise(Vector3 p,Vector3 bias,float scale) {
        return Mathf.PerlinNoise(p.x*scale+bias.x,p.z*scale+bias.z);
    }
    
}
