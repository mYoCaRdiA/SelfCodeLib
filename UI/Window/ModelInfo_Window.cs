using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class ModelInfo_Window : UiWindow
{
    public Text modelName;
    public Text modelType;
    public Image modelImage;

    public Button gouJianInfoButton;
    public Button bimInfoButton;

    public Book infoBook;

    public Button addChangeReport;
    public Button closeButton;

    public Text modelInfoShow;

    public ScrollRect scrollRect;


    protected override void OnEnable()
    {
        base.OnEnable();
        scrollRect.normalizedPosition= new Vector2(scrollRect.normalizedPosition.x, 1); 
    }
    public void ResetValue(ModelInfo info)
    {
        modelName.text = info.modelName;
        modelType.text = info.modelType;
        modelImage.sprite = info.modelSprite;
        modelInfoShow.text = info.modelInfo;

    }


    private void Start()
    {

        closeButton.onClick.AddListener(() =>
        {
            CloseUi();
        });

        gouJianInfoButton.onClick.AddListener(() =>
        {
            infoBook.ChangePageTo(1);
        });

        bimInfoButton.onClick.AddListener(() =>
        {
            infoBook.ChangePageTo(2);
        });
        addChangeReport.onClick.AddListener(() =>
        {
            Debug.LogError("弹出---新建整改通知单---窗口");
        });
    }
}

public class ModelInfo
{
    public string modelName;
    public string modelType;
    public string modelInfo;
    public Sprite modelSprite;

    public ModelInfo(string modelName, string modelType, string modelInfo,Sprite modelSprite)
    {
        this.modelName = modelName;
        this.modelType = modelType;
        this.modelSprite = modelSprite;
        this.modelInfo = modelInfo;
    }
}
