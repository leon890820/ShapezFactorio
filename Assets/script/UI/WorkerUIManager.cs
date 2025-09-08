using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUIManager : SelectedObjectUIManager
{
    public Text statustext;
    public Button add_job;
    public Button cancle_job;
    public GameObject selectjob1;
    
    private NPC npc;
    
    
    private void Start() {
        
        npc = select_object.GetComponent<NPC>();
        offset = new Vector3(0, 1.0f, 0);
        gameObject.SetActive(false);
        add_job.onClick.AddListener(AddJob);
    }

    private void Update() {
        image.transform.position = UnityEngine.Camera.main.WorldToScreenPoint(select_object.transform.position + offset);
        statustext.text = "Status : " + npc.GetStatus().ToString();

    }

  

    private void AddJob() {
        add_job.gameObject.SetActive(false);
        cancle_job.gameObject.SetActive(true);
        selectjob1.SetActive(true);
    }
}
