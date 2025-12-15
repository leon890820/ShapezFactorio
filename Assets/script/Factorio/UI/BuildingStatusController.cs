using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStatusController : MonoBehaviour{
    public SpriteRenderer statusIconRenderer;
    public Transform buildingTransform;
    public AlertIcon alertIcon;

    public float height = 2f;     // vertical offset
    public float forwardOffset = 2f;  // distance toward the camera
    public float blinSpeed = 5.0f;

    private void Update() {        
        transform.forward = Camera.main.transform.forward;
        Vector3 dir = (Camera.main.transform.position - buildingTransform.position).normalized;
        transform.localPosition = dir * forwardOffset;

        BlinTexture();
    }

    public void SetAlertIcon(BuildStatus status) {
        switch (status) { 
            case BuildStatus.Working:
                statusIconRenderer.sprite = null;
                statusIconRenderer.enabled = false;
                break;
            case BuildStatus.NoInput:
                statusIconRenderer.sprite = alertIcon.items[1].sprite;
                statusIconRenderer.enabled = true;
                break;
            case BuildStatus.NoPower:
                statusIconRenderer.sprite = alertIcon.items[0].sprite;
                statusIconRenderer.enabled = true;
                break;
            default:
                statusIconRenderer.sprite = null;
                statusIconRenderer.enabled = false;
                break;
        }    
    }

    public void BlinTexture() {
        if (statusIconRenderer == null)
            return;

        if (statusIconRenderer.sprite != null) {
            float t = (Mathf.Sin(Time.time * blinSpeed) + 1f) * 0.5f;
            float a = Mathf.Lerp(0.1f, 1.0f, t);

            Color c = statusIconRenderer.color;
            c.a = a;
            statusIconRenderer.color = c;
        } else {
            Color c = statusIconRenderer.color;
            c.a = 0f;
            statusIconRenderer.color = c;
        }
    }

}
