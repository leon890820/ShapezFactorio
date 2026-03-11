using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

[ExecuteAlways]
public class SkillNodeLineUpdater : MonoBehaviour {
    [SerializeField] private SkillNode skillNode;
    [SerializeField] private List<UILineRenderer> lineRenderers = new List<UILineRenderer>();

    private bool pendingRebuild;

    private void Awake() {
        CacheReference();
    }

    private void OnEnable() {
        CacheReference();
        pendingRebuild = true;
    }

    private void OnValidate() {
        CacheReference();
        pendingRebuild = true;
    }

    private void Update() {
        if (Application.isPlaying)
            return;

        CacheReference();

        if (skillNode == null)
            return;

        bool needRefresh = pendingRebuild;

        if (skillNode.transform.hasChanged) {
            skillNode.transform.hasChanged = false;
            needRefresh = true;
        }

        foreach (SkillNode node in skillNode.next) {
            if (node == null)
                continue;

            if (node.transform.hasChanged) {
                node.transform.hasChanged = false;
                needRefresh = true;
            }
        }

        if (!needRefresh)
            return;

        if (pendingRebuild) {
            RebuildLineRenderers();
            pendingRebuild = false;
        }

        RefreshAllLines();
        foreach (SkillNode node in skillNode.previous) {
            if (node == null) return;
            node.skillNodeLineUpdater.RefreshAllLines();
        }
    }

    public void MarkDirty(SkillNode node) {
        skillNode = node;
        pendingRebuild = true;
    }

    private void CacheReference() {
        if (skillNode == null)
            skillNode = GetComponent<SkillNode>();
    }

    private void RebuildLineRenderers() {
        CleanupMissingReferences();

        List<GameObject> toDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child == null)
                continue;

            if (child.GetComponent<UILineRenderer>() != null)
                toDelete.Add(child.gameObject);
        }

        lineRenderers.Clear();

        for (int i = 0; i < toDelete.Count; i++) {
            GameObject go = toDelete[i];
            if (go == null)
                continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(go);
            else
                Destroy(go);
#else
        Destroy(go);
#endif
        }

        if (skillNode == null)
            return;

        for (int i = 0; i < skillNode.next.Count; i++) {
            if (skillNode.next[i] == null)
                continue;

            lineRenderers.Add(CreateLineRenderer(i));
        }
    }

    public void RefreshAllLines() {
        if (skillNode == null)
            return;

        CleanupMissingReferences();

        int count = 0;
        foreach (SkillNode toNode in skillNode.next) {
            if (toNode == null)
                continue;

            if (count >= lineRenderers.Count)
                break;

            RefreshSingleLine(skillNode, toNode, count);
            count++;
        }
    }

    private void RefreshSingleLine(SkillNode fromNode, SkillNode toNode, int index) {
        if (fromNode == null || toNode == null)
            return;

        if (fromNode.rectTransform == null || toNode.rectTransform == null)
            return;

        UILineRenderer line = lineRenderers[index];
        if (line == null)
            return;

        RectTransform lineParent = line.rectTransform.parent as RectTransform;
        if (lineParent == null)
            return;

        Vector2 startPos = GetWorldPointToLocal(lineParent, GetBottomCenterWorld(fromNode.rectTransform));
        Vector2 endPos = GetWorldPointToLocal(lineParent, GetTopCenterWorld(toNode.rectTransform));

        float midY = (startPos.y + endPos.y) * 0.5f;

        Vector2[] points = new Vector2[]
        {
            startPos,
            new Vector2(startPos.x, midY),
            new Vector2(endPos.x, midY),
            endPos
        };

        line.Points = points;
        line.SetAllDirty();
    }

    private UILineRenderer CreateLineRenderer(int index) {
        GameObject go = new GameObject($"LineRenderer_{index}", typeof(RectTransform));
        go.transform.SetParent(transform, false);

        var line = go.AddComponent<UILineRenderer>();
        line.raycastTarget = false;
        line.LineThickness = 5f;
        line.RelativeSize = false;
        line.LineList = false;

        RectTransform rt = line.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        return line;
    }

    private void CleanupMissingReferences() {
        for (int i = lineRenderers.Count - 1; i >= 0; i--) {
            if (lineRenderers[i] == null)
                lineRenderers.RemoveAt(i);
        }
    }

    private Vector3 GetBottomCenterWorld(RectTransform rt) {
        return rt.TransformPoint(new Vector3(0f, rt.rect.yMin, 0f));
    }

    private Vector3 GetTopCenterWorld(RectTransform rt) {
        return rt.TransformPoint(new Vector3(0f, rt.rect.yMax, 0f));
    }

    private Vector2 GetWorldPointToLocal(RectTransform targetSpace, Vector3 worldPos) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetSpace,
            RectTransformUtility.WorldToScreenPoint(null, worldPos),
            null,
            out Vector2 localPoint
        );
        return localPoint;
    }
}