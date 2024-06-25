using COSXML.Transfer;
using COSXML;

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;
using COSXML.Auth;
using COSXML.Model.Bucket;
using COSXML.Model.Object;

using COSXML.Common;

public class WorkerInfo_Window : UiWindow
{
    public Text[] allText;
    public Image photo;
    public Button closeButton;
    public GameObject disciplinaryPrefab;
    public Button[] changePages;
    public GameObject[] pages;
    public Transform content;
    static Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
    private DisciplinarySend disciplinarySend;
    public  DisciplinaryAdd disciplinaryAdd;
    private string userName;
    private string teamName;
    private string typeName;
    private string id;
    public Text[] disciplinaryText;
    public Button takePhoto;
    public InputField inputField;
    public static string sendUrl;
    private void Start()
    {
        
        disciplinarySend = new DisciplinarySend
        {
            currentPage = 1,
            pageSize = 1000,
            name = ""
        };
        disciplinaryAdd = new DisciplinaryAdd
        {
            userName = ""
        };
        closeButton.onClick.AddListener(() =>
        {
            CloseUi();
        });
        changePages[0].onClick.AddListener(() => { ChangePages(0, 0); });
        changePages[1].onClick.AddListener(() => { ChangePages(1, 1); });
        changePages[2].onClick.AddListener(() => { ChangePages(2, 2); });
        changePages[3].onClick.AddListener(() => { ChangePages(3, 3); StartCoroutine(TestWorkInfoInfo()); });
        changePages[4].onClick.AddListener(() => { ChangePages(4, 4); });
        changePages[5].onClick.AddListener(() => { ChangePages(3, 5); });
        changePages[6].onClick.AddListener(() => { StartCoroutine(AddInfo()); ChangePages(5, 6);  });
        changePages[7].onClick.AddListener(() => { ChangePages(3, 7); });
        takePhoto.onClick.AddListener(() =>
        {
            StartCoroutine(ScreenShotTool.ScreenShoot((sprite) =>
            {
                takePhoto.GetComponent<Image>().sprite = sprite;


            }));
         
        });
    }
    public void SetData(WorkerData data)
    {


        
        teamName = data.result.TeamName;
        typeName = data.result.TypeName;
        id = data.result.Id;
        userName = data.result.UserName;
        photo.gameObject.SetActive(false);
        allText[0].text = "姓名：" + data.result.UserName;
        allText[1].text = "年龄：" + data.result.Age;
        allText[2].text = "工种：" + data.result.TypeName;
        allText[3].text = "班组：" + data.result.TeamName;
        allText[4].text = "性别：" + data.result.Sex;
        allText[5].text = "民族：" + data.result.Nation;
        allText[6].text = "进场时间：" + TimeTool.TimeStamp2Str(data.result.EnterTime);
        allText[7].text = "出场时间：" + TimeTool.TimeStamp2Str(data.result.OutTime);
        allText[8].text = "身份证号：" + data.result.Id;
        allText[9].text = "电话号码：" + data.result.Phone;
        allText[10].text = "家庭住址：" + data.result.Address;

        disciplinaryText[0].text = "违纪人员姓名：" + data.result.UserName;
        disciplinaryText[1].text = "班组名称：" + data.result.TeamName;
        disciplinaryText[2].text = "班组类型：" + data.result.TypeName;
        disciplinaryText[3].text = "身份证号：" + data.result.Id;
        
        List<Photo> photos = JsonConvert.DeserializeObject<List<Photo>>(data.result.Photo);
        if (photos.Count != 0)
        {
            string photoUrlTail = photos[0].url;
            string fullUrl = HttpTool.URL + photoUrlTail;

            if (loadedSprites.ContainsKey(fullUrl))
            {
                photo.sprite = loadedSprites[fullUrl];
                photo.gameObject.SetActive(true);
            }
            else
            {
                IEnumerator setImage = HttpTool.GetImage(fullUrl, (sprite) =>
                   {
                       photo.sprite = sprite;
                       photo.gameObject.SetActive(true);
                       if (!loadedSprites.ContainsKey(fullUrl))
                       {
                           loadedSprites.Add(fullUrl, sprite);
                       }
                   });

                StartCoroutine(setImage);
            }
        }
    }
    IEnumerator TestWorkInfoInfo()
    {
        disciplinarySend.name = userName;
        IEnumerator workInfoRoutine = HttpTool.PostRequestTest<RequestClass>(HttpTool.projectToken, UrlTail.filterDisciplineList, (xxx) =>
        {


            DataJson dataJson = JsonConvert.DeserializeObject<DataJson>(xxx.data.ToString());
            List<string> info = new List<string>();
            foreach (var item in dataJson.list)
            {
                
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(item.Time);

                    // 获取 UTC 时间
                    DateTime utcDateTime = dateTimeOffset.LocalDateTime;
                    string formattedDateTime = utcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string a = "日期：" + utcDateTime + " 违纪类型：" + item.TypeName + " 违纪事项：" + item.CauseContent;
                    info.Add(a);
                
            }


            GenarateContent(info);
        }, disciplinarySend);
        yield return workInfoRoutine;

    }
    IEnumerator AddInfo()
    {
        Program.UploadImage();
        disciplinaryAdd.userName = userName;
        disciplinaryAdd.teamName = teamName;
        disciplinaryAdd.teamType = typeName;
        disciplinaryAdd.idCard = id;
        disciplinaryAdd.cause = inputField.text;
        disciplinaryAdd.picPath = WorkerInfo_Window.sendUrl;
        disciplinaryAdd.creatorTime = System.DateTime.Now.ToString();
        IEnumerator workInfoRoutine = HttpTool.PostRequestTest<RequestClass>(HttpTool.projectToken, UrlTail.saveDisciplineList, (xxx) =>
        {
           Debug.Log( xxx.data.ToString());
        }, disciplinaryAdd);
        yield return workInfoRoutine;

    }
    public void ChangePages(int page, int button)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (i == page)
            {
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                pages[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (i == button)
            {
                changePages[i].GetComponent<Image>().color = new Color(0.06f, 0, 1, 1);
            }
            else
            {
                changePages[i].GetComponent<Image>().color = new Color(0.35f, 0.52f, 0.86f, 1);
            }
        }
    }
    public void GenarateContent(List<string> data)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 根据数组长度生成Text对象
        foreach (string message in data)
        {
            GameObject newText = Instantiate(disciplinaryPrefab, content);
            newText.transform.GetComponent<Text>().text = message;
        }

        // 调整Content大小
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        float allDelta = 0;
        foreach (RectTransform rect in content)
        {
            allDelta += rect.sizeDelta.y;
        }
        allDelta += 1;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, allDelta);

    }

}
public class Program
{

    internal static string bucket = @"hhy-cos-1325356865";

    
    internal static void UploadImage()
    {

        // 腾讯云 SecretId
        //string secretId = Environment.GetEnvironmentVariable("SECRET_ID");
        string secretId = "AKIDLZgeSG1vgDlCQVXVLKAtSsATDtLua9Zm";
        // 腾讯云 SecretKey
        //string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
        string secretKey = "MWR7ZnKdifU7qX1INMF2dK0azHf0LZbQ";
        // 存储桶所在地域
        string region = "ap-shanghai";

        // 普通初始化方式
        CosXmlConfig config = new CosXmlConfig.Builder()
            .SetRegion(region)
            .SetDebugLog(true)
            .Build();

        

        long keyDurationSecond = 600;
        QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, keyDurationSecond);

        // service 初始化完成
        CosXmlServer cosXml = new CosXmlServer(config, qCloudCredentialProvider);
        //.cssg-snippet-body-start:[transfer-upload-file]
        // 初始化 TransferConfig
        TransferConfig transferConfig = new TransferConfig();

        // 初始化 TransferManager
        TransferManager transferManager = new TransferManager(cosXml, transferConfig);
       

        //对象在存储桶中的位置标识符，即称对象键
        String cosPath = "ar_huiyan/discipline/"+ new FileInfo(IoTools.photoPathToSend).Name;
        WorkerInfo_Window.sendUrl = "https://hhy-cos-1325356865.cos.ap-shanghai.myqcloud.com/ar_huiyan/discipline/" + new FileInfo(IoTools.photoPathToSend).Name;
        // 上传对象
        COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, cosPath);
        uploadTask.SetSrcPath(IoTools.photoPathToSend);

        transferManager.UploadAsync(uploadTask);
        Debug.Log(transferManager.UploadAsync(uploadTask).Result.eTag);
        


    }

}
public class DisciplinarySend
{
    public int currentPage { get; set; }
    public int pageSize { get; set; }
    public string name { get; set; }
}
public class DataJson
{
    public List<DisciplinaryBack> list { get; set; }
    public int total { get; set; }
}
public class DisciplinaryBack
{
    public string TypeName { get; set; }
    public string TeamTypeName { get; set; }
    public string WorkerName { get; set; }
    public string CauseContent { get; set; }
    public long Time { get; set; }
    public long? ProjectId { get; set; }
    public long? UserId { get; set; }
    public string UserName { get; set; }
    public long? TeamId { get; set; }
    public string TeamName { get; set; }
    public string Cause { get; set; }
    public string TeamType { get; set; }
    public long? CreatorTime { get; set; }
    public object Penalties { get; set; }
    public string PicPath { get; set; }
    public long? IdCard { get; set; }
    public long? Id { get; set; }

}
public class DisciplinaryAdd
{
    public string id { get; set; }
    public string projectId { get; set; }
    public string userId { get; set; }
    public string userName { get; set; }
    public string teamId { get; set; }
    public string teamName { get; set; }
    public string cause { get; set; }
    public string teamType { get; set; }
    public string creatorTime { get; set; }
    public string penalties { get; set; }
    public string picPath { get; set; }
    public string idCard { get; set; }

}