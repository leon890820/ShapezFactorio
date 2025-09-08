using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : SelectableObject {
    private Vector3 velocity = new Vector3(0,0,0);
    private float orientation = 0.0f;
    private bool ismove = false;
    private Animator animator;
    private string NPCname;
    private STATUS status;
    
    void Start() {
        animator = GetComponent<Animator>();
        status = STATUS.IDLE;
    }

    // Update is called once per frame
    void Update() {

        transform.position+=velocity*Time.deltaTime;

        ismove = velocity.sqrMagnitude != 0;
        animator.SetBool("walk", ismove);

    }
    public STATUS GetStatus() {
        return status;
    }



}

public enum STATUS { 
    IDLE,
    WORKING
}

public class Backpad {
    public Resource[] resources;
    public int MAX;
    public Backpad(int _MAX = 10) { 
        MAX = _MAX;
    }



}
