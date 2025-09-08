using UnityEngine;

public class Planet : MonoBehaviour {
    // === �~���Ѽ� ===
    public ComputeShader terrainCompute;

    public SimpleNoiseSettings noiseSettings_ground;
    public SimpleNoiseSettings noiseSettings_mask;
    public RidgeNoiseSettings ridgeNoiseSettings_mountain;

    public float oceanDepthMultiplier = 4.9f;
    public float oceanFloorDepth = 1.35f;
    public float oceanFloorSmoothing = 0.55f;
    public float mountainBlend = 1.0f;

    // === �������A ===
    private BodyPlaceholder bodyPlaceholder;
    private ComputeBuffer vertexBuffer;
    private PRNG prng;
    private bool settingsChanged;

    private Material material;

    // === ��l�� ===
    void Awake() {
        terrainCompute = Resources.Load<ComputeShader>("Shaders/Terrain");
        material = Resources.Load<Material>("Shaders/Earth");

        bodyPlaceholder = gameObject.AddComponent<BodyPlaceholder>();

       
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

    // === �D�n�a�Υͦ��y�{ ===
    void GenerateTerrain() {
        bodyPlaceholder.ResetMesh();       
        SetComputeData();
        ComputeHelper.Release(vertexBuffer);
    }

    // === �]�w Compute Shader �Ѽƨð��� ===
    void SetComputeData() {
        Vector3[] meshVerts = bodyPlaceholder.GetMeshVertexData();
        int count = bodyPlaceholder.GetVertexCount();

        vertexBuffer = ComputeHelper.CreateAndSetBuffer<Vector3>(meshVerts, terrainCompute, "vertex");
        terrainCompute.SetInt("numberVertex", count);

        SetNoiseParameters();
        SetGlobalFloatParameters();

        ComputeHelper.Run(terrainCompute, count);

        Vector3[] resultVerts = new Vector3[count];
        vertexBuffer.GetData(resultVerts);
        bodyPlaceholder.SetVertexData(resultVerts);
    }

    // === ���n��l�� ===
    void InitNoise() {
        noiseSettings_ground = new SimpleNoiseSettings();
        noiseSettings_ground.SetParameter(6, 2f, 0.5f, 1f, 4.65f, 0f);

        noiseSettings_mask = new SimpleNoiseSettings();
        noiseSettings_mask.SetParameter(3, 2.06f, 0.76f, 1.34f, 1f, -0.38f);

        ridgeNoiseSettings_mountain = new RidgeNoiseSettings();
        ridgeNoiseSettings_mountain.SetParameter(7, 2.61f, 0.5f, 1.26f, 1.17f, 5.42f, 1.24f, 0.11f, 0f);
    }

    // === �ǰe Noise �]�w�Ѽƨ� ComputeShader ===
    void SetNoiseParameters() {
        noiseSettings_ground.SetComputeValues(terrainCompute, prng, "_ground");
        noiseSettings_mask.SetComputeValues(terrainCompute, prng, "_mask");
        ridgeNoiseSettings_mountain.SetComputeValues(terrainCompute, prng, "_mountains");
    }

    // === �ǰe�B�I�Ѽơ]�D Noise�^ ===
    void SetGlobalFloatParameters() {
        terrainCompute.SetFloat("oceanDepthMultiplier", oceanDepthMultiplier);
        terrainCompute.SetFloat("oceanFloorDepth", oceanFloorDepth);
        terrainCompute.SetFloat("oceanFloorSmoothing", oceanFloorSmoothing);
        terrainCompute.SetFloat("mountainBlend", mountainBlend);
    }
}