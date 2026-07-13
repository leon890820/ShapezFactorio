using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PhysicWire : MonoBehaviour{
    public Transform startPosition;
    public Transform endPosition;

    public bool fixedStart = true;
    public bool fixedEnd = true;

    public float stiffness = 1f;
    public float damping = 0.99f;
    public float wireLength = 6f;

    public int numberPoints = 10;
    



    private Vector3[] pointsPosition;
    private Vector3[] pointsVelocity;
    private Vector3[] pointsForce;
    private Vector3 gravity = new Vector3(0, -4.8f, 0);
    private float mass;

    private LineRenderer lineRenderer;

    private void Start() {
        Initialized();
    }

    private void OnValidate() {
        Initialized();
    }

    private void Initialized() {
        if (startPosition == null || endPosition == null) { 
            Debug.LogError("PhysicWire: StartPosition or EndPosition is null.");
            return;
        }

        if(!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();

        pointsPosition = new Vector3[numberPoints + 2];
        pointsVelocity = new Vector3[numberPoints + 2];
        pointsForce = new Vector3[numberPoints + 2];
        for (int i = 0; i < pointsPosition.Length; i++) {
            pointsPosition[i] = Vector3.Lerp(startPosition.position, endPosition.position, (float)i / (numberPoints + 1f));
            pointsVelocity[i] = Vector3.zero;
        }
        mass = 1.0f / ((float)numberPoints + 2f);
    }

    private void FixedUpdate() {
        if (startPosition == null || endPosition == null) {
            Debug.LogError("PhysicWire: StartPosition or EndPosition is null.");
            return;
        }
        pointsPosition[0] = startPosition.position ;
        pointsPosition[^1] = endPosition.position ;

        CalcPhysic();
    }

    void CalcPhysic() {
        ClearFoece();
        ApplyGravity();
        ApplyWireForce();
        UpdatePositionsAndVelocity();
        UpdateLineRenderPoints();
    }

    void ClearFoece() {
        for (int i = 0; i < pointsForce.Length; i++) {
            pointsForce[i] = Vector3.zero;
        }
    }

    void ApplyGravity() {
        for (int i = 1; i < pointsPosition.Length - 1; i++) {
            pointsForce[i] =  mass * gravity;
        }
        if(!fixedStart) pointsForce[0] = mass * gravity;
        if(!fixedEnd) pointsForce[^1] = mass * gravity;
    }

    void ApplyWireForce() {

        float restLength = wireLength / (numberPoints + 1f);
        for (int i = 1; i < pointsForce.Length - 1; i++) {
            pointsForce[i] += ComputeTensionForce(pointsPosition[i],pointsPosition[i - 1],restLength,stiffness);
            pointsForce[i] += ComputeTensionForce(pointsPosition[i], pointsPosition[i + 1], restLength, stiffness);
        }
        if(!fixedStart) {
            pointsForce[0] += ComputeTensionForce(pointsPosition[0], pointsPosition[1], restLength, stiffness);
        }
        if (!fixedEnd) {
            pointsForce[^1] += ComputeTensionForce(pointsPosition[^1], pointsPosition[^2], restLength, stiffness);
        }

    }

    private Vector3 ComputeTensionForce(Vector3 pCurrent,Vector3 pOther,float restLength,float stiffness) {
        float currentDist = Vector3.Distance(pCurrent, pOther);
        float stretch = Mathf.Max(0f, currentDist - restLength);
        if (stretch <= 0f)
            return Vector3.zero;

        float tension = stiffness * stretch;
        Vector3 dir = (pOther - pCurrent).normalized;

        return tension * dir;  // 對 current 施加的力
    }

    void UpdatePositionsAndVelocity() {
        for (int i = 0; i < pointsVelocity.Length; i++) { 
            pointsVelocity[i] += (pointsForce[i] / mass) * Time.fixedDeltaTime;
            pointsVelocity[i] *= damping;
        }
        for (int i = 0; i < pointsPosition.Length; i++) { 
            pointsPosition[i] += pointsVelocity[i] * Time.fixedDeltaTime;
        }

        startPosition.position = pointsPosition[0];
        endPosition.position = pointsPosition[^1];

    }

    void UpdateLineRenderPoints() {
        lineRenderer.positionCount = pointsPosition.Length;

        for (int i = 0; i < pointsPosition.Length; i++) {
            lineRenderer.SetPosition(i, pointsPosition[i]);
        }
    }

}
