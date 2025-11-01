using Cinemachine;
using UnityEngine;

public class FactorioPlanet : MonoBehaviour {
    // === 场把计 ===
    public ComputeShader terrainCompute;
    [SerializeField]
    public NoiseSettingsData noiseSettingAttribute;
    public Material material;

    // === ず场篈 ===
    private SimpleNoiseSettings noiseSettings_ground;
    private SimpleNoiseSettings noiseSettings_mask;
    private RidgeNoiseSettings ridgeNoiseSettings_mountain;

    private BodyPlaceholder bodyPlaceholder;
    private ComputeBuffer vertexBuffer;
    private PRNG prng;
    private bool settingsChanged;

    

    // === ﹍て ===
    void Awake() {
        bodyPlaceholder = GetComponent<BodyPlaceholder>();       
        bodyPlaceholder.SetMaterial(material);
        prng = new PRNG(0);
        InitNoise();
    }

    void Start() {
        GenerateTerrain();
        ComputeHelper.Release(vertexBuffer);
    }

    void Update() {
        if (settingsChanged) {
            settingsChanged = false;
            GenerateTerrain();
        }
    }

    void OnValidate() {
        settingsChanged = true;
    }

    // === 璶ネΘ瑈祘 ===
    void GenerateTerrain() {
        bodyPlaceholder.ResetMesh();       
        SetComputeData();
        ComputeHelper.Release(vertexBuffer);
    }

    // === 砞﹚ Compute Shader 把计磅︽ ===
    void SetComputeData() {
        Vector3[] meshVerts = bodyPlaceholder.GetMeshVertexData();
        int count = bodyPlaceholder.GetVertexCount();

        vertexBuffer = ComputeHelper.CreateAndSetBuffer<Vector3>(meshVerts, terrainCompute, "vertex");
        terrainCompute.SetInt("numberVertex", count);

        SetNoiseParameters();
        if(noiseSettingAttribute.hasOcean) SetGlobalFloatParameters();

        ComputeHelper.Run(terrainCompute, count);

        Vector3[] resultVerts = new Vector3[count];
        vertexBuffer.GetData(resultVerts);
        bodyPlaceholder.SetVertexData(resultVerts);
    }

    // === 靖羘﹍て ===
    void InitNoise() {
        noiseSettings_ground = new SimpleNoiseSettings();
        noiseSettings_ground.SetParameter(noiseSettingAttribute.ground);

        noiseSettings_mask = new SimpleNoiseSettings();
        noiseSettings_mask.SetParameter(noiseSettingAttribute.mask);

        ridgeNoiseSettings_mountain = new RidgeNoiseSettings();
        ridgeNoiseSettings_mountain.SetParameter(noiseSettingAttribute.mountain);
    }

    // === 肚癳 Noise 砞﹚把计 ComputeShader ===
    void SetNoiseParameters() {
        noiseSettings_ground.SetComputeValues(terrainCompute, prng, "_ground");
        noiseSettings_mask.SetComputeValues(terrainCompute, prng, "_mask");
        ridgeNoiseSettings_mountain.SetComputeValues(terrainCompute, prng, "_mountains");
    }

    // === 肚癳疊翴把计獶 Noise ===
    void SetGlobalFloatParameters() {
        terrainCompute.SetFloat("oceanDepthMultiplier", noiseSettingAttribute.oceanDepthMultiplier);
        terrainCompute.SetFloat("oceanFloorDepth", noiseSettingAttribute.oceanFloorDepth);
        terrainCompute.SetFloat("oceanFloorSmoothing", noiseSettingAttribute.oceanFloorSmoothing);
        terrainCompute.SetFloat("mountainBlend", noiseSettingAttribute.mountainBlend);
    }
}