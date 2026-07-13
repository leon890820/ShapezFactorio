using System.Collections.Generic;
using UnityEngine;

public class ResearchManager : MonoBehaviour {
    public static ResearchManager Instance { get; private set; }

    private SkillNode currentSkillNode;
    private readonly Dictionary<FactorioId, int> progress = new();
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
            if (condition == null || condition.id == FactorioId.None)
                continue;

            progress[condition.id] = 0;
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
            if (condition == null || condition.id == FactorioId.None)
                continue;

            var prefabData = PrefabManager.Instance.GetPrefab(condition.id);
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
            if (condition == null || condition.id == FactorioId.None)
                return false;

            var prefabData = PrefabManager.Instance.GetPrefab(condition.id);
            if (prefabData == null || prefabData.object_prefab == null)
                return false;

            if (prefabData.object_prefab is SciencePack) {
                if (!progress.TryGetValue(condition.id, out int currentProgress))
                    return false;

                if (currentProgress < condition.requiredCount)
                    return false;
            } else {
                int itemAmount = GameStats.Instance.GetItemAmount(condition.id);
                progress[condition.id] = itemAmount;

                if (itemAmount < condition.requiredCount)
                    return false;
            }
        }

        return true;
    }

    public bool IsResearchComplete(FactorioId id) {
        var conditions = currentSkillNode.GetUnlockConditionData();
        foreach (var condiction in conditions) {
            if (condiction.id != id) continue;
            if (condiction.requiredCount >= progress[id]) {
                return true;
            }
        }
        return false;
    }

    public void IncrementProgress(FactorioId id, int amount = 1) {
        if (currentSkillNode == null)
            return;

        if (!progress.ContainsKey(id))
            return;

        progress[id] += amount;

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
            string item = progress[condiction.id] + "/" + condiction.requiredCount;
            SkillNodeManager.Instance.SetDescription(item, PrefabManager.Instance.GetPrefab(condiction.id).info, i);
        }
    }
}
