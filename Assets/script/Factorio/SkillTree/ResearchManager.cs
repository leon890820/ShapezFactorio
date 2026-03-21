using System.Collections.Generic;
using UnityEngine;

public class ResearchManager : MonoBehaviour {
    public static ResearchManager Instance { get; private set; }

    private SkillNode currentSkillNode;
    private readonly Dictionary<string, int> progress = new();
    List<FactorioGameObjectBase> researchScienceList = new();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;        
    }

    private void Start() {
        GameStats.Instance.OnStatsUpdated += CheckResearchComplete;
    }


    private void OnDestroy() {
        if (Instance == this)
            Instance = null;

        if (GameStats.Instance != null)
            GameStats.Instance.OnStatsUpdated -= CheckResearchComplete;
    }

    public void SetSkillNode(SkillNode skillNode) {
        currentSkillNode = skillNode;
        progress.Clear();

        if (currentSkillNode == null)
            return;

        var conditions = currentSkillNode.GetUnlockConditionData();
        if (conditions == null)
            return;

        foreach (var condition in conditions) {
            if (condition == null || string.IsNullOrEmpty(condition.name))
                continue;

            progress[condition.name] = 0;
        }       
        researchScienceList = SetResearchScienceList();
        SkillNodeManager.Instance.SetImage(currentSkillNode.GetImage());
        UpdateResearchProgressUI();
    }

    public bool HasResearchingNode() {
        return currentSkillNode != null;
    }
    public List<FactorioGameObjectBase> GetResearchScienceList() => researchScienceList;
    public List<FactorioGameObjectBase> SetResearchScienceList() {

        List<FactorioGameObjectBase> result = new();

        if (currentSkillNode == null)
            return result;

        var conditions = currentSkillNode.GetUnlockConditionData();
        if (conditions == null)
            return result;

        foreach (var condition in conditions) {
            if (condition == null || string.IsNullOrEmpty(condition.name))
                continue;

            var prefabData = PrefabManager.Instance.GetPrefab(condition.name);
            if (prefabData == null || prefabData.object_prefab == null)
                continue;

            if (prefabData.object_prefab is SciencePack sciencePack)
                result.Add(sciencePack);
        }

        return result;
    }

    public void CheckResearchComplete() {
        Debug.Log("CheckResearchComplete");
        if (!IsResearchComplete())
            return;

        CompleteCurrentResearch();
    }

    public bool IsResearchComplete() {
        if (currentSkillNode == null)
            return false;
        UpdateResearchProgressUI();
        var conditions = currentSkillNode.GetUnlockConditionData();
        if (conditions == null || conditions.Length == 0)
            return false;

        foreach (var condition in conditions) {
            if (condition == null || string.IsNullOrEmpty(condition.name))
                return false;

            var prefabData = PrefabManager.Instance.GetPrefab(condition.name);
            if (prefabData == null || prefabData.object_prefab == null)
                return false;

            if (prefabData.object_prefab is SciencePack) {
                if (!progress.TryGetValue(condition.name, out int currentProgress))
                    return false;

                if (currentProgress < condition.requiredCount)
                    return false;
            } else {
                int itemAmount = GameStats.Instance.GetItemAmount(condition.name);
                progress[condition.name] = itemAmount;

                if (itemAmount < condition.requiredCount)
                    return false;
            }
        }

        return true;
    }

    public bool IsResearchComplete(string name) {
        var conditions = currentSkillNode.GetUnlockConditionData();
        foreach (var condiction in conditions) {
            if (!condiction.name.Equals(condiction)) continue;
            if (condiction.requiredCount >= progress[name]) {                
                return true;
            }
        }
        return false;
    }

    public void IncrementProgress(string itemName, int amount = 1) {
        if (currentSkillNode == null)
            return;

        if (!progress.ContainsKey(itemName))
            return;

        progress[itemName] += amount;

        if (!IsResearchComplete())
            return;

        CompleteCurrentResearch();
    }

    private void CompleteCurrentResearch() {
        if (currentSkillNode == null)
            return;
        SkillNodeManager.Instance.SetImage(null);
        SkillNodeManager.Instance.SetDescription("", null);
        currentSkillNode.SetUnLock();
        currentSkillNode = null;
        researchScienceList.Clear();
        progress.Clear();
    }

    private void UpdateResearchProgressUI() {
        var conditions = currentSkillNode.GetUnlockConditionData();
        for(int i = 0; i < conditions.Length; i++) {
            var condiction = conditions[i];
            string item = progress[condiction.name] + "/" + condiction.requiredCount;
            SkillNodeManager.Instance.SetDescription(item, PrefabManager.Instance.GetPrefab(condiction.name).info, i);
        }
    }
}