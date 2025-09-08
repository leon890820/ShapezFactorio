using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireLine{
    public Vector3 start_position;
    public Vector3 end_position;
    public GameObject sphere1;
    public GameObject sphere2;
    public GameObject cylinder;
    public GameObject gameLocation;
    
    public WireLine(Transform a,Transform b, Transform p,Material m) { 
        start_position = a.position + Matrix4x4.Rotate(a.rotation).MultiplyPoint3x4(Vector3.up) * a.localScale.y;
        end_position = b.position + Matrix4x4.Rotate(b.rotation).MultiplyPoint3x4(Vector3.up) * b.localScale.y;

;

        gameLocation = new GameObject();
        gameLocation.name = b.parent.name;
        gameLocation.transform.parent = p;
        if (cylinder == null) { 
            cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.parent = gameLocation.transform;
            cylinder.GetComponent<CapsuleCollider>().enabled = false;
            cylinder.transform.position = (start_position + end_position) / 2.0f;
            Vector3 dis = end_position - start_position;
            float angle = Mathf.Atan2(dis.z, dis.x);
            float sa = Mathf.Asin(dis.y / dis.magnitude);
            cylinder.transform.rotation = Quaternion.Euler(0.0f, -angle * 180.0f/Mathf.PI, 90.0f +  sa*180f/Mathf.PI);
            cylinder.transform.localScale = new Vector3(0.2f, (dis.magnitude)*0.5f, 0.2f);
            cylinder.GetComponent<MeshRenderer>().material = m;
        }

        if (sphere1 == null) {
            sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere1.transform.parent = gameLocation.transform;
            sphere1.GetComponent<SphereCollider>().enabled = false;
            sphere1.transform.position = start_position ;
            sphere1.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
            sphere1.GetComponent<MeshRenderer>().material = m;
        }

        if (sphere2 == null) {
            sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere2.transform.parent = gameLocation.transform;
            sphere2.GetComponent<SphereCollider>().enabled = false;
            sphere2.transform.position = end_position ;
            sphere2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            sphere2.GetComponent<MeshRenderer>().material = m;
        }
    }

   


   
}
