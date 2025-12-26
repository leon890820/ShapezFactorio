using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;


public class PostProcessing : MonoBehaviour{
    // Start is called before the first frame update

    public Shader oceanShader;
    Material oceanMaterial;

    public Color colA;
    public Color colB;
    public float depthMutiplier = 1.0f;
    public float alphaMutiplier = 1.0f;


    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (!oceanMaterial) { 
            oceanMaterial = new Material(oceanShader);
        }

        var planets = GalaxyManager.Instance.planets.Values.ToList();

        oceanMaterial.SetColor("colA", colA);
        oceanMaterial.SetColor("colB", colB);
        oceanMaterial.SetFloat("depthMutiplier", depthMutiplier);
        oceanMaterial.SetFloat("alphaMutiplier", alphaMutiplier);
        oceanMaterial.SetVector("pos", planets[0].transform.position);
        oceanMaterial.SetFloat("radius", planets[0].transform.localScale.x);

        Graphics.Blit(source, destination, oceanMaterial);
    }

}
