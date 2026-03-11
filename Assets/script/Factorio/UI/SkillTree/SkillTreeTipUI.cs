using TMPro;
using UnityEngine;

public class SkillTooltipUI : MonoBehaviour {
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f);

    private void Awake() {
    }

    public void Show(string title, string desc) {
        //titleText.text = title;
        //descText.text = desc;
        panel.gameObject.SetActive(true);
    }

    public void Hide() {
        panel.gameObject.SetActive(false);
    }
}