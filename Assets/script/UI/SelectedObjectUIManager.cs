using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObjectUIManager : MonoBehaviour
{
    public GameObject select_object;

    public Image image;
    protected Vector3 offset;
    public LineRenderer lineRenderer;
    public Transform resource_transform;

    
    // Start is called before the first frame update
    void Start() {
       
        offset = new Vector3(0,1.5f,0);
        lineRenderer.positionCount = 2;
        //lineRenderer.transform.position = object_transform.position;
        lineRenderer.SetPosition(0, new Vector3());
        lineRenderer.SetPosition(1, new Vector3() + offset);

        List<Resource> res = select_object.GetComponent<SelectableObject>().resources;

        for (int i = 0; i < res.Count; i += 1) {
            GameObject imgobject = new GameObject("resource_img");
            GameObject textobject = new GameObject("resource_num");
            Image img = imgobject.AddComponent<Image>();
            Text text = textobject.AddComponent<Text>();

            img.sprite = res[i].info_image;
            text.text = "x " + res[i].number;
            text.fontSize = 30;
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.alignment = TextAnchor.MiddleLeft;
            RectTransform irt =  imgobject.GetComponent<RectTransform>();
            irt.SetParent(resource_transform);
            irt.sizeDelta = new Vector2(20.0f, 20.0f);
            irt.localPosition = new Vector3(-50 , 0 - 40*i);

            RectTransform trt = textobject.GetComponent<RectTransform>();
            trt.SetParent(resource_transform);
            trt.pivot = new Vector3(0, 0.5f, 0);
            trt.sizeDelta = new Vector2(150.0f, 40.0f);
            trt.localPosition = new Vector3(0 , 0 - 40*i);
            trt.localScale = new Vector3(1, 1, 1);

        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        image.transform.position = UnityEngine.Camera.main.WorldToScreenPoint(select_object.transform.position + offset); 
        //transform.rotation = Quaternion.FromToRotation(Vector3.forward, transform.position - cam.transform.position);
    }
}
