using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettingsData",menuName = "Terrain/Noise Settings Data",order = 0)]
public class NoiseSettingsData : ScriptableObject {
    // ------ /琍瞴俱砰把计 ------
    [Header("Global / Terrain Blend Settings")]
    public bool hasOcean = true;
    public float oceanDepthMultiplier = 4.9f;
    public float oceanFloorDepth = 1.35f;
    public float oceanFloorSmoothing = 0.55f;
    public float mountainBlend = 1.0f;

    // ------ 摸 Noise 灿竊把计 ------
    [System.Serializable]
    public struct SimpleNoiseParams {
        [Header("SimpleNoiseSettings.SetParameter")]
        public int numLayers;
        public float lacunarity;
        public float persistence;
        public float scale;
        public float elevation;
        public float verticalShift;
    }

    [System.Serializable]
    public struct RidgeNoiseParams {
        [Header("RidgeNoiseSettings.SetParameter")]
        public int numLayers;
        public float lacunarity;
        public float persistence;
        public float scale;
        public float power;
        public float elevation;
        public float gain;
        public float verticalShift;
        public float peakSmoothing;
    }

    [Header("Ground Noise (noiseSettings_ground)")]
    public SimpleNoiseParams ground;

    [Header("Mask Noise (noiseSettings_mask)")]
    public SimpleNoiseParams mask;

    [Header("Mountain Ridge Noise (ridgeNoiseSettings_mountain)")]
    public RidgeNoiseParams mountain;
}
