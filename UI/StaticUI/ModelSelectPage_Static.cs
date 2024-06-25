using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public static class ModelDownloadData
{


}
public class ModelSelectPage_Static : StaticUi
{
    BuildingModel_Element selectedButton;

    Dictionary<BuildingType_Element, List<BuildingModel_Element>> nowBuildModelButtons = new Dictionary<BuildingType_Element, List<BuildingModel_Element>>();



    public BuildingTypeData lastBuildingTypeData;
    private List<BuildingType_Element> nowBuildingTypeEntities = new List<BuildingType_Element>();

    public Book book;
    public Text title;
    public UnityEngine.UI.Button backButton;
    public UnityEngine.UI.Button openModelButton1;

    public Transform leftSeletionContainer;
    public Transform rightSeletionContainer;
    public static string modelName;
    private void Start()
    {
        
        backButton.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            UiManager.NowBook.ChangePageTo(2);

        });
        openModelButton1.onClick.AddListener(() =>
        {
            if (selectedButton != null && selectedButton.gameObject.activeInHierarchy && selectedButton.seletedFlag.activeSelf == true)
            {
                GameManager.dataManager.nowModelInfoItem = selectedButton.modelInfoItem;
                UiManager.NowBook.ChangePageTo(4);
                //Debug.Log("切换新页面：" + selectedButton.modelInfoItem.ModelName);
            }
        });
    }
    private void OnEnable()
    {
        //初始化到模型选择画面
        book.ChangePageTo(1);
        if (lastBuildingTypeData != GameManager.dataManager.nowBuildingTypeData)
        {
            StartCoroutine(GenerateModelTypeInfo2Buttons());
            title.text = GameManager.dataManager.nowProjectItemData.fullName;
        }
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    }



    IEnumerator GenerateModelTypeInfo2Buttons()
    {
        MaskOnly_Window uiWin = GameManager.uiManager.ShowUiWindow<MaskOnly_Window>();
        foreach (var item in nowBuildingTypeEntities)
        {
            item.CloseUi();
        }
        nowBuildingTypeEntities.Clear();
        foreach (var item in nowBuildModelButtons)
        {
            foreach (var item2 in item.Value)
            {
                item2.CloseUi();
            }
        }
        nowBuildModelButtons.Clear();

        string title = GameManager.dataManager.nowBuildingTypeData.list["Item1"].Count != 0 ? "Item1" : "";
        yield return StartCoroutine(CreateAllElement(title));

        ShowAllModelButtons();
        lastBuildingTypeData = GameManager.dataManager.nowBuildingTypeData;
        uiWin.CloseUi();
    }


    IEnumerator CreateAllElement(string title = "")
    {
        if (title == "")
        {
            BuildingType_Element element = GameManager.uiManager.GetUiElement<BuildingType_Element>();
            element.RestoreDefaultColor();
            element.transform.SetParent(leftSeletionContainer);
            element.transform.localScale = Vector3.one;
            element.SetTextStr("所有模型");
            nowBuildingTypeEntities.Add(element);
            yield return StartCoroutine(CreateTypeButtons(element, ""));
        }
        else
        {
            foreach (Label item in GameManager.dataManager.nowBuildingTypeData.list[title])
            {
                yield return StartCoroutine(CreateOneType(item));
            }
        }


        IEnumerator CreateOneType(Label item)
        {
            BuildingType_Element element = GameManager.uiManager.GetUiElement<BuildingType_Element>();
            element.RestoreDefaultColor();
            element.transform.SetParent(leftSeletionContainer);
            element.transform.localScale = Vector3.one;
            element.SetTextStr(item.LabelName);
            nowBuildingTypeEntities.Add(element);

            yield return StartCoroutine(CreateTypeButtons(element, item.Id));

            element.onAim = new Action(() =>
            {
                foreach (var item in nowBuildingTypeEntities)
                {
                    if (item != element) item.RestoreDefaultColor();
                }
                ShowTypeModelButtons(element);
            });

            element.onDisAim = new Action(() =>
            {
                ShowAllModelButtons();
            });



        }

        IEnumerator CreateTypeButtons(BuildingType_Element type, string id)
        {
            // 创建一个字典来存储要发送的数据
            Dictionary<string, string> jsonData = new Dictionary<string, string>();
            if (id != "")
            {
                jsonData.Add("tagId1", id);
            }
            jsonData.Add("type", "web");

            RequestClass targetData = null;
            IEnumerator requestModelSelectInfo = HttpTool.PostRequest<RequestClass>(HttpTool.projectToken, UrlTail.projectModelInfo, (result) =>
            {

                targetData = result;
            }, jsonData);
            yield return StartCoroutine(requestModelSelectInfo);

            string jsonStr = targetData.data.ToString();
            Debug.Log("jsonstr:" + jsonStr);
            ModelInfoData modelInfoData = JsonConvert.DeserializeObject<ModelInfoData>(jsonStr);

            nowBuildModelButtons.Add(type, new List<BuildingModel_Element>());

            //这里开始遍历存储数据
            foreach (var modelInfo in modelInfoData.list)
            {

                GenerateBuildingModel_Element(modelInfo, type);

            }
        }

    }
    void GenerateBuildingModel_Element(ModelInfoData.LoadPathItem modelInfo, BuildingType_Element type)
    {
        BuildingModel_Element buildingModel_Element = GameManager.uiManager.GetUiElement<BuildingModel_Element>();
        buildingModel_Element.modelInfoItem = modelInfo;
        string a = buildingModel_Element.modelInfoItem.LoadPath.ModelPath.Replace("/glbs", "");
        string path = Path.Combine(Application.persistentDataPath, "ModelDatas", a.Replace("/", "-"), "OK.txt");
        buildingModel_Element.button.onClick.RemoveAllListeners();
        buildingModel_Element.slider.gameObject.SetActive(false);
        if (File.Exists(path))
        {
            buildingModel_Element.SetNameAndTime(modelInfo.ModelName, TimeTool.TimeStamp2Str(modelInfo.LastModifyTime));
            nowBuildModelButtons[type].Add(buildingModel_Element);

            buildingModel_Element.transform.SetParent(rightSeletionContainer);
            buildingModel_Element.seletedFlag.SetActive(false);
            buildingModel_Element.downloadImage.gameObject.SetActive(false);
            buildingModel_Element.readyImage.gameObject.SetActive(true);
            EventTriggerTool.SetTriggerEvent(buildingModel_Element.button.image, new Action(() =>
            {
                ReleaseSelection();
                //记录当前的BuildingModel_Element按钮元素
                selectedButton = buildingModel_Element;
                selectedButton.seletedFlag.SetActive(true);
                GameManager.dataManager.nowBuildUrl = buildingModel_Element.modelInfoItem.LoadPath.ModelPath;
                GameManager.dataManager.nowProjectId = buildingModel_Element.modelInfoItem.LoadPath.ModelId;
                modelName = modelInfo.ModelName;

            }), UnityEngine.EventSystems.EventTriggerType.PointerDown);
        }
        else
        {

            buildingModel_Element.SetNameAndTime(modelInfo.ModelName, TimeTool.TimeStamp2Str(modelInfo.LastModifyTime));
            nowBuildModelButtons[type].Add(buildingModel_Element);
            buildingModel_Element.downloadImage.gameObject.SetActive(true);
            buildingModel_Element.readyImage.gameObject.SetActive(false);
            buildingModel_Element.transform.SetParent(rightSeletionContainer);
            buildingModel_Element.seletedFlag.SetActive(false);
            if (buildingModel_Element.downloadImage.gameObject.activeSelf == true)
            {
                buildingModel_Element.button.onClick.AddListener(() =>
                {
                    ReleaseSelection();

                    GameManager.dataManager.nowBuildUrl = buildingModel_Element.modelInfoItem.LoadPath.ModelPath;
                    GameManager.dataManager.nowProjectId = buildingModel_Element.modelInfoItem.LoadPath.ModelId;
                    Debug.Log(GameManager.dataManager.nowBuildUrl);
                    buildingModel_Element.button.onClick.RemoveAllListeners();
                    buildingModel_Element.slider.gameObject.SetActive(true);
                    modelName = modelInfo.ModelName;
                    StartCoroutine(HttpTool.GetModelDataFiles(GameManager.dataManager.nowBuildUrl, (percent) =>
                    {
                        DownloadSlider(buildingModel_Element.slider, buildingModel_Element.downloadImage, buildingModel_Element.readyImage, buildingModel_Element, percent);
                    }));
                });
            }
            else
            {
                EventTriggerTool.SetTriggerEvent(buildingModel_Element.button.image, new Action(() =>
                {

                    ReleaseSelection();
                    //记录当前的BuildingModel_Element按钮元素
                    selectedButton = buildingModel_Element;
                    GameManager.dataManager.nowBuildUrl = buildingModel_Element.modelInfoItem.LoadPath.ModelPath;
                    GameManager.dataManager.nowProjectId = buildingModel_Element.modelInfoItem.LoadPath.ModelId;
                    selectedButton.seletedFlag.SetActive(true);
                    modelName = modelInfo.ModelName;
                }), UnityEngine.EventSystems.EventTriggerType.PointerDown);
            }
        }
    }

    void ReleaseSelection()
    {
        foreach (var item in nowBuildModelButtons)
        {
            foreach (var item2 in item.Value)
            {
                item2.seletedFlag.SetActive(false);
            }
        }
    }
    void ShowTypeModelButtons(BuildingType_Element typeElement)
    {
        Debug.Log(typeElement.ButtonString);
        foreach (var item in nowBuildModelButtons)
        {
            // List<BuildingType_Element> buildingType_Elements =;
            if (item.Key != typeElement)
            {
                foreach (var modelButton in item.Value)
                {
                    modelButton.CloseUi();
                }
            }
            else
            {
                foreach (var modelButton in item.Value)
                {
                    modelButton.ShowUi();
                    modelButton.transform.SetAsLastSibling();
                }
            }
        }

    }
    void ShowAllModelButtons()
    {
        foreach (var item in nowBuildModelButtons)
        {

            foreach (var modelButton in item.Value)
            {
                modelButton.ShowUi();
                modelButton.transform.SetAsLastSibling();
            }

        }
    }
    void DownloadSlider(UnityEngine.UI.Slider slider, UnityEngine.UI.Image downloadImage, UnityEngine.UI.Image readyImage, BuildingModel_Element buildingModel_Element, float percent)
    {
        
        
        slider.value = percent;
        if (slider.value == 1)
        
        {

            slider.gameObject.SetActive(false);

            downloadImage.gameObject.SetActive(false);
            readyImage.gameObject.SetActive(true);


            buildingModel_Element.button.onClick.RemoveAllListeners();
            EventTriggerTool.SetTriggerEvent(buildingModel_Element.button.image, new Action(() =>
            {
                ReleaseSelection();
                //记录当前的BuildingModel_Element按钮元素
                selectedButton = buildingModel_Element;
                selectedButton.seletedFlag.SetActive(true);
            }), UnityEngine.EventSystems.EventTriggerType.PointerDown);

        }


    }
}
