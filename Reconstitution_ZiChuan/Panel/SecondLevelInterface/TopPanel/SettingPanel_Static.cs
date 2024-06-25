using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/// <summary>
/// 设置界面数据类
/// </summary>
/*public class SettingInfoClass
{
    
    public float postExposureValue;
    public float contrastValue;
    public float saturationValue;
    public float lineWidthValue;
    public float sectionCutValue;

    public SettingInfoClass( float _postExposureValue, float _contrastValue, float _saturationValue, float _lineWidthValue, float _sectionCutValue)
    {
        
        postExposureValue = _postExposureValue;
        contrastValue = _contrastValue;
        saturationValue = _saturationValue;
        lineWidthValue = _lineWidthValue;
        sectionCutValue = _sectionCutValue;
    }
}*/
public class SettingPanel_Static : StaticUi
{
    public Toggle realTwinMode;
    [Header("Slider")]
    public Slider postExposureSlider;
    public Slider contrastSlider;
    public Slider saturationSlider;
    public Slider lineWidth;
    public Slider sectionCut;
    [Header("Button")]
    public Button btnClose;
    public Button btnClearData;
    public Button btnResetModel;
    public Button btnResetParam;

    //private Volume myVolume;
   // private ColorAdjustments colorAdjustments;
    private SettingInfoClass infoClass;

   // ScriptableRendererData[] rendererDataList;
    //private scan scan;
    public Transform canvasLoaction { get; set; }

    private void OnEnable()
    {
        infoClass = new SettingInfoClass(postExposureSlider.value, contrastSlider.value, saturationSlider.value, lineWidth.value, sectionCut.value);
    }
    // Start is called before the first frame update
    void Start()
    {
        realTwinMode.isOn = false;
        realTwinMode.onValueChanged.AddListener((isOn) => 
        {
            MainPanel_Static.realTwinMode = isOn;
        }); ;
        //InitInfo();
        canvasLoaction = this.transform.parent.transform;
        postExposureSlider.onValueChanged.AddListener((float value) => {
            //colorAdjustments.postExposure.value = value;
            postExposureSlider.transform.Find("Text").GetComponent<Text>().text = value.ToString("f0");
        });
        postExposureSlider.value = 1;

        contrastSlider.onValueChanged.AddListener((float value) =>
        {
            //colorAdjustments.contrast.value = value;
            contrastSlider.transform.Find("Text").GetComponent<Text>().text = value.ToString("f0");
        });
        contrastSlider.value = 0;

        saturationSlider.onValueChanged.AddListener((float value) =>
        {
            //colorAdjustments.saturation.value = value;
            saturationSlider.transform.Find("Text").GetComponent<Text>().text = value.ToString("f0");
        });
        saturationSlider.value = 0;


       

        lineWidth.onValueChanged.AddListener((float value) =>
        {
            /*scan.mysetting.EdgeSample = value;
            scan.Create();*/
            Debug.Log("边框宽度");
            lineWidth.transform.Find("Text").GetComponent<Text>().text = value.ToString("f1");
        });
        lineWidth.value = 0.0f;

        sectionCut.onValueChanged.AddListener((float value) => {
            Camera.main.nearClipPlane = value;
            sectionCut.transform.Find("Text").GetComponent<Text>().text = value.ToString("f1");
        });
        sectionCut.value = 0.01f;


        btnClose.onClick.AddListener(() => { PanelManager.instance.CloseSecondPanel(); });
        btnClearData.onClick.AddListener(() => {
            Debug.Log("清空缓存");
        });
        btnResetModel.onClick.AddListener(() => {
            Debug.Log("重置模型");
        });
        btnResetParam.onClick.AddListener(() => {
            ResetParam();
            Debug.Log("重设参数");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    /// <summary>
    /// 初始化数据
    /// </summary>
    private void InitInfo()
    {
        sectionCut.value = Camera.main.nearClipPlane;

        var proInfo = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList",
                      BindingFlags.NonPublic | BindingFlags.Instance);

        if (proInfo != null)
        {
            UniversalRenderPipelineAsset asset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            Debug.Log(asset);
            rendererDataList = (ScriptableRendererData[])proInfo.GetValue(asset);
            for (int i = 0; i < rendererDataList.Length; i++)
            {
                if (rendererDataList[i].name.Equals("UrpTest_Renderer"))
                {
                   scan = rendererDataList[i].rendererFeatures[4] as scan;
                    if (scan != null)
                    {
                        lineWidth.value = scan.mysetting.EdgeSample;
                    }
                }
            }

            myVolume = GameObject.Find("Box Volume").GetComponent<Volume>();
            postExposureSlider.SetSliderMinAndMax(-50.0f, 50.0f);
            postExposureSlider.value = 0;

            lineWidth.SetSliderMinAndMax(0.0f, 5.0f);

            if (myVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
            {
                contrastSlider.SetSliderMinAndMax(colorAdjustments.contrast.min, colorAdjustments.contrast.max);
                contrastSlider.value = colorAdjustments.contrast.value;

                saturationSlider.SetSliderMinAndMax(colorAdjustments.saturation.min, colorAdjustments.saturation.max);
                saturationSlider.value = colorAdjustments.saturation.value;
            }
        }
    }*/
    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetParam()
    {
        postExposureSlider.value = infoClass.postExposureValue;
        contrastSlider.value = infoClass.contrastValue;
        saturationSlider.value = infoClass.saturationValue;
        lineWidth.value = infoClass.lineWidthValue;
        sectionCut.value = infoClass.sectionCutValue;
    }
   
}
