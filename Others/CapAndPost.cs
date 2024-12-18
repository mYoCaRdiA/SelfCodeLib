using System.Collections;
using UnityEngine;
using System;
using Rect = UnityEngine.Rect;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using System.Runtime.InteropServices;
using static OpenCVForUnityExample.AlphaBlendingExample;
using System.Net.NetworkInformation;
using System.IO;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using OpenCVForUnity.ImgprocModule;
using Newtonsoft.Json.Linq;
using UnityEngine.Experimental.Rendering;

public class CapAndPost : MonoBehaviour
{



    private RenderTexture renderTexture;
    public delegate void CallBack();
    private Texture2D screenShot;
    private Byte[] upLoad;
    private Texture2D texture;
    // Start is called before the first frame update
    void Start()
    {
        screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        StartCoroutine(Post());
        Loom.Initialize();


    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 每隔一秒发送
    /// </summary>
    /// <returns></returns>
    private IEnumerator Post()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            ScreenCapture1();
            Start_websocket(upLoad);
        }
    }

    /// <summary>
    /// 截取屏幕中心区域
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    private static Texture2D ChangeDrawTexture(Texture2D img)
    {
        var Texture = img;

        Sprite a = BornSprite(Texture);
        var targetTex = new Texture2D((int)a.rect.width, (int)a.rect.height);
        var pixels = a.texture.GetPixels(
            (int)a.textureRect.x,
            (int)a.textureRect.y,
            (int)a.textureRect.width,
            (int)a.textureRect.height);
        targetTex.SetPixels(pixels);
        targetTex.Apply();
        return targetTex;
    }
    /// <summary>
    /// 按照原图一半的比例再剪去需要截取大小的一半
    /// </summary>
    /// <param name="Texture"></param>
    /// <param name="Width"></param>
    /// <param name="Height"></param>
    /// <returns></returns>
    private static Sprite BornSprite(Texture2D Texture, int Width = 200, int Height = 200)
    {

        Sprite sp = Sprite.Create(Texture,
            new Rect(new Vector2(Texture.width / 2 - Width / 2, Texture.height / 2 - Height / 2),
                new Vector2(Width, Height)), new Vector2(0.5f, 0.5f));
        return sp;
    }
    /// <summary>
    /// 截图并保存
    /// </summary>
    /// <returns></returns>
    public void ScreenCapture1()
    {
        //yield return new WaitForEndOfFrame();
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();
        Texture2D screenshotTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshotTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshotTexture.Apply();
        screenShot = screenshotTexture;
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        //upLoad = screenShot.EncodeToJPG();                   
        Texture2D texture = new Texture2D(384, 216, TextureFormat.RGBA32, true);

        for (int i = 0; i < texture.height; i++)//压缩图片
        {
            for (int j = 0; j < texture.width; j++)
            {
                Color color = screenShot.GetPixel(j * 5, i * 5);
                texture.SetPixel(j, i, color);

            }
        }
        texture.Apply();
        Thread thread = new Thread(A);
        thread.Start();
        //upLoad = texture.EncodeToJPG();
    }

    private void A()
    {

        Loom.QueueOnMainThread((object param) =>
        {
            upLoad = texture.EncodeToJPG();
        }, null);


    }






    /// <summary>
    /// 发送图片到服务器
    /// </summary>
    async void Start_websocket(Byte[] post)
    {
        using (ClientWebSocket clientWebSocket = new ClientWebSocket())
        {

            Uri serverUri = new Uri("ws://10.168.1.125:3002/ws");
            CancellationToken cancellationToken = new CancellationToken();
            await clientWebSocket.ConnectAsync(serverUri, cancellationToken);
            // 发送消息
            string guid = (Guid.NewGuid()).ToString();
            var obj = new
            {
                SendClientId = guid,
                RoomNo = guid,
                action = "ToAnalyze",
                msg = JsonConvert.SerializeObject(post)
            };
            string message = JsonConvert.SerializeObject(obj);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(messageBytes);
            await clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
            // 接收消息
            byte[] receiveBuffer = new byte[1024];
            ArraySegment<byte> receiveBufferSegment = new ArraySegment<byte>(receiveBuffer);
            WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(receiveBufferSegment, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                Console.WriteLine("Received message: " + receivedMessage);
                Debug.Log(receivedMessage);
            }
            // 关闭连接
            try
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing WebSocket connection", cancellationToken);
            }
            catch { }
        }
    }



}
