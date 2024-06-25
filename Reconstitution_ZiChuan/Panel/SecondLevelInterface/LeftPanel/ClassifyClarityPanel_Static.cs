using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ClassifyClarityPanel_Static : MonoBehaviour
{
    
    public PrefabInfo mainSliderPrefab;
    public PrefabInfo childSliderPrefab;
    public Transform mainSliderContainer;

    public Button resetButton;
    public Button invisiableAllButton;
    public Button openAllListButton;
    public Button closeAllListButton;

    Dictionary<string, MainAlphaSlider_Element> mainSliders = new Dictionary<string, MainAlphaSlider_Element>();
    Dictionary<string, List<Material>> allMats = new Dictionary<string, List<Material>>();
    Transform targetSceneModel;
    

    
    //public Dictionary<GameObject,ChildAlphaSlider_Element> allChildSliders=new Dictionary<GameObject, ChildAlphaSlider_Element>();  

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainSliderContainer.GetComponent<RectTransform>());
    }
    private void Awake()
    {
        Ini();
        
    }
    public void Ini()
    {
        

        targetSceneModel = GameManager.arManager.targetSceneModel;
        for (int i = 0; i < targetSceneModel.childCount; i++)
        {
            Transform transTemp = targetSceneModel.GetChild(i);
            transTemp.gameObject.AddComponent<ReflashMatMode>();
            AddDic(transTemp);
        }
        GenerateMainSliders();


        resetButton.onClick.AddListener(() =>
        {
            GameManager.arManager.SetAllModelAlpha(1);
        });
        invisiableAllButton.onClick.AddListener(() =>
        {
            GameManager.arManager.SetAllModelAlpha(0);
        });
        openAllListButton.onClick.AddListener(() =>
        {
            ChangeAllChildListStatus(true);
        });
        closeAllListButton.onClick.AddListener(() =>
        {
            ChangeAllChildListStatus(false);
        });

        LayoutRebuilder.ForceRebuildLayoutImmediate(mainSliderContainer.GetComponent<RectTransform>());

        void AddDic(Transform trans)
        {

            MeshRenderer meshRender = trans.GetComponent<MeshRenderer>();
            if (meshRender == null)
            {
                return;
            }
            else
            {
                if (!allMats.ContainsKey(trans.name))
                {
                    allMats.Add(trans.name, new List<Material>());
                }
                // 创建一个新的材质，使用标准着色器
                Material transparentMaterial = new Material(Shader.Find("Standard"));

                meshRender.material = transparentMaterial;

                allMats[trans.name].Add(transparentMaterial);
            }


        }


    }


    private void ChangeAllChildListStatus(bool show)
    {
        foreach (var item in mainSliders)
        {
            item.Value.ChangeChildListStatus(show);
        }
    }


    public  void SetAllModelAlpha(float value)
    {
        foreach (var item in mainSliders)
        {
            item.Value.SetAlphaValue(value);
        }
    }



    void GenerateMainSliders()
    {

        foreach (var item in allMats)
        {
            GameObject mainSliderTemp = GameObjectPoolTool.GetFromPoolForce(true, mainSliderPrefab.path);
            mainSliderTemp.transform.SetParent(mainSliderContainer);
            MainAlphaSlider_Element mainAlphaSlider_Element = mainSliderTemp.GetComponent<MainAlphaSlider_Element>();

            List<Material> jiShuMats = new List<Material>();
            List<Material> ouShuMats = new List<Material>();

            for (int i = 0; i < item.Value.Count; i++)
            {
                Material matItem = item.Value[i];
                if (i % 2 == 0)
                {
                    ouShuMats.Add(matItem);
                }
                else
                {
                    jiShuMats.Add(matItem);
                }
            }

            List<ChildAlphaSlider_Element> targetChildSliders = new List<ChildAlphaSlider_Element>();
            if (jiShuMats.Count != 0)
            {
                targetChildSliders.Add(GenerateChildSlider("基数下标", jiShuMats, mainAlphaSlider_Element));
            }

            if (ouShuMats.Count != 0)
            {
                targetChildSliders.Add(GenerateChildSlider("偶数下标", ouShuMats, mainAlphaSlider_Element));
            }

            mainAlphaSlider_Element.Ini(targetChildSliders, item.Key);

            mainSliders.Add(item.Key, mainAlphaSlider_Element);
        }

        if (mainSliders.ContainsKey("Other"))
        {
            mainSliders["Other"].transform.SetAsLastSibling();
        }


        ChildAlphaSlider_Element GenerateChildSlider(string title, List<Material> mats, MainAlphaSlider_Element fatherSlider)
        {
            GameObject childSliderTemp = GameObjectPoolTool.GetFromPoolForce(true, childSliderPrefab.path);
            childSliderTemp.transform.SetParent(fatherSlider.childSliderFather.transform);
            ChildAlphaSlider_Element childAlphaSlider_Element = childSliderTemp.GetComponent<ChildAlphaSlider_Element>();
            childAlphaSlider_Element.Ini(mats, title);
            return childAlphaSlider_Element;
        }
    }



}
