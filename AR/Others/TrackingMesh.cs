using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingMesh : MonoBehaviour
{

    private void Start()
    {
       MeshRenderer meshRender= transform.GetComponent<MeshRenderer>();
        if(GameManager.arManagerClone == null)
        {
            meshRender.material = GameManager.arManager.trackingMeshMat;
        }
        else
        {
            meshRender.material = GameManager.arManagerClone.trackingMeshMat;
        }
        
    }
}
