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
       
        material.SetFloat("layer", PlayerControll.Instance.GetBuildingLayer());
        material.SetVector("backgroundColor", backgroundColor);
        material.SetVector("lineColor", lineColor);

        var anchor = PlayerControll.Instance.GetAnchor();
        if (anchor.Count > 0) material.SetVector("hitPosition", anchor[^1]);

        Vector3 size = new Vector3(FactorioData.platformHalfTexelSize - 1, FactorioData.platformHalfTexelSize - 1, FactorioData.platformHalfTexelSize - 1);

        if (anchor.Count > 0) {
            material.SetVector("boundMin", anchor[^1] - size);
            material.SetVector("boundMax", anchor[^1] + size);
        }

        Graphics.Blit(source, destination, material);
    }

}
