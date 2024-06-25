using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Outline = QuickOutline.Outline;

public class ArHomePage_Static : StaticUi
{
    public Outline.Mode oulineMode = Outline.Mode.OutlineAll;
    public Color outLineColor = Color.yellow;

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
                    //nowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 1, 1, GameManager.arManager.NowAllModelAlpha);
                }
            }

            nowSelectModel = value;
        }
        get { return nowSelectModel; }
    }

    public GameObject selectedPage;
    public GameObject unSelectedPage;

    public Text selectInfoShow;
    public Button abortSelectButton;
    public Button modelInfoButton;

    public bool modelAlphaToggleNoEventFlag = false;
    public Toggle modelAlphaToggle;

    public GameObject SelectModelPage;

    public Button[] allPageButtons;




    private void Start()
    {
        //初始化
        modelInfoButton.onClick.AddListener(() =>
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
        });

        //modelInfoButton.onClick.AddListener(() =>
        //{
        //    Debug.LogError("展示模型信息，待实现");
        //});


        abortSelectButton.onClick.AddListener(() =>
        {
            NowSelectModel = null;
        });

        modelAlphaToggle.onValueChanged.AddListener((isOn) =>
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
        });

        allPageButtons[0].onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageByPageName("TwoPointLocatePage");
        });

        allPageButtons[1].onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageByPageName("ManualLocatePage");
        });

        allPageButtons[2].onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageByPageName("ModelAlphaControlPage");
        });

        allPageButtons[3].onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageByPageName("FaceDetectPage");
        });
        allPageButtons[4].onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageByPageName("PictureLocatePage");
        });
    }


    private void OnEnable()
    {
        doubleClickLastTime = 0;
    }

    void SetToggleWithoutDoEvent(bool value)
    {
        modelAlphaToggleNoEventFlag = true;
        modelAlphaToggle.isOn = value;
        modelAlphaToggleNoEventFlag = false;
    }

    private void Update()
    {
        CheckMouseSelect();

        if (NowSelectModel == null)
        {
            selectedPage.SetActive(false);
            unSelectedPage.SetActive(true);
        }
        else
        {
            selectedPage.SetActive(true);
            unSelectedPage.SetActive(false);

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
                        selectInfoShow.text = "选中模型:" + modelName;
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
}
