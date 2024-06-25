using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ScanPanel_Static : StaticUi
{
    public Transform scanLine;
    private RectTransform rootRect;

    private float height;
    private float lineHeight;

    private Tweener tweener;
    public Transform canvasLoaction { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        canvasLoaction = this.transform.parent.transform;
        rootRect = transform.GetComponent<RectTransform>();
        height = rootRect.sizeDelta.y;
        lineHeight = scanLine.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable()
    {
        scanLine.localPosition = new Vector3(0, height / 2, 0);
        tweener = scanLine.DOLocalMove(new Vector3(0, -height / 2 + lineHeight / 2, 0), 5.0f).SetLoops(-1).SetEase(Ease.Linear).OnStepComplete(() =>
        {
            scanLine.localPosition = new Vector3(0, height / 2, 0);
        });

        
    }
    private void OnDisable()
    {
        

        tweener.Kill();
    }
}
