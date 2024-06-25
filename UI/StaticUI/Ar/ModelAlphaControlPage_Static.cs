using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModelAlphaControlPage_Static : StaticUi
{
    public Button returnButton;
    public PrefabInfo mainSliderPrefab;
    public PrefabInfo childSliderPrefab;
    public Transform mainSliderContainer;

    public Button resetButton;
    public Button invisiableAllButton;
    public Button openAllListButton;
    public Button closeAllListButton;

    Dictionary<string, MainAlphaSlider_Element> mainSliders = new Dictionary<string, MainAlphaSlider_Element>();

    Dictionary<string, Dictionary<string, List<Material>>> treeMapper = new Dictionary<string, Dictionary<string, List<Material>>>();
    Dictionary<string, string> treeMapperChildFatherPair = new Dictionary<string, string>();


    Transform targetSceneModel;

    //public Dictionary<GameObject,ChildAlphaSlider_Element> allChildSliders=new Dictionary<GameObject, ChildAlphaSlider_Element>();  

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "NowVersion")
        {
            returnButton.gameObject.SetActive(false);
        }
        else
        {
            returnButton.gameObject.SetActive(true);

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainSliderContainer.GetComponent<RectTransform>());
    }





    private void ChangeAllChildListStatus(bool show)
    {
        foreach (var item in mainSliders)
        {
            item.Value.ChangeChildListStatus(show);
        }
    }


    public void SetAllModelAlpha(float value)
    {
        foreach (var item in mainSliders)
        {
            item.Value.SetAlphaValue(value);
        }
    }



    public IEnumerator Ini()
    {

        if (GameManager.arManager.targetSceneModel != null)
        {
            Destroy(GameManager.arManager.targetSceneModel.gameObject);
        }

        //初始化属性数据，之后会用于构建
        yield return GameManager.arManager.StartCoroutine(SetPropertyData());

        //构建treeMapper
        yield return GameManager.arManager.StartCoroutine(MakeTreeMapper());

        //生成模型进度条
        Loading_Window loading_Window = GameManager.uiManager.ShowUiWindow<Loading_Window>();
        //生成模型,包装模型,并treeMapper绑定材质
        yield return GameManager.arManager.StartCoroutine(GenerateModels(GameManager.dataManager.nowBuildUrl, (t) => { loading_Window.ChangeTargetPercent(t); }));
        yield return new WaitForSeconds(1f);
        loading_Window.CloseUi();

        //生成透明度滑动条
        MakeAlphaSlider();

        //按钮赋值
        IniButtonEvent();

        IEnumerator SetPropertyData()
        {
            string dataPath = IoTools.Model_IO.GetModelDownloadPath(GameManager.dataManager.nowBuildUrl);
            string jsonPath = Path.Combine(dataPath, "property", "property.json");
            string jsonString = IoTools.ReadFileString(jsonPath);
            ModelPropertyData modelPropertyData = JsonConvert.DeserializeObject<ModelPropertyData>(jsonString);
            // string temp = modelPropertyData.Categorys[item2[0]] + "*" + modelPropertyData.Names[item2[1]] + "*" + modelPropertyData.Values[item2[2]];
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> allModelPropertiesDic = GameManager.arManager.nowAllModelProperties;
            allModelPropertiesDic.Clear();
            foreach (var item in modelPropertyData.Models)
            {
                allModelPropertiesDic.Add(item.Key, new Dictionary<string, Dictionary<string, string>>());
                Dictionary<string, Dictionary<string, string>> targetModelInfoData = allModelPropertiesDic[item.Key];
                foreach (var item2 in item.Value)
                {
                    string title1 = modelPropertyData.Categorys[item2[0]];
                    string title2 = modelPropertyData.Names[item2[1]];
                    string title3 = modelPropertyData.Values[item2[2]];
                    if (!targetModelInfoData.ContainsKey(title1))
                    {
                        targetModelInfoData.Add(title1, new Dictionary<string, string>());
                    }
                    if (!targetModelInfoData[title1].ContainsKey(title2))
                    {
                        targetModelInfoData[title1].Add(title2, title3);
                    }
                }
                List<KeyValuePair<string, Dictionary<string, string>>> list = new List<KeyValuePair<string, Dictionary<string, string>>>();
                //反转
                foreach (var temp in targetModelInfoData)
                {
                    list.Add(temp);
                }
                targetModelInfoData.Clear();
                list.Reverse();
                foreach (var temp2 in list)
                {
                    targetModelInfoData.Add(temp2.Key, temp2.Value);
                }
            }


            Debug.Log("属性字典生成完毕");
            yield return null;
        }
        IEnumerator MakeTreeMapper()
        {
            //完善材质组数据-bigMatGloups
            TreeMapperData treeMapperData = GetTreeMapperData(GameManager.dataManager.nowBuildUrl);
            Dictionary<string, List<string>> mapperDic = new Dictionary<string, List<string>>();
            foreach (var item in treeMapperData.treeMappers)
            {
                if (!mapperDic.ContainsKey(item.T_Name))
                {
                    mapperDic.Add(item.T_Name, new List<string>());
                }

                if (!mapperDic[item.T_Name].Contains(item.Name))
                {
                    mapperDic[item.T_Name].Add(item.Name);
                    // Debug.Log("添加" + item.Name);
                }

            }
            foreach (var item in mapperDic)
            {
                treeMapper.Add(item.Key, new Dictionary<string, List<Material>>());
                foreach (var item2 in item.Value)
                {
                    treeMapper[item.Key].Add(item2, new List<Material>());
                    treeMapperChildFatherPair.Add(item2, item.Key);
                }


            }
            yield return null;

            TreeMapperData GetTreeMapperData(string urlPath)
            {
                string dataPath = IoTools.Model_IO.GetModelDownloadPath(urlPath);
                string jsonPath = Path.Combine(dataPath, "treeMapper", "treeMapper.json");
                string jsonString = IoTools.ReadFileString(jsonPath);
                List<TreeMapperData.TreeMapper> mappers = JsonConvert.DeserializeObject<List<TreeMapperData.TreeMapper>>(jsonString);
                TreeMapperData treeMapperData = new TreeMapperData(mappers);
                return treeMapperData;
            }

        }
        IEnumerator GenerateModels(string urlPath, Action<float> progressUpdate)
        {

            GlbFactory glbFactory = GameManager.arManager.GetComponent<GlbFactory>();
            if (glbFactory == null)
            {
                glbFactory = GameManager.arManager.gameObject.AddComponent<GlbFactory>();
            }

            //第一步生成父物体,设置坐标和ar相机坐标
            string dataPath = IoTools.Model_IO.GetModelDownloadPath(urlPath);
            string jsonPath = Path.Combine(dataPath, "boundingbox.json");
            string jsonString = IoTools.ReadFileString(jsonPath);
            BoundingBoxData boundingBoxData = JsonConvert.DeserializeObject<BoundingBoxData>(jsonString);
            GameObject gameObj = new GameObject(boundingBoxData.ModelName);
            gameObj.transform.position = Vector3.zero;


            //设置模型和相机中点位置
            string[] pos = boundingBoxData.Center.Split(',');
            Vector3 centerPos = new Vector3(-float.Parse(pos[0]), float.Parse(pos[2]), -float.Parse(pos[1]));
            centerPos = centerPos * 0.3048f;
            gameObj.transform.position = centerPos;
            GameManager.arManager.transform.position = gameObj.transform.position;


            //第二步生成模型，绑定父物体
            yield return GameManager.arManager.StartCoroutine(glbFactory.CreateUrlGlb(urlPath, (t) => { progressUpdate.Invoke(0.8f * t); }, gameObj.transform));

            yield return GameManager.arManager.StartCoroutine(PipeFactory.GenerateUrlPipes(urlPath, (t) => { progressUpdate.Invoke(0.9f * t); }, gameObj.transform));

            yield return GameManager.arManager.StartCoroutine(SemanticFactory.CreateUrlSemantic(urlPath, (t) => { progressUpdate.Invoke(0.95f * t); }, gameObj.transform));

            GameManager.arManager.targetSceneModel = gameObj.transform;
            
            
            progressUpdate.Invoke(97);

            //绑定大模型
            targetSceneModel = GameManager.arManager.targetSceneModel;

            //生成虚拟
            if (GameManager.arManager.isIos == false)
            {
                GameObject arMeshEmu = Instantiate(targetSceneModel.gameObject);
                arMeshEmu.name = "模拟真实场景网格";
                for (int i = 0; i < arMeshEmu.transform.childCount; i++)
                {
                    Transform transTemp = arMeshEmu.transform.GetChild(i);
                    for (int j = 0; j < transTemp.childCount; j++)
                    {
                        Transform targetTrans = transTemp.GetChild(j);
                        targetTrans.gameObject.layer = ArEngine.TrackMeshLayer;
                        MeshRenderer mr = targetTrans.GetComponent<MeshRenderer>();
                        if (mr != null) mr.material = GameManager.arManager.trackingMeshMat;
                    }
                }
            }

            //包装模型
            for (int i = 0; i < targetSceneModel.childCount; i++)
            {
                Transform transTemp = targetSceneModel.GetChild(i);
                for (int j = 0; j < transTemp.childCount; j++)
                {
                    Transform tagetTrans = transTemp.GetChild(j);
                    PackingModel(tagetTrans);
                }
            }



            progressUpdate.Invoke(100);
            Debug.Log("所有模型加载完毕");
            void PackingModel(Transform trans)
            {
                MeshRenderer meshRender = trans.GetComponent<MeshRenderer>();
                if (meshRender == null) return;
                Collider[] colliders = trans.GetComponents<Collider>();
                foreach (var item in colliders)
                {
                    Destroy(item);
                }
                trans.gameObject.AddComponent<MeshCollider>();
                // 创建一个新的材质，使用标准着色器
                Material transparentMaterial = new Material(Shader.Find("Standard"));
                //transparentMaterial.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
                Material oldMat = meshRender.sharedMaterial;
                meshRender.material = transparentMaterial;
                meshRender.sharedMaterial.color = oldMat.color;
                meshRender.sharedMaterial.mainTexture = oldMat.mainTexture;
                Destroy(oldMat);
                trans.gameObject.layer = ArEngine.GenerateModelLayer;
                //这里添加动态材质组件
                trans.gameObject.AddComponent<ReflashMatMode>();

                //这里索引材质去treemapper
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> propertyData = GameManager.arManager.nowAllModelProperties;
                string uuid = ArManager.GetUuidByModelName(trans.name);
                string secondKey = propertyData[uuid]["基础信息"]["构件类别"];
                string firstKey = treeMapperChildFatherPair[secondKey];
                treeMapper[firstKey][secondKey].Add(meshRender.sharedMaterial);

                if (!GameManager.arManager.nowCombineModels.ContainsKey(uuid))
                {
                    GameManager.arManager.nowCombineModels.Add(uuid, new List<Transform>());
                }
                GameManager.arManager.nowCombineModels[uuid].Add(trans);

            }
        }
        void MakeAlphaSlider()
        {
            foreach (var bigGroup in treeMapper)
            {
                GameObject mainSliderTemp = GameObjectPoolTool.GetFromPoolForce(true, mainSliderPrefab.path);
                mainSliderTemp.transform.SetParent(mainSliderContainer);
                MainAlphaSlider_Element mainAlphaSlider_Element = mainSliderTemp.GetComponent<MainAlphaSlider_Element>();
                List<ChildAlphaSlider_Element> targetChildSliders = new List<ChildAlphaSlider_Element>();
                foreach (var littleGroup in bigGroup.Value)
                {
                    targetChildSliders.Add(GenerateChildSlider(littleGroup.Key, littleGroup.Value, mainAlphaSlider_Element));
                }
                mainAlphaSlider_Element.Ini(targetChildSliders, bigGroup.Key);
                mainSliders.Add(bigGroup.Key, mainAlphaSlider_Element);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(mainSliderContainer.GetComponent<RectTransform>());

            ChildAlphaSlider_Element GenerateChildSlider(string title, List<Material> mats, MainAlphaSlider_Element fatherSlider)
            {
                GameObject childSliderTemp = GameObjectPoolTool.GetFromPoolForce(true, childSliderPrefab.path);
                childSliderTemp.transform.SetParent(fatherSlider.childSliderFather.transform);
                ChildAlphaSlider_Element childAlphaSlider_Element = childSliderTemp.GetComponent<ChildAlphaSlider_Element>();
                childAlphaSlider_Element.Ini(mats, title);
                return childAlphaSlider_Element;
            }
        }
        void IniButtonEvent()
        {
            returnButton.onClick.AddListener(() =>
            {
                UiManager.NowBook.ChangePageTo(1);
            });
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

        }

    }





}
