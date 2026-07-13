using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class EarthShape : FactorioPlanetShape{


    public NoiseSettingsData noiseSettingAttribute;
    private BodyPlaceholder bodyPlaceholder;
    private ComputeBuffer vertexBuffer;
    private PRNG prng;

    private SimpleNoiseSettings noiseSettings_ground;
    private SimpleNoiseSettings noiseSettings_mask;
    private RidgeNoiseSettings ridgeNoiseSettings_mountain;

    void Awake() {
        bodyPlaceholder = GetComponent<BodyPlaceholder>();
        prng = new PRNG(0);
        InitNoise();
        
    }

    public override void CalcHeight() {
        Vector3[] meshVerts = bodyPlaceholder.GetMeshVertexData();
        int count = bodyPlaceholder.GetVertexCount();

        vertexBuffer = ComputeHelper.CreateAndSetBuffer<Vector3>(meshVerts, terrainCompute, "vertex");
        terrainCompute.SetInt("numberVertex", count);

        SetNoiseParameters();
        if (noiseSettingAttribute.hasOcean) SetGlobalFloatParameters();

        ComputeHelper.Run(terrainCompute, count);

        Vector3[] resultVerts = new Vector3[count];
        vertexBuffer.GetData(resultVerts);
        bodyPlaceholder.SetVertexData(resultVerts);
    }

    void SetNoiseParameters() {
        noiseSettings_ground.SetComputeValues(terrainCompute, prng, "_ground");
        noiseSettings_mask.SetComputeValues(terrainCompute, prng, "_mask");
        ridgeNoiseSettings_mountain.SetComputeValues(terrainCompute, prng, "_mountains");
    }

    // === 傳送浮點參數（非 Noise） ===
    void SetGlobalFloatParameters() {
        terrainCompute.SetFloat("oceanDepthMultiplier", noiseSettingAttribute.oceanDepthMultiplier);
        terrainCompute.SetFloat("oceanFloorDepth", noiseSettingAttribute.oceanFloorDepth);
        terrainCompute.SetFloat("oceanFloorSmoothing", noiseSettingAttribute.oceanFloorSmoothing);
        terrainCompute.SetFloat("mountainBlend", noiseSettingAttribute.mountainBlend);
    }

    public override void Release() {
        ComputeHelper.Release(vertexBuffer);
    }

    void InitNoise() {
        noiseSettings_ground = new SimpleNoiseSettings();
        noiseSettings_ground.SetParameter(noiseSettingAttribute.ground);

        noiseSettings_mask = new SimpleNoiseSettings();
        noiseSettings_mask.SetParameter(noiseSettingAttribute.mask);

        ridgeNoiseSettings_mountain = new RidgeNoiseSettings();
        ridgeNoiseSettings_mountain.SetParameter(noiseSettingAttribute.mountain);
    }

}
