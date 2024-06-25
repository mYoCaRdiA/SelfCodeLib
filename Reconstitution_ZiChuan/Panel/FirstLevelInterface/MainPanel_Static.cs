using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Outline = QuickOutline.Outline;
public class MainPanel_Static : MonoBehaviour
{
    [Header("右侧按钮")]
    public Button btnQuestion;
    public Button btnAttribute;
    public Toggle modelAlphaToggle;
    public Button btnProfessionalDisplay;
    public Button[] allModelShowButtons;
    public Slider allModelAlphaSlider;

    [Header("左侧按钮")]
    public Button btnLocate;
    public Button btnMeasure;
    public Button btnLabor;
    public Button btnClassifyClarity;


    [Header("顶部按钮")]
    public Button btnSetting;
    public Button btnPhoto;
    public Button btnScanQRCodes;
    public Button btnRefresh;
    public Button btnBack;

    public Text modelName;
    public Dropdown arMeshDropdown;
    public Dropdown arLightDropdown;
    public Toggle openShadowToggle;


    

    [Header("其他")]
    public Material material;
    public Outline.Mode oulineMode = Outline.Mode.OutlineAll;
    public Color outLineColor = Color.yellow;
    public bool modelAlphaToggleNoEventFlag = false;
    Dictionary<string, MainAlphaSlider_Element> mainSliders = new Dictionary<string, MainAlphaSlider_Element>();
    //public static GameObject nowSelectModel;
    public static float allModelAlphaValue = 100;
    public static int realModel;
    public static bool realTwinMode = false;
    private bool previousBool;
    private Light _light;
    public Transform canvasLoaction { get; set; }
    List<Transform> nowSelectModel;
    List<Transform> NowSelectModel
    {
        set
        {
            if (value != nowSelectModel)
            {
                if (nowSelectModel != null)
                {
                    foreach (var item in nowSelectModel)
                    {
                        item.gameObject.GetComponent<Outline>().enabled = false;
                    }

                }
            }

            nowSelectModel = value;
        }
        get { return nowSelectModel; }
    }
    private void OnEnable()
    {
        doubleClickLastTime = 0;
    }


    [Obsolete]
    private void Start()
    {
        MainPanelIni();
        #region 添加监听事件

        //右侧
        btnQuestion.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel(UIPanelName.ProblemRecordPanel);
        });
        btnAttribute.onClick.AddListener(() =>
        {
            OpenAttributePanel();
        });
        modelAlphaToggle.onValueChanged.AddListener((isOn) =>
        {
            ModelAlphaValueChanged(isOn);
        });
        btnProfessionalDisplay.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel(UIPanelName.ProfessionalDisplayPanel, canvasLoaction);
        });
        allModelShowButtons[0].onClick.AddListener(() =>
        {

            GameManager.arManager.SetAllModelAlpha(100);
            allModelAlphaValue = 100;
            allModelShowButtons[0].gameObject.SetActive(false);
            allModelShowButtons[1].gameObject.SetActive(true);
        });
        allModelShowButtons[1].onClick.AddListener(() =>
        {

            GameManager.arManager.SetAllModelAlpha(0);
            allModelAlphaValue = 0;
            allModelShowButtons[1].gameObject.SetActive(false);
            allModelShowButtons[0].gameObject.SetActive(true);
        });
        allModelAlphaSlider.onValueChanged.AddListener((value) =>
        {
            allModelAlphaValue = value;
            GameManager.arManager.SetAllModelAlpha(value);
            if (value == 0)
            {
                allModelShowButtons[1].gameObject.SetActive(false);
                allModelShowButtons[0].gameObject.SetActive(true);
            }
            else
            {
                allModelShowButtons[0].gameObject.SetActive(false);
                allModelShowButtons[1].gameObject.SetActive(true);
            }

        });
        //左侧
        btnLocate.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel(UIPanelName.LocateTypePanel, canvasLoaction); CloseAllHighLight();
        });
        btnMeasure.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel(UIPanelName.MeasureTypePanel, canvasLoaction);
        });
        btnLabor.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel(UIPanelName.LaborPanel, canvasLoaction);
        });
        btnClassifyClarity.onClick.AddListener(() =>
        {
            OpenClassifyClarityPanel();
        });

        //顶部
        btnSetting.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel(UIPanelName.SettingPanel, canvasLoaction);
        });
        btnPhoto.onClick.AddListener(() =>
        {
            btnPhoto.interactable = false;
            Action onShootOver = () => { btnPhoto.interactable = true; };
            StartCoroutine(ScreenShotTool.ScreenShoot((sprite) =>
            {
                ImageTipData data = new ImageTipData("截图已保存", sprite);

                btnPhoto.interactable = true;
            }));
        });
        btnScanQRCodes.onClick.AddListener(() =>
        {
            PanelManager.instance.OpenSecondPanel("PictureLocatePage", canvasLoaction);
        });
        btnRefresh.onClick.AddListener(() =>
        {
            GameManager.arManager.SetSceneModelToCentre();

        });
        btnBack.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("Void");
        });


        openShadowToggle.onValueChanged.AddListener((bool isOn) =>
        {
            _light.shadows = isOn ? LightShadows.Soft : LightShadows.None;
        });
        openShadowToggle.isOn = true;
        List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("显示AR网格"),
            new Dropdown.OptionData("隐藏AR网格"),
            new Dropdown.OptionData("真实AR遮罩")
        };
        arMeshDropdown.AddOptions(datas);
        arMeshDropdown.onValueChanged.AddListener((index) =>
        {
            GameManager.arManager.SetTrackingMeshType((TrackingMeshType)index);
            realModel = index;
        });
        arMeshDropdown.value = 1;
        realModel = arMeshDropdown.value;
        datas = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("虚拟光源"),
            new Dropdown.OptionData("真实光源")
        };
        arLightDropdown.AddOptions(datas);
        arLightDropdown.onValueChanged.AddListener((index) =>
        {
            GameManager.arManager.SwitchLight(index == 1 ? true : false);
        });

        arLightDropdown.onValueChanged.Invoke(arLightDropdown.value);



        #endregion

    }

    [Obsolete]
    private void Update()
    {
        CalledEveryFrame();
    }

    #region 内部方法

    void CalledEveryFrame()
    {
        if (realTwinMode != previousBool)
        {
            HideOrNotComponent(!realTwinMode);
            previousBool = realTwinMode;
        }
        if (realTwinMode)
        {
            CheckMouseSelectTransparent();

        }
        else
        {

            if (GameObject.Find("Left").transform.FindChild("MeasureTypePanel_Static").gameObject.activeSelf == false && GameObject.Find("Left").transform.FindChild("LocateTypePanel_Static").gameObject.activeSelf == false)
            {
                CheckMouseSelect();
            }
        }

        if (NowSelectModel != null)
        {
            /*selectedPage.SetActive(true);
            unSelectedPage.SetActive(false);
            */
            foreach (var item in NowSelectModel)
            {
                Color oringnalColor = item.GetComponent<MeshRenderer>().sharedMaterial.color;
                if (modelAlphaToggle.isOn == true)
                {
                    if (item.GetComponent<MeshRenderer>().sharedMaterial.color.a != 0)
                    {
                        item.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(oringnalColor.r, oringnalColor.g, oringnalColor.b, 0);
                    }
                }
                else
                {
                    if (item.GetComponent<MeshRenderer>().sharedMaterial.color.a == 0)
                    {
                        item.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(oringnalColor.r, oringnalColor.g, oringnalColor.b, GameManager.arManager.NowAllModelAlpha);
                    }
                }
            }
        }
    }
    void HideOrNotComponent(bool HideOrNot)
    {
        if (HideOrNot)
        {
            allModelAlphaSlider.value = 1;
            allModelShowButtons[0].gameObject.SetActive(false);
            allModelShowButtons[1].gameObject.SetActive(true);
        }
        else
        {
            allModelAlphaSlider.value = 0;
            allModelShowButtons[0].gameObject.SetActive(HideOrNot);
            allModelShowButtons[1].gameObject.SetActive(HideOrNot);
        }
        modelAlphaToggle.gameObject.SetActive(HideOrNot);
        btnProfessionalDisplay.gameObject.SetActive(HideOrNot);

        allModelAlphaSlider.gameObject.SetActive(HideOrNot);
        btnClassifyClarity.gameObject.SetActive(HideOrNot);
        btnLocate.gameObject.SetActive(HideOrNot);
    }
    void MainPanelIni()
    {
        previousBool = realTwinMode;
        modelName.text = ModelSelectPage_Static.modelName;
        transform.SetAsLastSibling();
        PanelColor.panelColor = material;
        canvasLoaction = this.transform.parent.transform;
        _light = GameObject.FindWithTag("Light").GetComponent<Light>();
    }
    void OpenClassifyClarityPanel()
    {
        if (GameObject.Find("ClassifyClarityPanel(Clone)") != null)
        {
            Destroy(GameObject.Find("ClassifyClarityPanel(Clone)"));

        }
        PanelManager.instance.OpenSecondPanel("ModelAlphaControlPage", canvasLoaction);
    }
    void OpenAttributePanel()
    {
        if (NowSelectModel != null)
        {
            string uuid = ArManager.GetUuidByModelName(NowSelectModel[0].name);
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> propertyData = GameManager.arManager.nowAllModelProperties;

            string modelName = propertyData[uuid]["基础信息"]["构件名称"];
            string modelType = propertyData[uuid]["基础信息"]["构件类别"];
            string modelInfoStr = "";
            foreach (var item in propertyData[uuid])
            {
                modelInfoStr += item.Key + ":\n";
                foreach (var item2 in item.Value)
                {
                    modelInfoStr += "   " + item2.Key + ":" + item2.Value + "\n";
                }
            }

            ModelInfo modelInfo = new ModelInfo(modelName, modelType, modelInfoStr, null);
            ModelInfo_Window modelInfo_Window = GameManager.uiManager.ShowUiWindow<ModelInfo_Window>(modelInfo);
        }
    }
    void ModelAlphaValueChanged(bool isOn)
    {
        if (modelAlphaToggleNoEventFlag)
        {
            foreach (var item in NowSelectModel)
            {
                Color oringnalColor = item.GetComponent<MeshRenderer>().sharedMaterial.color;
                if (isOn)
                {
                    item.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(oringnalColor.r, oringnalColor.g, oringnalColor.b, 0);
                }
                else
                {
                    item.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(oringnalColor.r, oringnalColor.g, oringnalColor.b, GameManager.arManager.NowAllModelAlpha);
                }
            }
        }
    }
    void CloseAllHighLight()
    {
        if (NowSelectModel != null)
        {
            foreach (var item in NowSelectModel)
            {
                item.GetComponent<Outline>().enabled = false;
            }
        }

    }
    GameObject lastClickedModel;
    float doubleClickLastTime = 0;
    void CheckMouseSelect()
    {
        doubleClickLastTime -= Time.deltaTime;
        if (doubleClickLastTime <= 0)
        {
            lastClickedModel = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.uiManager.IsOnUIElement(Input.mousePosition))
            {
                MyArHitInfo hitInfo = ArEngine.ScreenRaycastGenerateModel(Input.mousePosition);
                if (hitInfo != null)
                {
                    string uuid = ArManager.GetUuidByModelName(hitInfo.transform.name);
                    List<Transform> nowSelectModelTemp = GameManager.arManager.nowCombineModels[uuid];
                    if (nowSelectModelTemp != NowSelectModel)
                    {
                        foreach (var item in nowSelectModelTemp)
                        {
                            GameObject targetModel = item.gameObject;
                            Material targetMat = targetModel.GetComponent<MeshRenderer>().sharedMaterial;
                            Color oringnalColor = targetMat.color;
                            targetMat.color = new Color(oringnalColor.r, oringnalColor.g, oringnalColor.b, GameManager.arManager.NowAllModelAlpha);
                            Outline targetOutLine = targetModel.GetComponent<Outline>();
                            if (targetOutLine == null)
                            {
                                Outline outline = targetModel.gameObject.AddComponent<Outline>();
                                outline.OutlineMode = oulineMode;
                                outline.OutlineColor = outLineColor;
                                outline.OutlineWidth = 5f;
                                targetOutLine = outline;
                            }
                            targetOutLine.enabled = true;
                        }

                        Dictionary<string, Dictionary<string, Dictionary<string, string>>> propertyData = GameManager.arManager.nowAllModelProperties;
                        string modelName = propertyData[uuid]["基础信息"]["构件名称"];

                        modelAlphaToggle.isOn = false;
                        NowSelectModel = nowSelectModelTemp;
                    }
                    else
                    {
                        if (lastClickedModel != null)
                        {
                            NowSelectModel = null;
                            lastClickedModel = null;
                        }
                        else
                        {
                            lastClickedModel = hitInfo.transform.gameObject;
                            doubleClickLastTime = 0.3f;
                        }
                    }
                }
            }
        }

    }
    void CheckMouseSelectTransparent()
    {
        doubleClickLastTime -= Time.deltaTime;
        if (doubleClickLastTime <= 0)
        {
            lastClickedModel = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.uiManager.IsOnUIElement(Input.mousePosition))
            {
                MyArHitInfo hitInfo = ArEngine.ScreenRaycastTransparent(Input.mousePosition);
                if (hitInfo != null)
                {
                    string uuid = ArManager.GetUuidByModelName(hitInfo.transform.name);
                    List<Transform> nowSelectModelTemp = GameManager.arManager.nowCombineModels[uuid];
                    if (nowSelectModelTemp != NowSelectModel)
                    {
                        foreach (var item in nowSelectModelTemp)
                        {
                            GameObject targetModel = item.gameObject;
                            Material targetMat = targetModel.GetComponent<MeshRenderer>().sharedMaterial;
                            Color oringnalColor = targetMat.color;
                            targetMat.color = new Color(oringnalColor.r, oringnalColor.g, oringnalColor.b, GameManager.arManager.NowAllModelAlpha);
                            Outline targetOutLine = targetModel.GetComponent<Outline>();
                            if (targetOutLine == null)
                            {
                                Outline outline = targetModel.gameObject.AddComponent<Outline>();
                                outline.OutlineMode = oulineMode;
                                outline.OutlineColor = outLineColor;
                                outline.OutlineWidth = 5f;
                                targetOutLine = outline;
                            }
                            targetOutLine.enabled = true;
                        }

                        Dictionary<string, Dictionary<string, Dictionary<string, string>>> propertyData = GameManager.arManager.nowAllModelProperties;
                        string modelName = propertyData[uuid]["基础信息"]["构件名称"];

                        modelAlphaToggle.isOn = false;
                        NowSelectModel = nowSelectModelTemp;
                    }
                    else
                    {
                        if (lastClickedModel != null)
                        {
                            NowSelectModel = null;
                            lastClickedModel = null;
                        }
                        else
                        {
                            lastClickedModel = hitInfo.transform.gameObject;
                            doubleClickLastTime = 0.3f;
                        }
                    }
                }
            }
        }

    }
    private void OpenOcclusion(bool isOpen)
    {

        if (Camera.main != null)
        {
            AROcclusionManager aROcclusionManager = Camera.main.GetComponent<AROcclusionManager>();
            if (isOpen == false)
            {
                aROcclusionManager.enabled = false;
                aROcclusionManager.requestedEnvironmentDepthMode = UnityEngine.XR.ARSubsystems.EnvironmentDepthMode.Disabled;
            }
            else
            {
                aROcclusionManager.enabled = true;
                aROcclusionManager.requestedEnvironmentDepthMode = UnityEngine.XR.ARSubsystems.EnvironmentDepthMode.Fastest;
            }
            Debug.Log(aROcclusionManager.requestedEnvironmentDepthMode.ToString());
        }
    }
    #endregion



}
