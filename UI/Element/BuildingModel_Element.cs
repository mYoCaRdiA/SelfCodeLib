using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingModel_Element : UiElement
{
    public Text nameText;
    public Text timeText;
    public Button button;
    public Action onClick;
    public Action onPointDown;
    public ModelInfoData.LoadPathItem modelInfoItem;
    public GameObject seletedFlag;
    public Image downloadImage;
    public Image readyImage;
    public Slider slider;
    public Button stop;
    private void Awake()
    {

        button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
        });
        
    }

    private void OnEnable()
    {
       // seletedFlag.SetActive(false);   
    }
    private void OnDisable()
    {
        seletedFlag.SetActive(false);
    }

    public void SetNameAndTime(string name, string time)
    {
        nameText.text = name;
        timeText.text = time;
    }
}
