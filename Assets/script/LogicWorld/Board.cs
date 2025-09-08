using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Board : SelectableObject
{
    public Transform boardAxisTransform;
    private bool turn;
    private bool animate;
    private float timeCount = 0.0f;
    private float animation_speed = 5.0f;
    public bool change = false;

    private Quaternion from;
    private Quaternion to;
    // Start is called before the first frame update
    void Start(){
        turn = false;
        animate = false;

        from = Quaternion.Euler(-40,0,0);
        to = Quaternion.Euler(40,0,0);
    }

    // Update is called once per frame
    void Update(){
        if (animate) {
            boardAxisTransform.localRotation = turn ? Quaternion.Slerp(from, to, timeCount) : Quaternion.Slerp(to, from, timeCount);
            timeCount += Time.deltaTime* animation_speed;
            if (timeCount >= 1.0f) {
                timeCount = 0.0f;
                animate = false;
            }
        }
    }

    public override void OnClick() {
        turn = !turn;
        animate = true;
        change = true;
    }

    public bool GetTurn() {
        return turn;
    }
}
