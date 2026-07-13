using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SkillNode : MonoBehaviour {
    public List<SkillNode> previous = new List<SkillNode>();
    public List<SkillNode> next = new List<SkillNode>();

    public SkillNodeLineUpdater skillNodeLineUpdater;
    public RectTransform rectTransform;
    public ImageManager imageManager;
    public Color[] statusColors;

    public DescriptionController descriptionController;
    public Transform tipTransform;

    private IUnlockCondition unlockCondition;

    private SkillStatus skillStatus = SkillStatus.Locked;



    private void Awake() {
        unlockCondition = GetComponent<IUnlockCondition>();
        rectTransform = transform as RectTransform;        
    }

    private void Start() {
        CreateResearchDescription();
    }

    private void OnValidate() {
        if (skillNodeLineUpdater != null)
            skillNodeLineUpdater.MarkDirty(this);
    }

    private void Update() {
        skillStatus = UpdateSkillStatus();
        UpdateSkillImage();
    }

    private SkillStatus UpdateSkillStatus() {
        if (skillStatus == SkillStatus.Unlocked)
            return SkillStatus.Unlocked;

        if (skillStatus == SkillStatus.Researching)
            return SkillStatus.Researching;

        if (!ArePrerequisitesUnlocked())
            return SkillStatus.Locked;

        return CanResearch()
            ? SkillStatus.Available
            : SkillStatus.Locked;
    }
    private bool ArePrerequisitesUnlocked() {
        if (previous == null)
            return true;

        foreach (var node in previous) {
            if (node.GetSkillStatus() != SkillStatus.Unlocked)
                return false;
        }

        return true;
    }

    private void UpdateSkillImage() {
        imageManager.SetImageColor(statusColors[(int)skillStatus]);
    }

    public bool CanResearch() {
        if (unlockCondition == null)
            return true;

        return unlockCondition.IsUnlocked();
    }

    public SkillStatus GetSkillStatus() {
        return skillStatus;
    }

    public void CreateResearchDescription() {
        var descriptions = unlockCondition.GetUnlockDescription();
        int count = 0;
        foreach (var description in descriptions) {
            var descriptionCont = Instantiate(descriptionController, tipTransform);
            descriptionCont.SetSprite(description.GetSprite());
            descriptionCont.SetText("x" + description.number.ToString());
            descriptionCont.SetPosition(new Vector3(-70 + count * 60, 30, 0));
        }
    }

    public void Research() {
        if (skillStatus != SkillStatus.Available) return;
        ResearchManager.Instance.SetSkillNode(this);
    }

    public void SetUnLock() {
        skillStatus = SkillStatus.Unlocked;
    }

    public ProductionUnlockConditionData[] GetUnlockConditionData() {
        return unlockCondition.GetUnlockConditionData();
    }

    public Sprite GetImage() {
        return imageManager.GetImage();

    }
}

public enum SkillStatus {
    Unlocked,      // 已解鎖
    Available,     // 可以研究
    Researching,   // 研究中
    Locked         // 無法研究
}