using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNodeManager : MonoBehaviour{
    public static SkillNodeManager Instance { get; private set; }

    public Image researchingNode;
    public DescriptionController[] descriptions;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ActiveUI(false);
    }


    public void ActiveUI(bool active) { 
        gameObject.SetActive(active);
    }

    public void ToggleUI() {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SetImage(Sprite sprite) {
        researchingNode.sprite = sprite;
    }

    public void SetDescription(string description, Sprite sprite, int index) { 
        descriptions[index].SetSprite(sprite);
        descriptions[index].SetText(description);
    }

    public void SetDescription(string description, Sprite sprite) {
        for (int i = 0; i < descriptions.Length; i++) {
            descriptions[i].SetSprite(sprite);
            descriptions[i].SetText(description);
        }
    }
}
