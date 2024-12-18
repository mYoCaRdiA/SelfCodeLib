using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : Manager
{
    [SerializeField, Header("窗口ui资源索引")]
    private PrefabInfo mask;
    [SerializeField]
    private PrefabInfo loadMask;
    [SerializeField]
    private PrefabInfo tip;
    [SerializeField]
    private PrefabInfo userInfo;
    [SerializeField]
    private PrefabInfo platform;
    [SerializeField]
    private PrefabInfo viewTip;
    [SerializeField]
    private PrefabInfo modelInfo;
    [SerializeField]
    private PrefabInfo imageTip;
    [SerializeField]
    private PrefabInfo workerInfo;
    [SerializeField]
    private PrefabInfo confirmTip;

    [SerializeField, Header("元素ui资源索引")]
    private PrefabInfo projectItem;
    [SerializeField]
    private PrefabInfo buildingType;
    [SerializeField]
    private PrefabInfo buildingModel;


    List<UiWindow> allUiWindows = new List<UiWindow>();
    List<UiElement> allUiElements = new List<UiElement>();


    static Book mainBook;
    static Book nowBook;



    public static Book NowBook
    {
        set { nowBook = value; }
        get
        {
            if (nowBook == null)
            {
                return mainBook;
            }
            else
            {
                return nowBook;
            }
        }
    }


    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private PointerEventData eventData;

    /// <summary>
    /// 获取ui元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUiElement<T>()
    {
        string path = "";
        string typeStr = typeof(T).ToString();
        switch (typeStr)
        {
            case nameof(ProjectItem_Element):
                path = projectItem.path;
                break;
            case nameof(BuildingType_Element):
                path = buildingType.path;
                break;
            case nameof(BuildingModel_Element):
                path = buildingModel.path;
                break;
        }
        if (path == "")
        {
            Debug.LogError(typeStr + ":未配置元素索引");
            return default(T);
        }

        GameObject targetGameObj = GameObjectPoolTool.GetFromPoolForce(true, path);
        T uiWin = targetGameObj.GetComponent<T>();
        RegisterElement(targetGameObj.GetComponent<UiElement>());
        return uiWin;



        void RegisterElement(UiElement uiElement)
        {
            if (!allUiElements.Contains(uiElement))
            {
                allUiElements.Add(uiElement);
            }
        }

    }

    /// <summary>
    /// 展示窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public T ShowUiWindow<T>(object data = null)
    {
        string path = "";
        string typeStr = typeof(T).ToString();
        switch (typeStr)
        {
            case nameof(MaskOnly_Window):
                path = mask.path;
                break;
            case nameof(Loading_Window):
                path = loadMask.path;
                break;
            case nameof(Tip_Window):
                path = tip.path;
                break;
            case nameof(UserInfo_Window):
                path = userInfo.path;
                break;
            case nameof(Platform_Window):
                path = platform.path;
                break;
            case nameof(ViewTip_Window):
                path = viewTip.path;
                break;
            case nameof(ModelInfo_Window):
                path = modelInfo.path;
                break;
            case nameof(ImageTip_Window):
                path = imageTip.path;
                break;
            case nameof(WorkerInfo_Window):
                path = workerInfo.path;
                break;
            case nameof(ConfirmTip_Window):
                path = confirmTip.path;
                break;
        }
        if (path == "")
        {
            Debug.LogError(typeStr + ":未配置窗口索引");
            return default(T);
        }

        GameObject targetGameObj = GameObjectPoolTool.GetFromPoolForce(true, path);
        T uiWinT = targetGameObj.GetComponent<T>();
        UiWindow uiWin = targetGameObj.GetComponent<UiWindow>();
        RegisterWindow(uiWin);

        switch (typeStr)
        {
            case nameof(Tip_Window):
                (uiWin as Tip_Window).SetTextValue(data.ToString());
                break;
            case nameof(ViewTip_Window):
                (uiWin as ViewTip_Window).SetTextValue(data.ToString());
                break;
            case nameof(ModelInfo_Window):
                (uiWin as ModelInfo_Window).ResetValue(data as ModelInfo);
                break;
            case nameof(ImageTip_Window):
                (uiWin as ImageTip_Window).SetTextAndSprite(data as ImageTipData);
                break;
            case nameof(WorkerInfo_Window):
                (uiWin as WorkerInfo_Window).SetData(data as WorkerData);
                break;
            case nameof(ConfirmTip_Window):
                (uiWin as ConfirmTip_Window).SetAllValue(data as ConfirmTip_Window.IniData);
                break;

        }

        //注册窗口
        void RegisterWindow(UiWindow uiWin)
        {
            if (!allUiWindows.Contains(uiWin))
            {
                allUiWindows.Add(uiWin);
            }
        }

        return uiWinT;
    }



    /// <summary>
    /// 是否在UI上
    /// </summary>
    /// <returns></returns>
    public bool IsOnUIElement(Vector3 screenPostion)
    {
        if (graphicRaycaster == null)
        {
            Debug.LogError("无GraphicRaycaster,有问题");
            return false;
        }
        eventData.pressPosition = screenPostion;
        eventData.position = screenPostion;
        List<RaycastResult> list = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, list);
        foreach (var temp in list)
        {
            if (temp.gameObject.layer.Equals(5)) return true;
        }
        return false;
    }

    /// <summary>
    /// 回收所有窗口
    /// </summary>
    public void CloseAllWindows()
    {
        foreach (var item in allUiWindows)
        {
            GameObjectPoolTool.PutInPool(item.gameObject);
        }
    }

    /// <summary>
    /// 回收所有元素
    /// </summary>
    public void CloseAllElements()
    {
        foreach (var item in allUiElements)
        {
            GameObjectPoolTool.PutInPool(item.gameObject);
        }
    }


    void UpdateUiComponet()
    {
        graphicRaycaster = NowBook.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        eventData = new PointerEventData(eventSystem);
    }
    public override void Ini()
    {
        if (GameManager.uiManager == null)
        {
            GameManager.uiManager = this;
        }

        SceneManager.sceneLoaded += (scene, type) =>
        {
            if (LoadSceneMode.Single == type)
            {
                UpdateUiComponet();
            }
        };
        mainBook = GameObject.Find("MainCanvas").transform.GetComponent<Book>();
        // mainBook.transform.SetParent(transform, true);
        //mainBook.name = "MainCanvas";
       DontDestroyOnLoad(mainBook);
       UpdateUiComponet();
    }
}
