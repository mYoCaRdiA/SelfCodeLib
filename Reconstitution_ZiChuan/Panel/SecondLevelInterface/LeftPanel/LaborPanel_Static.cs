using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class LaborPanel_Static : MonoBehaviour
{
    public Button btnAmount;
    public Button btnFace;

    public Transform canvasLoaction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        canvasLoaction = this.transform.parent.transform;

        btnAmount.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenThirdPanel(UIPanelName.AmountPanel, canvasLoaction);
            PanelManager.instance.OpenAnotherPanel("FaceDetectPage", canvasLoaction);



        });
        btnFace.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenThirdPanel(UIPanelName.FacePanel, canvasLoaction);
            PanelManager.instance.OpenAnotherPanel("FaceDetectPage", canvasLoaction);
        });

    }

    // Update is called once per frame
    void Update()
    {

    }


}
