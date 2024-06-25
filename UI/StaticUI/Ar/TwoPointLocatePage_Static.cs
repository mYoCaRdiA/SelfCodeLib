using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class TwoPointLocatePage_Static : StaticUi
{
    public RawImage fangdaImage; // 用于显示放大内容的Image组件
    public RectTransform fangdaMask;
    public float fangdaRate = 2f; // 放大倍数
    public Vector2 fangDaOffset=Vector2.zero;
    public bool firstClickOnScene = false;

    Texture2D cameraTexture;


    public Button returnButon;
    Transform pageFather;
    public Text tipText;

    public Button centerButton;
    public Button locateButton;

    public List<Button> pointSetButtons = new List<Button>();
    public PrefabInfo redBall;
    public PrefabInfo greenBall;

    int targetPointIndex = -1;


    List<string> pointInfo = new List<string>();
    Transform[] points = new Transform[4];

    [System.Obsolete]
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "NowVersion")
        {
            returnButon.gameObject.SetActive(false);
        }
        else
        {
            returnButon.gameObject.SetActive(true);

        }
        cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fangdaImage.texture = cameraTexture;
       
        fangdaImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        fangdaImage.transform.localScale *= fangdaRate;

        pageFather = transform.GetChild(0);
        pointInfo.Add("模型点位1");
        pointInfo.Add("模型点位2");
        pointInfo.Add("现实点位1");
        pointInfo.Add("现实点位2");


        for (int i = 0; i < pointSetButtons.Count; i++)
        {
            Button temp = pointSetButtons[i];
            temp.GetComponentInChildren<Text>().text = pointInfo[i];
            //这里给按钮赋值功能
            int index = i;
            temp.onClick.AddListener(() =>
            {
                Debug.Log(index);
                targetPointIndex = index;
                if (targetPointIndex <= 1)
                {
                    GameManager.arManager.SwitchShowSceneModelEntity(true);
                    GameManager.arManager.SetTrackingMeshType(TrackingMeshType.InVisiable);
                }
                else
                {
                    GameManager.arManager.SwitchShowSceneModelEntity(false);
                    GameManager.arManager.SetTrackingMeshType(TrackingMeshType.Normal);
                }
            });
        }

        returnButon.onClick.AddListener(() =>
        {
            GameManager.arManager.SetTrackingMeshType(TrackingMeshType.InVisiable);
            //UiManager.NowBook.ChangePageTo(1);
            if (SceneManager.GetActiveScene().name == "NowVersion")
            {   
                this.transform.FindChild("panel").gameObject.SetActive(true);
                returnButon.gameObject.SetActive(false);
            }
            else
            {
                UiManager.NowBook.ChangePageTo(1);

            }
            
        });

        centerButton.onClick.AddListener(() =>
        {
            GameManager.arManager.SetSceneModelToCentre();
        });
        locateButton.onClick.AddListener(OnLocateButtonClick);
    }

    private IEnumerator RenderScreenToTextureNextFrame()
    {
        // 等待下一帧
        yield return new WaitForEndOfFrame();

        // 读取屏幕上的像素数据到Texture2D中
        cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        cameraTexture.Apply();
    }

    private void Update()
    {
        ReflashTip();
        ReflashButtonColor();
        PointSet();
       
        CheckFangDa();
    }







    void CheckFangDa()
    {
        Vector3 mousePos = Input.mousePosition;

        bool mouseOnScene = !GameManager.uiManager.IsOnUIElement(mousePos);

        if (mouseOnScene && Input.GetMouseButtonDown(0))
        {
            firstClickOnScene = true;
            //fangdaMask.gameObject.SetActive(true);
        }
        else if (firstClickOnScene && mouseOnScene && Input.GetMouseButton(0))
        {
            StartCoroutine(RenderScreenToTextureNextFrame());
            fangdaMask.gameObject.SetActive(false);
            fangdaMask.transform.position = mousePos;
            Vector3 targetPos = fangdaMask.transform.localPosition;
            Vector3 offset = new Vector3(fangdaMask.sizeDelta.x / 2, fangdaMask.sizeDelta.x / 2, 0);

            fangdaImage.transform.localPosition = -targetPos * fangdaRate + offset;

            fangdaMask.transform.position +=(Vector3)fangDaOffset;
            fangdaMask.gameObject.SetActive(true);
        }
        else
        {
            fangdaMask.gameObject.SetActive(false);
            firstClickOnScene = false;
        }
    }



    private void OnEnable()
    {
        GameManager.arManager.arPublicPanel_Static.LockArMeshDropdown(true);
        pointSetButtons[0].onClick.Invoke();
        firstClickOnScene = false;
    }
    private void OnDisable()
    {
        RevertToStart();
        GameManager.arManager.arPublicPanel_Static.LockArMeshDropdown(false);
    }

    private void RevertToStart()
    {
        ClearAllPoint();
        targetPointIndex = -1;
        GameManager.arManager.SwitchShowSceneModelEntity(true);
        GameManager.arManager.SetTrackingMeshType(TrackingMeshType.InVisiable);
    }


    private void PointSet()
    {
        if (fangdaMask.gameObject.activeSelf == false)
        {
            return;
        }

        if (targetPointIndex != -1 && Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Input.mousePosition;
            if (GameManager.uiManager.IsOnUIElement(mousePos) == false)
            {
                PrefabInfo ballInfo;
                MyArHitInfo hitInfo;
                if (targetPointIndex <= 1)
                {
                    ballInfo = redBall;
                    hitInfo = ArEngine.ScreenRaycastGenerateModel(mousePos);
                }
                else
                {
                    ballInfo = greenBall;
                    hitInfo = ArEngine.ScreenRaycastTrackMesh(mousePos);
                }
                if (hitInfo != null)
                {
                    GameObject ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity.transform.SetParent(null);
                    ballEntity.transform.position = hitInfo.pointPostion;
                    if (points[targetPointIndex] != null)
                    {
                        GameObjectPoolTool.PutInPool(points[targetPointIndex].gameObject);
                    }
                    points[targetPointIndex] = ballEntity.transform;

                    if (targetPointIndex < points.Length - 1)
                    {
                        pointSetButtons[targetPointIndex + 1].onClick.Invoke();
                    }
                }
            }
        }
    }

    private void ReflashTip()
    {
        if (targetPointIndex != -1)
        {
            if (points[targetPointIndex] != null)
            {
                tipText.text = pointInfo[targetPointIndex] + "已设置，您可点击场景修改点位，或点击下方白色按钮切换目标点位设置";
            }
            else
            {
                tipText.text = "请点击场景设置点位：" + pointInfo[targetPointIndex];
            }
        }
        else
        {
            tipText.text = "请点击下方按钮，选择要设置的点位。";
        }

    }

    private void ReflashButtonColor()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
            {
                pointSetButtons[i].image.color = Color.green;
            }
            else
            {
                pointSetButtons[i].image.color = Color.white;
            }
        }
    }








    void OnLocateButtonClick()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                GameManager.uiManager.ShowUiWindow<Tip_Window>(pointInfo[i] + "未设置，请设置后重试");
                return;
            }
        }

        if (Vector3.Distance(points[0].position, points[1].position) <= 0.2f)
        {
            GameManager.uiManager.ShowUiWindow<Tip_Window>(pointInfo[0] + "和" + pointInfo[1] + "的距离小于0.2米，会导致精度太低，请重新调整这两个点位");
            return;
        }

        if (Vector3.Distance(points[0].position, points[1].position) <= 0.2f)
        {
            GameManager.uiManager.ShowUiWindow<Tip_Window>(pointInfo[2] + "和" + pointInfo[3] + "的距离小于0.2米，会导致精度太低，请重新调整这两个点位");
            return;
        }

        //开始定位模型
        StartCoroutine(LocateRoutine());
    }


    IEnumerator LocateRoutine()
    {
        GameManager.arManager.SwitchShowSceneModelEntity(true);
        GameManager.arManager.SetTrackingMeshType(TrackingMeshType.Normal);
        pageFather.gameObject.SetActive(false);

        MaskOnly_Window maskWin = GameManager.uiManager.ShowUiWindow<MaskOnly_Window>();
        ViewTip_Window viewWin = GameManager.uiManager.ShowUiWindow<ViewTip_Window>("请稍候，模型正在定位..");

        Transform targetModel = GameManager.arManager.targetSceneModel;

        //算出目标位置
        Vector3 postionOffset = points[2].position - points[0].position;
        Vector3 targetPostion = postionOffset + targetModel.position;

        //移动前将模型点位同时带走
        points[0].transform.SetParent(targetModel);
        points[1].transform.SetParent(targetModel);

        while (Vector3.Distance(targetModel.position, targetPostion) > 1f)
        {
            targetModel.position = Vector3.Lerp(targetModel.position, targetPostion, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        targetModel.position = targetPostion;
        viewWin.infoText.text = "坐标定位成功，正在修正旋转..";


        ////释放模型点位
        //points[0].transform.SetParent(null);
        //points[1].transform.SetParent(null);



        //将用于旋转的的点位y值。设定成，目标位置点位同一y值
        points[1].position = new Vector3(points[1].position.x, points[0].position.y, points[1].position.z);
        points[3].position = new Vector3(points[3].position.x, points[0].position.y, points[3].position.z);
        //让模型位置点位，看向模型旋转点位，并设置其为模型的父物体
        points[0].LookAt(points[1].position);
        points[0].SetParent(null);
        targetModel.SetParent(points[0]);

        //算出让模型点位看向现实旋转点位的旋转度，这样此就可以带动模型修正成正确的旋转
        Quaternion rotation = Quaternion.LookRotation(points[3].position - points[0].position);
        while (Vector3.Distance(points[0].eulerAngles, rotation.eulerAngles) > 1f)
        {
            points[0].rotation = Quaternion.Lerp(points[0].rotation, rotation, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        points[0].rotation = rotation;
        //旋转完成，移除模型父物体，清空点位
        targetModel.SetParent(null);
        viewWin.CloseUi();
        maskWin.CloseUi();
        GameManager.uiManager.ShowUiWindow<Tip_Window>("定位完毕");

        yield return new WaitForSeconds(2);
        ClearAllPoint();
        pageFather.gameObject.SetActive(true);

        returnButon.onClick.Invoke();
    }


    void ClearAllPoint()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
            {
                GameObject item = points[i].gameObject;
                item.transform.SetParent(null);
                GameObjectPoolTool.PutInPool(item);
                points[i] = null;

            }
        }
        targetPointIndex = -1;
    }

}
