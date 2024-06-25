using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArPublicPanel_Static : StaticUi
{
    public RawImage screenShotImage;
    public Toggle shadowShow;
    public Button modelToCenterButton;
    public Button screenPhotoCatchButton;
    public Dropdown arMeshDropdown;
    public Dropdown arLightDropdown;
    public Button[] allModelShowButtons;
    public Slider allModelAlphaSlider;

    public Button questionButton;
    public GameObject questionPanel;

    public Sprite[] viewButtonSprites;
    public Button switchAllModelAlphaButton;

    

    private void Start()
    {
        switchAllModelAlphaButton.image.sprite = viewButtonSprites[0];
        GameManager.arManager.SetAllModelAlpha(1);
        switchAllModelAlphaButton.onClick.AddListener(() =>
        {
            float aimValue = switchAllModelAlphaButton.image.sprite == viewButtonSprites[0] ? 0 : 1;
            GameManager.arManager.SetAllModelAlpha(aimValue);
            if (aimValue == 1)
            {
                switchAllModelAlphaButton.image.sprite = viewButtonSprites[0];
            }
            else
            {
                switchAllModelAlphaButton.image.sprite = viewButtonSprites[1];
            }
        });

        questionButton.onClick.AddListener(() =>
        {

            questionPanel.SetActive(questionPanel.activeSelf == true ? false : true);
        });

        screenPhotoCatchButton.onClick.AddListener(() =>
        {
            screenPhotoCatchButton.interactable = false;
            Action onShootOver = () => { screenPhotoCatchButton.interactable = true; };
            StartCoroutine(ScreenShotTool.ScreenShoot((sprite) =>
            {
                ImageTipData data = new ImageTipData("��ͼ�ѱ���", sprite);
                GameManager.uiManager.ShowUiWindow<ImageTip_Window>(data);
                screenPhotoCatchButton.interactable = true;
            }));
        });

        shadowShow.onValueChanged.AddListener((show) =>
        {
            if (show == true)
            {
                GameManager.arManager.targetLight.shadows = LightShadows.Soft;
            }
            else
            {
                GameManager.arManager.targetLight.shadows = LightShadows.None;
            }
        });

        shadowShow.isOn = true;

        modelToCenterButton.onClick.AddListener(() =>
        {
            GameManager.arManager.SetSceneModelToCentre();
        });

        List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("��ʾAR����"),
            new Dropdown.OptionData("����AR����"),
            new Dropdown.OptionData("��ʵAR����")
        };
        arMeshDropdown.AddOptions(datas);
        arMeshDropdown.onValueChanged.AddListener((index) =>
        {
            GameManager.arManager.SetTrackingMeshType((TrackingMeshType)index);
        });
        arMeshDropdown.value = 1;

        datas = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("�����Դ"),
            new Dropdown.OptionData("��ʵ��Դ")
        };
        arLightDropdown.AddOptions(datas);
        arLightDropdown.onValueChanged.AddListener((index) =>
        {
            GameManager.arManager.SwitchLight(index == 1 ? true : false);
        });

        arLightDropdown.value = 0;




        allModelAlphaSlider.onValueChanged.AddListener((value) =>
        {
            GameManager.arManager.SetAllModelAlpha(value);
        });

        allModelShowButtons[0].onClick.AddListener(() =>
        {
            GameManager.arManager.SwitchShowSceneModelEntity(true);
        });
        allModelShowButtons[1].onClick.AddListener(() =>
        {
            GameManager.arManager.SwitchShowSceneModelEntity(false);
        });


        
    }


    public void LockArMeshDropdown(bool isLock)
    {
        arMeshDropdown.interactable = !isLock;
        if (isLock == false)
        {
            arMeshDropdown.onValueChanged.Invoke(arMeshDropdown.value);
        }
    }



}
