using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerGrid : MonoBehaviour{


    public Shader shader;
    public Color backgroundColor;
    public Color lineColor;

    public PlayerControll playerControll;

    private Material material;



    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (!material) { 
            material = new Material(shader);
        }

        

        material.SetFloat("layer", PlayerControll.BuildingLayer);
        material.SetVector("backgroundColor", backgroundColor);
        material.SetVector("lineColor", lineColor);
        if(PlayerControll.anchor.Count > 0) material.SetVector("hitPosition", PlayerControll.anchor[PlayerControll.anchor.Count - 1]);

        Vector3 size = new Vector3(FactorioData.platformHalfTexelSize - 1, FactorioData.platformHalfTexelSize - 1, FactorioData.platformHalfTexelSize - 1);

        if (PlayerControll.anchor.Count > 0) {
            material.SetVector("boundMin", PlayerControll.anchor[PlayerControll.anchor.Count - 1] - size);
            material.SetVector("boundMax", PlayerControll.anchor[PlayerControll.anchor.Count - 1] + size);
        }



        Graphics.Blit(source, destination, material);
    }

}
