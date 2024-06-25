using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PictureLocatePage_Static;

public class ArImageUi_Element : UiElement
{
    public Button locateButton;
    public Button uploadButton;
    public Button clearButton;
    public Text infoText;

    public Transform zuoShang;
    public Transform youXia;


    public float GetRealHeight()
    {
        Vector3 point1 = zuoShang.localPosition;
        Vector3 point2 = new Vector3(zuoShang.localPosition.x, youXia.localPosition.y, youXia.localPosition.z);
        point1=zuoShang.parent.TransformPoint(point1);
        point2 = zuoShang.parent.TransformPoint(point2);
        return Vector3.Distance(point1,point2);
    }

    public float GetRealWidth()
    {
     
        Vector3 point1 = zuoShang.localPosition;
        Vector3 point2 = new Vector3(youXia.localPosition.x, zuoShang.localPosition.y, youXia.localPosition.z);
        point1 = zuoShang.parent.TransformPoint(point1);
        point2 = zuoShang.parent.TransformPoint(point2);
        return Vector3.Distance(point1, point2);
    }

    //private void Awake()
    //{
    //    Action locate = () =>
    //    {
    //        Debug.Log("11111111");
    //    };


    //    Action update = () =>
    //    {
    //        ConfirmTip_Window.IniData iniData = new ConfirmTip_Window.IniData("是否确定更新定位图（"  + "）的数据？", new Action(() =>
    //        {
    //            GameManager.uiManager.ShowUiWindow<Tip_Window>("222222222222");
    //        }));
    //        GameManager.uiManager.ShowUiWindow<ConfirmTip_Window>(iniData);

    //    };


    //    Action clear = () =>
    //    {
    //        Debug.Log("33333333333");
    //    };

    //    SetButtonEvents(locate, update, clear);
    //}

    public void SetButtonEvents(Action locate, Action upload, Action clear)
    {
        locateButton.onClick.RemoveAllListeners();
        uploadButton.onClick.RemoveAllListeners();
        clearButton.onClick.RemoveAllListeners();

        locateButton.onClick.AddListener(() =>
        {
            locate.Invoke();
        });

        uploadButton.onClick.AddListener(() =>
        {
            upload.Invoke();
        });

        clearButton.onClick.AddListener(() =>
        {
            clear.Invoke();
        });
    }

}
