using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class HttpTool : MonoBehaviour
{
    Action xxx;
    public static string token = "";
    private const string urlHead = "http://";
    private const string address = "175.178.56.185";
    public static string URL
    {
        get { return urlHead + address; }
    }

    /// <summary>
    /// get请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator GetRequest<T>(string urlTail, Action<T> onOver)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL + urlTail);
        request.SetRequestHeader("authorization", token);
        Debug.Log(request.url);

        // 发送请求并等待返回
        yield return request.SendWebRequest();

        // 如果发生错误，输出错误信息
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("API请求成功: " + request.downloadHandler.text);
            // 请求成功时调用回调函数，传递返回的文本数据
            if (typeof(T) == typeof(string))
            {
                onOver.Invoke((T)(object)request.downloadHandler.text);
            }
            else
            {
                onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
            }
        }
        else
        {
            Debug.LogError("API请求失败: " + request.error);
            // 在这里处理API请求失败的情况
            onOver.Invoke(default(T));
        }
    }

    /// <summary>
    /// post请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="urlTail"></param>
    /// <param name="form"></param>
    /// <param name="onOver"></param>
    /// <returns></returns>
    public static IEnumerator PostRequest<T>(string urlTail, WWWForm form, Action<T> onOver)
    {
        // 创建UnityWebRequest对象
        UnityWebRequest request = UnityWebRequest.Post(URL + urlTail, form);

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        // 处理响应
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("API请求成功: " + request.downloadHandler.text);
            // 在这里处理API响应
            onOver.Invoke(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError("API请求失败: " + request.error);
            // 在这里处理API请求失败的情况

            onOver.Invoke(default(T));
        }
    }

}

public class UrlTail
{
    public const string login = "/api/oauth/Login";
    public const string projectGantt = "/api/extend/ProjectGantt";
    public const string currentUser = "/api/oauth/CurrentUser";
}
