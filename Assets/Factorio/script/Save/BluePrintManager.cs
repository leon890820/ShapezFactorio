using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class BluePrintManager : MonoBehaviour{

    public static BluePrintManager Instance { get; private set; }
    public Transform blueprintPivot;
    public ClickButton buttonPrefab;

    private string folderPath;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        folderPath = Path.Combine(Application.dataPath, "Factorio", "res", "save");
    }


    public void Refresh() {
        ClearButtons();
        string[] files = Directory.GetFiles(folderPath, "*.json");

        foreach (string file in files) {
            CreateBlueprintButton(file);
        }
    }

    private void CreateBlueprintButton(string filePath) {
        ClickButton buttonInstance = Instantiate(buttonPrefab, blueprintPivot);
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        buttonInstance.name = "BlueprintButton_" + fileName;
        buttonInstance.SetText(fileName);
        buttonInstance.SetTextPosition(Vector3.zero);
        buttonInstance.AddAction(() => OnClickBlueprint(filePath));
    }

    private void OnClickBlueprint(string filePath) {
        string json = File.ReadAllText(filePath);
        var data = JsonUtility.FromJson<PlayGroundBuildingBlueprintData>(json);
        PlayerControll.Instance.LoadBuilding(data);
        gameObject.SetActive(false);
    }

    private void ClearButtons() {
        for (int i = blueprintPivot.childCount - 1; i >= 0; i--) {
            Destroy(blueprintPivot.GetChild(i).gameObject);
        }
    }

    public void ToggleUI() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

}
