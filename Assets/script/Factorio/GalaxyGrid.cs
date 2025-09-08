using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GalaxyGrid : MonoBehaviour {
    public Shader gridShader;

    private Material mat;
    private CommandBuffer commandBuffer;
    private Camera cam;
    private RenderTexture tempRT;

    void OnEnable() {


        //commandBuffer = new CommandBuffer();
        //commandBuffer.name = "GridBeforeForwardOpaque";

        //int tempID = Shader.PropertyToID("_TempRT");

        // 建立一個 RT 並 Blit 成灰階
        //commandBuffer.GetTemporaryRT(tempID, -1, -1, 0, FilterMode.Bilinear);
        //commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, tempID);
        //commandBuffer.Blit(tempID, BuiltinRenderTextureType.CameraTarget, mat);
        //commandBuffer.ReleaseTemporaryRT(tempID);

        // 插入在 image effect 之前（還沒進入 OnRenderImage）
        //cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);


    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (!mat) {
            mat = new Material(gridShader);

        }
        Graphics.Blit(source, destination, mat);
    }

    //void OnDisable() {
    //    if (commandBuffer != null) {
    //        cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
    //         commandBuffer.Release();
    //    }
    //}

}
