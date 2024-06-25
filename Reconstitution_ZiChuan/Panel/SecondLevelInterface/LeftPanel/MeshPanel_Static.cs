using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class MeshPanel_Static : StaticUi
{
    public Button btnOpenMesh;
    public Button btnCloseMesh;
    public Button btnOcclusion;
    

    public Transform canvasLoaction { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        canvasLoaction = this.transform.parent.transform;
        btnOpenMesh.onClick.AddListener(() => { GameObject.Find("AR Session Origin").GetComponent<ArManager>().SetTrackingMeshType(0); ; });
        btnCloseMesh.onClick.AddListener(() => { GameObject.Find("AR Session Origin").GetComponent<ArManager>().SetTrackingMeshType(2); ; });
        btnOcclusion.onClick.AddListener(() => { GameObject.Find("AR Session Origin").GetComponent<ArManager>().SetTrackingMeshType(1); ; });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
