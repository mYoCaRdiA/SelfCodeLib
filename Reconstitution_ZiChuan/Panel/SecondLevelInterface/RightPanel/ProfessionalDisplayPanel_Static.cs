using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfessionalDisplayPanel_Static : StaticUi
{
    public Button btnReset;

    public Transform canvasLoaction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        canvasLoaction = this.transform.parent.transform;
        
        btnReset.onClick.AddListener(() => {
            Debug.Log("÷ÿ÷√Õ∏√˜∂»");
        });

    }

    // Update is called once per frame
    void Update()
    {

    }
}
