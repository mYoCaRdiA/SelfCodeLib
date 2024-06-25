using com.baidu.ai;
using com.baidu.ai.search;
using Newtonsoft.Json;
using NN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FaceManager : MonoBehaviour
{

    public float cdTime = 1;
    public static FaceManager instance;

    public float maxDisOffset = 100f;
    List<FaceInfo> nowFaces = new List<FaceInfo>();

    private Queue<FaceInfo> waitForDateFaces = new Queue<FaceInfo>();


    private void Start()
    {
        instance = this;

    }

    void OnEnable()
    {
        StartCoroutine(GetFaceDate());
        StartCoroutine(DrawInfoCoroutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }


    //处理人脸数据队列
    IEnumerator GetFaceDate()
    {
        nowFaces.Clear();
        waitForDateFaces.Clear();
        float passTime = 0;
        while (true)
        {
            if (waitForDateFaces.Count != 0)
            {
                passTime += Time.deltaTime;
                FaceInfo targetFace = waitForDateFaces.Dequeue();
                //判定人脸信息是否是当前识别中的信息
                if (nowFaces.Contains(targetFace))
                {
                    //拿到百度人脸信息
                    FaceSearchInfo faceSearchInfo = null;
                    if (passTime > cdTime)
                    {
                        yield return StartCoroutine(FaceDetect.instance.SearchDetectAsync(targetFace.base64Data, (info) =>
                        {
                            faceSearchInfo = info;
                        }));
                        passTime = 0;
                    }

                    string faceid = "";
                    if (faceSearchInfo != null)
                    {
                        faceid = FaceDetect.instance.GetFaceID(faceSearchInfo);
                    }

                    //判定百度是否回馈，如果不回，则继续入队
                    if (faceid == "")
                    {
                        //这里判断是否是有必要入队的人脸（如果人离开就没必要）
                        if (!waitForDateFaces.Contains(targetFace))
                        {
                            waitForDateFaces.Enqueue(targetFace);
                        }
                        targetFace.showText = "正在识别..";
                    }
                    else
                    {
                        //这里已经拿到了人脸id
                        //执行协程让补充人脸信息id，显示文本
                        IEnumerator workInfoRoutine = HttpTool.GetRequest<RequestClass>(HttpTool.projectToken, UrlTail.workerInfo, (xxx) =>
                        {
                            WorkerData workerData = new WorkerData();
                            workerData = JsonConvert.DeserializeObject<WorkerData>(xxx.data.ToString());
                            if (xxx.data != null)
                            {
                                targetFace.faceId = faceid;
                                targetFace.showText = workerData.result.UserName + " " + workerData.result.Age + "岁\n" + workerData.result.TeamName;

                            }
                        }, new KeyValuePair<string, string>("id", faceid));
                        yield return StartCoroutine(workInfoRoutine);
                    }
                }
              
            }
            yield return new WaitForEndOfFrame();
        }
    }


    //注册人脸
    public void CheckOutFaces(List<ResultBox> personResultBoxs, Texture2D targetImg)
    {
        List<int> updatedBoxIndex = new List<int>();
        List<FaceInfo> updatedExistFace = new List<FaceInfo>();

        //更新现有的人脸盒子位子信息和图片数据
        for (int i = 0; i < personResultBoxs.Count; i++)
        {
            ResultBox box = personResultBoxs[i];

            for (int j = 0; j < nowFaces.Count; j++)
            {
                FaceInfo face = nowFaces[j];
                if (!updatedExistFace.Contains(face))
                {
                    if (Vector2.Distance(face.resultBox.rect.position, box.rect.position) < maxDisOffset)
                    {
                        updatedBoxIndex.Add(i);
                        updatedExistFace.Add(face);
                        face.resultBox = box;
                        face.base64Data = SliceTexture2DToBase64(targetImg, box.rect);
                        break;
                    }
                }
            }
        }

        //移除旧的丢失的盒子
        nowFaces.Clear();
        nowFaces = updatedExistFace;

        //新增人脸盒子
        for (int i = 0; i < personResultBoxs.Count; i++)
        {
            if (!updatedBoxIndex.Contains(i))
            {
                ResultBox box = personResultBoxs[i];
                string base64data = SliceTexture2DToBase64(targetImg, box.rect);

                FaceInfo face = new FaceInfo(box, base64data);
                nowFaces.Add(face);
                waitForDateFaces.Enqueue(face);
            }
        }

     //   DrawInfo();
    }




    IEnumerator DrawInfoCoroutine()
    {
        while (true)
        {
            DrawInfo();
            yield return new WaitForSeconds(0.2f);
        }
    }
    void DrawInfo()
    {
        Detector.instance.ClearDraw();
        foreach (var item in nowFaces)
        {
            Detector.instance.ShowBox(item.resultBox, null);
            Detector.instance.ShowInfo(item.resultBox, item.showText, item);
        }
    }


    // 使用Rect参数来截取Texture2D的一部分，转换为字节流，然后转换为Base64字符串
    public string SliceTexture2DToBase64(Texture2D originalTexture, Rect rect)
    {
        // 调整Rect的y坐标，以确保截取的位置正确
        // Unity的Texture2D坐标原点在左下角，但在很多情况下（如UI）我们习惯于以左上角为原点
        rect.y = originalTexture.height - rect.y - rect.height;

        // 确保rect不会超出原始纹理的边界
        rect.x = Mathf.Clamp(rect.x, 0, originalTexture.width);
        rect.y = Mathf.Clamp(rect.y, 0, originalTexture.height);
        rect.width = Mathf.Clamp(rect.width, 0, originalTexture.width - rect.x);
        rect.height = Mathf.Clamp(rect.height, 0, originalTexture.height - rect.y);

        // 创建新的Texture2D，尺寸与截取区域相匹配
        Texture2D newTexture = new Texture2D((int)rect.width, (int)rect.height, originalTexture.format, false);

        // 读取指定区域的像素
        Color[] pixels = originalTexture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

        // 将读取的像素应用到新Texture2D上
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        // 将新Texture2D转换为字节流，这里以PNG格式为例
        byte[] bytes = newTexture.EncodeToPNG();

        // 销毁临时创建的Texture2D对象
        Destroy(newTexture);

        // 将字节流转换为Base64字符串
        string base64String = Convert.ToBase64String(bytes);

        // 返回Base64字符串
        return base64String;
    }
}



public class FaceInfo
{
    public ResultBox resultBox;
    public string base64Data;
    public string showText;
    public string faceId;
    //public Action clickEvent;


    public IEnumerator OnClick()
    {
        //Debug.LogError("点击了");
        IEnumerator workInfoRoutine = HttpTool.GetRequest<RequestClass>(HttpTool.projectToken, UrlTail.workerInfo, (xxx) =>
        {
            WorkerData workerData = new WorkerData();
            workerData = JsonConvert.DeserializeObject<WorkerData>(xxx.data.ToString());
            GameManager.uiManager.ShowUiWindow<WorkerInfo_Window>(workerData);
        }, new KeyValuePair<string, string>("id", faceId));
        yield return workInfoRoutine;
    }

    // 构造函数，接受rectPos、base64Data和key作为参数
    public FaceInfo(ResultBox resultBox, string base64Data)
    {
        this.resultBox = resultBox;
        this.base64Data = base64Data;
    }
}
