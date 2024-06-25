using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using com.baidu.ai.search;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

namespace com.baidu.ai
{
    public class FaceDetect : MonoBehaviour
    {

        public static FaceDetect instance;

        public List<string> group_id_list = new List<string>();
        string accessToken;

        private void Awake()
        {
            accessToken = AccessToken.getAccessToken();
            instance = this;
        }

        //[ContextMenu("关闭所有协程")]
        //private void StopAlMyCoroutine()
        //{
        //    StopAllCoroutines();
        //}

        //人脸搜索
        public IEnumerator SearchDetectAsync(string Base64Image, Action<FaceSearchInfo> callback)
        {
            string host = "https://aip.baidubce.com/rest/2.0/face/v3/search?access_token=" + accessToken;
            Encoding encoding = Encoding.Default;

            // 构建请求数据
            string groupIdListString = "";
            if (group_id_list.Count != 0 && group_id_list.Count <= 10)
            {
                for (int i = 0; i < group_id_list.Count; i++)
                {
                    groupIdListString += group_id_list[i];
                    if (i != group_id_list.Count - 1)
                    {
                        groupIdListString += ",";
                    }
                }
            }
            else
            {
                Debug.Log("搜索失败\n人脸库数量不对!\n请重新搜索");
                callback?.Invoke(null);
                yield break;
            }

            string requestData = "{\"image\":\"" + Base64Image + "\",\"image_type\":\"BASE64\",\"group_id_list\":\"" + groupIdListString + "\",\"quality_control\":\"LOW\",\"liveness_control\":\"NORMAL\"}";
            byte[] requestDataBytes = encoding.GetBytes(requestData);

            // 创建请求对象
            using (UnityWebRequest request = new UnityWebRequest(host, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(requestDataBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // 发送请求并等待响应
                yield return request.SendWebRequest();

                // 处理响应数据
                if (request.result == UnityWebRequest.Result.Success)
                {
                    //Debug.Log("人脸搜索返回信息:" + request.downloadHandler.text);
                    FaceSearchInfo faceSearchInfo = JsonConvert.DeserializeObject<FaceSearchInfo>(request.downloadHandler.text);
                    callback?.Invoke(faceSearchInfo);
                }
                else
                {
                    Debug.LogError("人脸搜索请求失败: " + request.error);
                    callback?.Invoke(null);
                }
            } // 使用using语句确保资源被释放


        }
        string CombineMsg(string title, string value)
        {
            try
            {
                string result = title + ":" + value + "\n";
                return result;
            }
            catch
            {
                return title + ":未能识别\n";
            }
        }

        //获取人脸搜索结果信息
        public string GetFaceID(FaceSearchInfo faceInfo)
        {
            if (faceInfo == null) return "";

            if (faceInfo.error_code == 0)
            {
                if (faceInfo.result.user_list[0].score >= 80)
                {

                    return faceInfo.result.user_list[0].user_id;
                }
                Debug.Log("分数太低不显示");
                return "";
            }
            else if (faceInfo.error_code == 222207)
            {
                Debug.Log("用户不存在\n请注册");
                return "";
            }
            else
            {
                Debug.Log("无法解析，原因可能是网络中断\n人脸不存在图像\n图像质量低\n请再次尝试搜索");
                return "";
                // text_searchState.text = "无法解析，原因可能是网络中断\n人脸不存在图像\n图像质量低\n请再次尝试搜索";
            }


        }


        /// <summary>
        /// 图片转换成base64编码文本
        /// </summary>
        public string ImgToBase64string(string path)
        {

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            string base64string = Convert.ToBase64String(buffer);
            //    Debug.Log("获取当前图片base64为---" + base64string);
            return base64string;
        }

        /// <summary>
        /// base64编码文本转换成Texture
        /// </summary>

        private Texture2D Base64ToTexter2d(string Base64STR)
        {
            Texture2D pic = new Texture2D(200, 200);
            byte[] data = System.Convert.FromBase64String(Base64STR);
            //File.WriteAllBytes(Application.dataPath + "/BuildImage/Base64ToSaveImage.png", data);
            pic.LoadImage(data);
            return pic;
        }

        /// <summary>
        /// base64编码文本转换成图片
        /// </summary>

        public void Base64ToImg(Image imgComponent, string str)
        {
            string base64 = str;
            byte[] bytes = Convert.FromBase64String(base64);
            Texture2D tex2D = new Texture2D(100, 100);
            tex2D.LoadImage(bytes);
            Sprite s = Sprite.Create(tex2D, new UnityEngine.Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
            imgComponent.sprite = s;
            Resources.UnloadUnusedAssets();
        }

    }
}