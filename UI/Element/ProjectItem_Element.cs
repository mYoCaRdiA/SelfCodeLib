using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectItem_Element : UiElement
{

    public Text title;
    public Text ProjectType;
    public Text Amount;
    public Text StartTime;
    public Text MonomerNum;
    public Text StageName;

    public Button button;

    public Action onButtonClick;

    private void Awake()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onButtonClick.Invoke();
        });
    }


    // 函数用于为所有 Text 字段赋值
    public void SetAllTextValues(string titleValue, string projectTypeValue, string amountValue, string startTimeValue, string monomerNumValue, string stageNameValue)
    {
        title.text = titleValue;
        ProjectType.text = projectTypeValue;
        Amount.text = amountValue;
        StartTime.text = startTimeValue;
        MonomerNum.text = monomerNumValue;
        StageName.text = stageNameValue;
    }
}
