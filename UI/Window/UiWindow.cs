using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UiWindow : RuntimeUi
{
    private RectTransform rectTrans;
    protected virtual void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    protected virtual void OnEnable()
    {
        rectTrans.transform.SetParent(UiManager.NowBook.transform, false);
        rectTrans.sizeDelta = new Vector2(Screen.width, Screen.height);
        rectTrans.transform.localPosition = Vector3.zero;
    }
  
}
