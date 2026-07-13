using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillNodeHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private SkillTooltipUI tooltipUI;

    public void OnPointerEnter(PointerEventData eventData) {
        tooltipUI.Show("", "");
    }

    public void OnPointerExit(PointerEventData eventData) {
        tooltipUI.Hide();
    }
}