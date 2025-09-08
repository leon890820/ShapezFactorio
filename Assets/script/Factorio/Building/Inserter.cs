using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inserter : FactorioPlatformBuilding
{
    public Animator animator;
    public AnimationClip clip;
    public Transform grabberTransform;


    private FactorioGameObjectBase resource;
    bool isAniamted = false;
    // Start is called before the first frame update


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Run() {
        if (bluePrintMode) return;

        if (resource) { 
            resource.transform.position = grabberTransform.position;
        }

        if (isAniamted) return;
        if (!resource) {
            if (TryGrab()) {
                isAniamted = true;
                PlayHalfThenStop();
            }


        } else {
            
            if (TryInsert()) {
                isAniamted = true;
                PlayFromTime("CINEMA_4D_Main", 1f);
            }
        
        }

    }

    private bool TryGrab() {
        FactorioPlatformBuilding fpb = playGroundPlatform.GetBuilding(this, FactorioData.direction[(rotation + 1) % 4]);
        FactorioGameObjectBase factorioResource = fpb?.TryBeGrab();
        if (factorioResource) {
            resource = factorioResource;
            return true;
        }
        return false;
    }

    private bool TryInsert() {
        Vector3Int dir = FactorioData.direction[(rotation + 3) % 4];
        Vector3Int pos = playGroundPlatform.GetLocalPositions(transform.position) + dir;
        FactorioPlatformBuilding fpb = playGroundPlatform.GetBuilding(this, dir);
        if (!fpb) return false;
        if (fpb.TryInput(resource, pos, 0, true)) {            
            resource = null;
            return true;
        }
        return false;
    }


    public override void SetPosition(Vector3 pos) {
        pivotTransform.position = Floor(pos) + new Vector3(0.5f,0f,0.5f);
    }


    public void PlayHalfThenStop() {
        animator.Play("CINEMA_4D_Main", 0, 0f);
        animator.speed = 1f;
        StartCoroutine(StopAfterTime(1.0f));
    }

    private IEnumerator StopAfterTime(float duration) {
        yield return new WaitForSeconds(duration);
        animator.speed = 0f; // 停止播放
        isAniamted = false;
    }


    public void PlayFromTime(string stateName, float timeInSeconds) {
        float normalized = Mathf.Clamp01(timeInSeconds / clip.length);
        animator.speed = 1f;
        animator.Play(stateName, 0, normalized);
        animator.Update(0f); // 立即套用狀態（重要）
        StartCoroutine(StopAfterTime(1.0f));
    }

    public override FactorioPrefabBaseObject Clone() {
        return PrefabManager.Instance.GetPrefab("Inserter");
    }

}
