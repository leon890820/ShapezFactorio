using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


abstract public class SelectableObject : MonoBehaviour
{
    public Sprite sprite;
    public GameObject canvas;
    public Vector2Int voxel_side = new Vector2Int(1,1);

    protected GenerateWorld world;
    public List<Resource> resources = new List<Resource>();
   

    private void Start() {
        world = GameObject.Find("world").GetComponent<GenerateWorld>();
    }
    public Sprite GetInfoSprite() {
        return sprite;
    }
    public void SetCanvasEnable(bool b) {
        if (!canvas) return;
        canvas.SetActive(b);
    }
    public void PutObjectOnTheChunk(Vector3 p ,GameObject go) {
        for (int i = 0; i < voxel_side.y; i += 1) {
            for (int j = 0; j < voxel_side.x; j += 1) {
                world.AddObjectToChunk(p + new Vector3(j,0,i),go);
            }
        }

    }

    virtual public void OnClick() { 
    
    }
    
    
    
}
