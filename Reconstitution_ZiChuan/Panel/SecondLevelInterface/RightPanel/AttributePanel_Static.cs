//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Outline = QuickOutline.Outline;
//public class AttributePanel_Static : StaticUi
//{
//    public Button btnClose;

//    public Transform canvasLoaction { get; set; }

    
    

    
//    public Outline.Mode oulineMode = Outline.Mode.OutlineAll;
//    public Color outLineColor = Color.yellow;

//    GameObject nowSelectModel;
//    /*GameObject NowSelectModel
//    {
//        set
//        {
//            if (value != nowSelectModel)
//            {
//                if (nowSelectModel != null)
//                {
//                    nowSelectModel.gameObject.GetComponent<Outline>().enabled = false;
//                    //nowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 1, 1, GameManager.arManager.NowAllModelAlpha);
//                }
//            }

//            nowSelectModel = value;
//        }
//        get { return nowSelectModel; }
//    }*/

//    public GameObject selectedPage;
//    public GameObject unSelectedPage;


//    public Text selectInfoShow;
//    public Button abortSelectButton;
//    //public Button modelInfoButton;

//    public bool modelAlphaToggleNoEventFlag = false;
//    public Toggle modelAlphaToggle;

//    public GameObject SelectModelPage;

    

//    private void Start()
//    {
//        canvasLoaction = this.transform.parent.transform;

//        btnClose.onClick.AddListener(() => { PanelManager.instance.CloseSecondPanel(); });
                    
        
//        /*modelInfoButton.onClick.AddListener(() =>
//        {
//            if (NowSelectModel != null)
//            {
//                ModelInfo modelInfo = new ModelInfo(NowSelectModel.name, "模型类型", null);
//                ModelInfo_Window modelInfo_Window = GameManager.uiManager.ShowUiWindow<ModelInfo_Window>(modelInfo);
//            }
//        });

//        modelInfoButton.onClick.AddListener(() =>
//        {
//            Debug.LogError("展示模型信息，待实现");
//        });*/


//        abortSelectButton.onClick.AddListener(() =>
//        {

//            MainPanel_Static.NowSelectModel = null;
//        });

//        modelAlphaToggle.onValueChanged.AddListener((isOn) =>
//        {
//            if (modelAlphaToggleNoEventFlag)
//            {
//                if (isOn)
//                {
//                    MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 1, 1, 0);
//                }
//                else
//                {
//                    MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 1, 1, GameManager.arManager.NowAllModelAlpha);
//                }
//            }
//        });

        
//    }


//    void SetToggleWithoutDoEvent(bool value)
//    {
//        modelAlphaToggleNoEventFlag = true;
//        modelAlphaToggle.isOn = value;
//        modelAlphaToggleNoEventFlag = false;
//    }

//    [System.Obsolete]
//    private void Update()
//    {
//        if (GameObject.Find("Left").transform.FindChild("MeasureTypePanel_Static").gameObject.activeSelf == false && GameObject.Find("Left").transform.FindChild("LocateTypePanel_Static").gameObject.activeSelf == false)
//        {
//            CheckMouseSelect();
//        }

//        if (MainPanel_Static.NowSelectModel == null)
//        {
//            selectedPage.SetActive(false);
//            unSelectedPage.SetActive(true);
//        }
//        else
//        {
//            selectedPage.SetActive(true);
//            unSelectedPage.SetActive(false);
//            selectInfoShow.text = "选中模型:" + MainPanel_Static.NowSelectModel.name;
//            if (modelAlphaToggle.isOn == true)
//            {
//                if (MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.a != 0)
//                {
//                    MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.r, MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.g, MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.b, 0);
//                }
//            }
//            else
//            {
//                if (MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.a == 0)
//                {
//                    MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.r, MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.g, MainPanel_Static.NowSelectModel.GetComponent<MeshRenderer>().sharedMaterial.color.b, GameManager.arManager.NowAllModelAlpha);
//                }
//            }
//        }
//    }


//    void CheckMouseSelect()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            if (!GameManager.uiManager.IsOnUIElement(Input.mousePosition))
//            {
//                MyArHitInfo hitInfo = ArEngine.ScreenRaycastGenerateModel(Input.mousePosition);
//                if (hitInfo != null && hitInfo.transform.gameObject != MainPanel_Static.NowSelectModel)
//                {
//                    GameObject targetModel = hitInfo.transform.gameObject;
//                    Material targetMat = targetModel.GetComponent<MeshRenderer>().sharedMaterial;
//                    selectInfoShow.text = "选中模型:" + targetModel.name;
//                    modelAlphaToggle.isOn = false;
//                    targetMat.color = new Color(targetMat.color.r, targetMat.color.g, targetMat.color.b, GameManager.arManager.NowAllModelAlpha);
//                    Outline targetOutLine = targetModel.GetComponent<Outline>();
//                    if (targetOutLine == null)
//                    {
//                        Outline outline = targetModel.gameObject.AddComponent<Outline>();
//                        outline.OutlineMode = oulineMode;
//                        outline.OutlineColor = outLineColor;
//                        outline.OutlineWidth = 5f;
//                        targetOutLine = outline;
//                    }
//                    targetOutLine.enabled = true;
//                    MainPanel_Static.NowSelectModel = targetModel;
//                }
//            }
//        }
//    }
//}
