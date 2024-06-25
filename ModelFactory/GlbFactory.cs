using Newtonsoft.Json;
using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GlbCatalogData;
using static GlbConstructNodeData;
using static GlbConstructNodeData2;

public class GlbFactory : MonoBehaviour
{
    public IEnumerator CreateUrlGlb(string modelUrlPath, Action<float> progressUpdate = null, Transform root = null)
    {
        if (progressUpdate != null) progressUpdate.Invoke(0);
        GameObject fatherGameObj = new GameObject("GLB模型");
        Transform fatherTrans = fatherGameObj.transform;
        fatherTrans.parent = root;

        //第一步生成glb目录的实体
        string dataPath = IoTools.Model_IO.GetModelDownloadPath(modelUrlPath);
        string jsonPath = Path.Combine(dataPath, "modelList", "modelList.json");
        string jsonString = IoTools.ReadFileString(jsonPath);
        Dictionary<string, Transform> glbCatalog = new Dictionary<string, Transform>();
        Dictionary<string, Transform> modelTempCatalog = new Dictionary<string, Transform>();
        if (jsonString != "")
        {
            List<GlbClass> GlbDatas = JsonConvert.DeserializeObject<List<GlbClass>>(jsonString);
            // GlbConstructData glbConstructData = new GlbConstructData(GlbDatas);
            Debug.Log("glb数量：" + GlbDatas.Count);
            int okTime = 0;
            for (int i = 0; i < GlbDatas.Count; i++)
            {
                string glbKey = GlbDatas[i].category;
                string glbPath = dataPath + glbKey + ".glb";
                //Debug.Log(dataPath);
                //Debug.Log(glbKey);
                StartCoroutine(CreateGlb(glbPath, (gameObj) =>
                {
                    glbCatalog.Add(glbKey, gameObj.transform);

                    for (int i = 0; i < gameObj.transform.childCount; i++)
                    {
                        Transform temp = gameObj.transform.GetChild(i);
                        if (!modelTempCatalog.ContainsKey(temp.name))
                        {
                            modelTempCatalog.Add(temp.name, temp);
                        }
                    }

                    okTime++;
                }));
            }

            while (okTime != GlbDatas.Count)
            {
                yield return new WaitForEndOfFrame();

                if (progressUpdate != null)
                {
                    float percent = ((float)okTime) / GlbDatas.Count * 80;
                    progressUpdate.Invoke(percent);
                }
            }
        }
        if (progressUpdate != null) progressUpdate.Invoke(80);
        Debug.Log("glb目录加载完毕");
        yield return new WaitForEndOfFrame();


        //第二步，通过目录的索引，生成所有node1构建
        jsonPath = Path.Combine(dataPath, "nodes", "nodes.json");
        jsonString = "";
        jsonString = IoTools.ReadFileString(jsonPath);
        if (jsonString != "")
        {
            // GlbConstructNodeData
            Dictionary<string, List<Primitive>> primitive = JsonConvert.DeserializeObject<Dictionary<string, List<Primitive>>>(jsonString);
            GlbConstructNodeData glbConstructNodeData = new GlbConstructNodeData(primitive);
            float targetTime = glbConstructNodeData.Primitives.Count;


            foreach (var item in glbConstructNodeData.Primitives)
            {
                Transform targetTempTrans = glbCatalog[item.Key];
                foreach (var node in item.Value)
                {
                    CreateModel(node, targetTempTrans, (gameObj) =>
                    {
                        gameObj.transform.parent = fatherTrans;
                    });
                }
            }
        }
        if (progressUpdate != null) progressUpdate.Invoke(90);
        Debug.Log("node1模型生成完毕");
        yield return new WaitForEndOfFrame();

        //第三步，通过目录的索引，生成所有node2构建
        jsonPath = Path.Combine(dataPath, "instanceList", "instanceList.json");
        jsonString = "";
        jsonString = IoTools.ReadFileString(jsonPath);
        if (jsonString != "")
        {
            //List<Category>
            List<Category> Categories = JsonConvert.DeserializeObject<List<Category>>(jsonString);
            foreach (var category in Categories)
            {
                foreach (var item in category.children)
                {
                    GameObject tempGameObj = modelTempCatalog[item.meshId].gameObject;
                    CreateModelsByTemp(item, tempGameObj, (gameObjs) =>
                    {
                        foreach (var item in gameObjs)
                        {
                            item.transform.parent = fatherTrans;
                        }
                    });
                }
            }
        }
        if (progressUpdate != null) progressUpdate.Invoke(95);
        Debug.Log("node2模型生成完毕");
        yield return new WaitForEndOfFrame();

        //第四步，释放目录
        foreach (var item in glbCatalog)
        {
            Destroy(item.Value.gameObject);
        }
        glbCatalog.Clear();
        modelTempCatalog.Clear();
        if (progressUpdate != null) progressUpdate.Invoke(100);
        Debug.Log("glb加载完毕");
        yield return new WaitForEndOfFrame();
    }

    public static IEnumerator CreateGlb(string path, Action<GameObject> onOver)
    {
        bool ok = false;
        GameObject targetGameObj = null;
        Importer.LoadFromFileAsync(path, new ImportSettings(), (gameObject, Anims) =>
        {
            ok = true;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform xx = gameObject.transform.GetChild(i);
                if (xx.GetComponent<MeshRenderer>() == null)
                {
                    Destroy(xx.gameObject);
                }
            }
            targetGameObj = gameObject;
        }, (percent) =>
        {

            Debug.Log(percent);

        });


        yield return new WaitUntil(() => ok);
        onOver.Invoke(targetGameObj);
    }


    public static void CreateModel(Primitive nodeData, Transform tempTrans, Action<GameObject> onOver = null)
    {
        string modelName = nodeData.Name;
        int childIndex = nodeData.Mesh;
        List<float> matrixData = nodeData.Matrix;

        //然后是克隆构件
        GameObject createObj = tempTrans.GetChild(childIndex).gameObject;
        //矩阵转
        Matrix4x4 matrix4X4 = new Matrix4x4()
        {
            m00 = matrixData[0],
            m10 = matrixData[1],
            m20 = matrixData[2],
            m30 = matrixData[3],
            m01 = matrixData[4],
            m11 = matrixData[5],
            m21 = matrixData[6],
            m31 = matrixData[7],
            m02 = matrixData[8],
            m12 = matrixData[9],
            m22 = matrixData[10],
            m32 = matrixData[11],
            m03 = matrixData[12],
            m13 = matrixData[13],
            m23 = matrixData[14],
            m33 = matrixData[15],
        };
        Vector3 targetPostion = new Vector3(-matrixData[12], matrixData[13], matrixData[14]);
        GameObject targetGameObject = Instantiate(createObj);
        targetGameObject.transform.parent = null;
        targetGameObject.transform.position = targetPostion;
        Vector3 originalEulerAngle = matrix4X4.rotation.eulerAngles;
        targetGameObject.transform.rotation = Quaternion.Euler(originalEulerAngle.x, -originalEulerAngle.y, -originalEulerAngle.z);
        targetGameObject.name = modelName;

        //  Debug.Log("生成了一个模型");

        if (onOver != null)
        {
            onOver.Invoke(targetGameObject);
        }
    }


    static void CreateModelsByTemp(GlbConstructNodeData2.MeshChild tempInfo, GameObject temp, Action<List<GameObject>> onOver = null)
    {
        List<GameObject> generatedModels = new List<GameObject>();
        foreach (var item in tempInfo.children)
        {
            string modelName = item.name;
            List<float> matrixData = item.matrix;

            //矩阵转
            Matrix4x4 matrix4X4 = new Matrix4x4()
            {
                m00 = matrixData[0],
                m10 = matrixData[1],
                m20 = matrixData[2],
                m30 = matrixData[3],
                m01 = matrixData[4],
                m11 = matrixData[5],
                m21 = matrixData[6],
                m31 = matrixData[7],
                m02 = matrixData[8],
                m12 = matrixData[9],
                m22 = matrixData[10],
                m32 = matrixData[11],
                m03 = matrixData[12],
                m13 = matrixData[13],
                m23 = matrixData[14],
                m33 = matrixData[15],
            };
            Vector3 targetPostion = new Vector3(-matrixData[12], matrixData[13], matrixData[14]);
            GameObject targetGameObject = Instantiate(temp);
            targetGameObject.transform.parent = null;
            targetGameObject.transform.position = targetPostion;
            Vector3 originalEulerAngle = matrix4X4.rotation.eulerAngles;
            targetGameObject.transform.rotation = Quaternion.Euler(originalEulerAngle.x, -originalEulerAngle.y, -originalEulerAngle.z);
            targetGameObject.name = modelName;
            generatedModels.Add(targetGameObject);
        }
        //  Debug.Log("生成了一类模型");
        if (onOver != null)
        {
            onOver.Invoke(generatedModels);
        }
    }
}
