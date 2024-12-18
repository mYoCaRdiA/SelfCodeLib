using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Newtonsoft.Json;
using System;


public class ArManager : Manager
{
    public Transform targetSceneModel;

    [HideInInspector]
    public Material trackingMeshMat;
    public Light targetLight;
    public Shader trackingMeshShader_Occlusion;
    public Shader trackingMeshShader_Normal;
    public ARSession arSession;
    public GameObject ball;

    //模型uuid对应的属性数据字典
    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> nowAllModelProperties = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
    //模型uuid对应的模型组合整体
    public Dictionary<string, List<Transform>> nowCombineModels = new Dictionary<string, List<Transform>>();

    public ModelAlphaControlPage_Static modelAlphaControlPage_Static;
    public ArPublicPanel_Static arPublicPanel_Static;
    public FaceDetectPage_Static faceDetectPage_Static;
    public PictureLocatePage_Static pictureLocatePage_Static;


    public float NowAllModelAlpha
    {
        get
        {
            return GameManager.arManager.arPublicPanel_Static.allModelAlphaSlider.value;
        }
    }

    private ArEngine arEngine;
    public bool isIos;

    public override void Ini()
    {
        isIos = Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.LinuxEditor;

        if (isIos == true)
        {
            arEngine = new ArEngineReal(arSession, targetLight);
        }
        else
        {
            arEngine = new ArEngineEmu(arSession, targetLight);
        }

        if (GameManager.arManager != null)
        {
            Destroy(GameManager.arManager);
        }
        trackingMeshMat = new Material(trackingMeshShader_Normal);
        SetTrackingMeshType(TrackingMeshType.Normal);

        SetSceneModelToCentre();
        GameManager.arManager = this;

        StartCoroutine(modelAlphaControlPage_Static.Ini());

        faceDetectPage_Static.Ini();

        if (HttpTool.projectToken == "")
        {
            SetTestToken(() => pictureLocatePage_Static.Ini());
        }
        else
        {

            pictureLocatePage_Static.Ini();
        }

    }

    public static string GetUuidByModelName(string nameStr)
    {
        string uuid = nameStr.Split("][")[1];
        uuid = uuid.Replace("]", "");
        return uuid;
    }

    void SetTestToken(Action onOver)
    {
        StartCoroutine(GetToken());

        IEnumerator GetToken()
        {
            // projectId = 540741826479392006
            WWWForm form = new WWWForm();
            form.AddField("account", "admin");
            form.AddField("password", HashTool.GenerateMD5("hhsz0615"));

            RequestClass requestEntity = null;
            IEnumerator post = HttpTool.PostRequest<RequestClass>(HttpTool.loginToken, UrlTail.login, (result) => { requestEntity = result; }, form);
            yield return post;
            TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(requestEntity.data.ToString());

            RequestClass targetData = null;
            HttpTool.loginToken = tokenData.token;
            IEnumerator requestModelSelectInfo = HttpTool.GetRequest<RequestClass>(tokenData.token, UrlTail.projectToken, (result) =>
            {
                targetData = result;
            }, new KeyValuePair<string, string>("projectId", "364571918214366470"));
            yield return StartCoroutine(requestModelSelectInfo);
            TokenData tokenData2 = JsonConvert.DeserializeObject<TokenData>(targetData.data.ToString());
            HttpTool.projectToken = tokenData2.token;
            if (GameManager.dataManager.nowProjectId == "")
            {
                GameManager.dataManager.nowProjectId = "364571918214366470";
            }
            onOver.Invoke();
        }

    }


    public void SetAllModelAlpha(float value)
    {
        modelAlphaControlPage_Static.SetAllModelAlpha(value);
        //if (classifyClarityPanel_Static!=null) {
        //    classifyClarityPanel_Static.SetAllModelAlpha(value); }
        arPublicPanel_Static.allModelAlphaSlider.value = value;
    }

    public void SwitchShowSceneModelEntity(bool active)
    {
        targetSceneModel.gameObject.SetActive(active);
    }

    public void SetSceneModelToCentre()
    {
        targetSceneModel.transform.rotation = Quaternion.identity;
        targetSceneModel.transform.position = Vector3.zero;
    }

    private void Update()
    {
        //RayCastTest();
        arEngine.RunPerFrame();
    }

    /// <summary>
    /// ˫�㶨λģ��λ��
    /// </summary>
    public void LocateModel_TwoPoint()
    {
        StartCoroutine(LocateModelRoutine());

        IEnumerator LocateModelRoutine()
        {
            Vector3 point1 = Vector3.zero;
            Vector3 point2 = Vector3.zero;
            Vector3 point3 = Vector3.zero;
            Vector3 point4 = Vector3.zero;

            UiWindow window = GameManager.uiManager.ShowUiWindow<ViewTip_Window>("����ģ�͵�λ1");
            yield return null;
        }
    }


    void RayCastTest()
    {
        // ��������Ļʱ��������
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (GameManager.uiManager.IsOnUIElement(mousePos) == false)
            {
                MyArHitInfo hit = ArEngine.ScreenRaycast(mousePos);
                if (hit != null)
                {
                    Instantiate(ball, hit.pointPostion, Quaternion.identity);
                }
            }
        }
    }



    public void SwitchLight(bool isReal)
    {
        arEngine.SwitchLight(isReal);
    }

    public void SetTrackingMeshType(TrackingMeshType type)
    {
        switch (type)
        {
            case TrackingMeshType.Normal:
                trackingMeshMat.shader = trackingMeshShader_Normal;
                trackingMeshMat.SetFloat("_Transparency", 0.3f);
                break;
            case TrackingMeshType.Occlusion:
                trackingMeshMat.shader = trackingMeshShader_Occlusion;
                break;
            case TrackingMeshType.InVisiable:
                trackingMeshMat.shader = Shader.Find("Standard");
                trackingMeshMat.color = new Color(1, 1, 1, 0);
                SetMaterialToTransparent(trackingMeshMat);
                //trackingMeshMat.shader = trackingMeshShader_Normal;
                //trackingMeshMat.SetFloat("_Transparency", 0f);
                break;
            default:
                Debug.LogError("��Ч��TrackingMeshTypeö������");
                break;
        }
    }


    void SetMaterialToTransparent(Material material)
    {
        // ������ȾģʽΪ͸��
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
    public void SetTrackingMeshType(int enumIndex)
    {
        TrackingMeshType type = (TrackingMeshType)enumIndex;
        SetTrackingMeshType(type);
    }




}


public enum TrackingMeshType
{
    Normal,
    InVisiable,
    Occlusion
}
