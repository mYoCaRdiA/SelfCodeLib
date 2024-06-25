using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class MeasureTypePanel_Static : StaticUi
{
    public Button btnGeneralMeasure;
    public Button btnClearDistance;
    public Button btnClearHeight;
    public Button btnCoord;

    public Transform canvasLoaction { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        canvasLoaction = this.transform.parent.transform;
        btnGeneralMeasure.onClick.AddListener(() => { PanelManager.instance.OpenThirdPanel(UIPanelName.GeneralMeasurePanel, canvasLoaction); });
        btnClearDistance.onClick.AddListener(() => { PanelManager.instance.OpenThirdPanel(UIPanelName.ClearDistancePanel, canvasLoaction); });
        btnClearHeight.onClick.AddListener(() => { PanelManager.instance.OpenThirdPanel(UIPanelName.ClearHeightPanel, canvasLoaction); });
        btnCoord.onClick.AddListener(() => { PanelManager.instance.OpenThirdPanel(UIPanelName.CoordPanel, canvasLoaction); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
