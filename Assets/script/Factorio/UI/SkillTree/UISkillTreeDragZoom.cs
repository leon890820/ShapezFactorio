using UnityEngine;
using UnityEngine.EventSystems;

public class UISkillTreeDragZoom : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler {
    [Header("References")]
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private Camera uiCamera;

    [Header("Drag")]
    [SerializeField] private bool limitPosition = false;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private float positionSmoothTime = 0.08f;

    [Header("Inertia")]
    [SerializeField] private bool enableInertia = true;
    [SerializeField] private float inertiaDamping = 6f;   // 越大停越快
    [SerializeField] private float inertiaMultiplier = 1f;
    [SerializeField] private float minInertiaSpeed = 10f; // 小於這個就停止

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;
    [SerializeField] private float zoomSmoothTime = 0.08f;

    private Vector2 lastPointerPosition;

    private Vector2 targetPosition;
    private Vector2 positionVelocity;

    private float targetScale;
    private float scaleVelocity;

    private bool isDragging;
    private Vector2 dragVelocity;
    private Vector2 inertiaVelocity;

    private void Awake() {
        targetPosition = content.anchoredPosition;
        targetScale = content.localScale.x;
    }

    private void Update() {
        if (!isDragging && enableInertia) {
            if (inertiaVelocity.sqrMagnitude > minInertiaSpeed * minInertiaSpeed) {
                targetPosition += inertiaVelocity * Time.unscaledDeltaTime;
                inertiaVelocity = Vector2.Lerp(
                    inertiaVelocity,
                    Vector2.zero,
                    inertiaDamping * Time.unscaledDeltaTime
                );

                ClampPosition();
            } else {
                inertiaVelocity = Vector2.zero;
            }
        }

        content.anchoredPosition = Vector2.SmoothDamp(
            content.anchoredPosition,
            targetPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        float scale = Mathf.SmoothDamp(
            content.localScale.x,
            targetScale,
            ref scaleVelocity,
            zoomSmoothTime
        );

        content.localScale = new Vector3(scale, scale, 1f);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        isDragging = true;
        inertiaVelocity = Vector2.zero;
        dragVelocity = Vector2.zero;
        lastPointerPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 currentPointerPosition = eventData.position;
        Vector2 delta = currentPointerPosition - lastPointerPosition;

        float dt = Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        dragVelocity = delta / dt;

        targetPosition += delta;
        lastPointerPosition = currentPointerPosition;

        ClampPosition();
    }

    public void OnEndDrag(PointerEventData eventData) {
        isDragging = false;

        if (enableInertia)
            inertiaVelocity = dragVelocity * inertiaMultiplier;
    }

    public void OnScroll(PointerEventData eventData) {
        float scrollValue = eventData.scrollDelta.y;
        if (Mathf.Approximately(scrollValue, 0f))
            return;

        float oldTargetScale = targetScale;
        float newScale = Mathf.Clamp(oldTargetScale + scrollValue * zoomSpeed, minScale, maxScale);

        if (Mathf.Approximately(oldTargetScale, newScale))
            return;

        Camera cam = eventData.pressEventCamera ?? uiCamera;

        Vector2 localPointBefore;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content,
            eventData.position,
            cam,
            out localPointBefore
        );

        Vector3 originalScale = content.localScale;
        content.localScale = new Vector3(newScale, newScale, 1f);

        Vector2 localPointAfter;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content,
            eventData.position,
            cam,
            out localPointAfter
        );

        content.localScale = originalScale;

        Vector2 localPointDelta = localPointAfter - localPointBefore;
        targetPosition += localPointDelta * newScale;

        targetScale = newScale;

        ClampPosition();
    }

    private void ClampPosition() {
        if (!limitPosition)
            return;

        Vector2 pos = targetPosition;
        pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
        targetPosition = pos;

        if (targetPosition.x == minPosition.x || targetPosition.x == maxPosition.x)
            inertiaVelocity.x = 0f;

        if (targetPosition.y == minPosition.y || targetPosition.y == maxPosition.y)
            inertiaVelocity.y = 0f;
    }
}