using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class LocateTypePanel_Static : MonoBehaviour
{ 
    public Button btnHandLocate;
    public Button btnFineTuning;
    public Transform canvasLoaction { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        canvasLoaction = this.transform.parent.transform;
        btnHandLocate.onClick.AddListener(() => { PanelManager.instance.OpenThirdPanel("TwoPointLocatePage", canvasLoaction); });
        btnFineTuning.onClick.AddListener(() => { PanelManager.instance.OpenThirdPanel("ManualLocatePage", canvasLoaction); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
