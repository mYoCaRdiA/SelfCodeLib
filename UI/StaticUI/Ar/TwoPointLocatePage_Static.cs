using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class TwoPointLocatePage_Static : StaticUi
{
    public RawImage fangdaImage; // ������ʾ�Ŵ����ݵ�Image���
    public RectTransform fangdaMask;
    public float fangdaRate = 2f; // �Ŵ���
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
        pointInfo.Add("ģ�͵�λ1");
        pointInfo.Add("ģ�͵�λ2");
        pointInfo.Add("��ʵ��λ1");
        pointInfo.Add("��ʵ��λ2");


        for (int i = 0; i < pointSetButtons.Count; i++)
        {
            Button temp = pointSetButtons[i];
            temp.GetComponentInChildren<Text>().text = pointInfo[i];
            //�������ť��ֵ����
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
        // �ȴ���һ֡
        yield return new WaitForEndOfFrame();

        // ��ȡ��Ļ�ϵ��������ݵ�Texture2D��
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
                tipText.text = pointInfo[targetPointIndex] + "�����ã����ɵ�������޸ĵ�λ�������·���ɫ��ť�л�Ŀ���λ����";
            }
            else
            {
                tipText.text = "�����������õ�λ��" + pointInfo[targetPointIndex];
            }
        }
        else
        {
            tipText.text = "�����·���ť��ѡ��Ҫ���õĵ�λ��";
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
                GameManager.uiManager.ShowUiWindow<Tip_Window>(pointInfo[i] + "δ���ã������ú�����");
                return;
            }
        }

        if (Vector3.Distance(points[0].position, points[1].position) <= 0.2f)
        {
            GameManager.uiManager.ShowUiWindow<Tip_Window>(pointInfo[0] + "��" + pointInfo[1] + "�ľ���С��0.2�ף��ᵼ�¾���̫�ͣ������µ�����������λ");
            return;
        }

        if (Vector3.Distance(points[0].position, points[1].position) <= 0.2f)
        {
            GameManager.uiManager.ShowUiWindow<Tip_Window>(pointInfo[2] + "��" + pointInfo[3] + "�ľ���С��0.2�ף��ᵼ�¾���̫�ͣ������µ�����������λ");
            return;
        }

        //��ʼ��λģ��
        StartCoroutine(LocateRoutine());
    }


    IEnumerator LocateRoutine()
    {
        GameManager.arManager.SwitchShowSceneModelEntity(true);
        GameManager.arManager.SetTrackingMeshType(TrackingMeshType.Normal);
        pageFather.gameObject.SetActive(false);

        MaskOnly_Window maskWin = GameManager.uiManager.ShowUiWindow<MaskOnly_Window>();
        ViewTip_Window viewWin = GameManager.uiManager.ShowUiWindow<ViewTip_Window>("���Ժ�ģ�����ڶ�λ..");

        Transform targetModel = GameManager.arManager.targetSceneModel;

        //���Ŀ��λ��
        Vector3 postionOffset = points[2].position - points[0].position;
        Vector3 targetPostion = postionOffset + targetModel.position;

        //�ƶ�ǰ��ģ�͵�λͬʱ����
        points[0].transform.SetParent(targetModel);
        points[1].transform.SetParent(targetModel);

        while (Vector3.Distance(targetModel.position, targetPostion) > 1f)
        {
            targetModel.position = Vector3.Lerp(targetModel.position, targetPostion, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        targetModel.position = targetPostion;
        viewWin.infoText.text = "���궨λ�ɹ�������������ת..";


        ////�ͷ�ģ�͵�λ
        //points[0].transform.SetParent(null);
        //points[1].transform.SetParent(null);



        //��������ת�ĵĵ�λyֵ���趨�ɣ�Ŀ��λ�õ�λͬһyֵ
        points[1].position = new Vector3(points[1].position.x, points[0].position.y, points[1].position.z);
        points[3].position = new Vector3(points[3].position.x, points[0].position.y, points[3].position.z);
        //��ģ��λ�õ�λ������ģ����ת��λ����������Ϊģ�͵ĸ�����
        points[0].LookAt(points[1].position);
        points[0].SetParent(null);
        targetModel.SetParent(points[0]);

        //�����ģ�͵�λ������ʵ��ת��λ����ת�ȣ������˾Ϳ��Դ���ģ����������ȷ����ת
        Quaternion rotation = Quaternion.LookRotation(points[3].position - points[0].position);
        while (Vector3.Distance(points[0].eulerAngles, rotation.eulerAngles) > 1f)
        {
            points[0].rotation = Quaternion.Lerp(points[0].rotation, rotation, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        points[0].rotation = rotation;
        //��ת��ɣ��Ƴ�ģ�͸����壬��յ�λ
        targetModel.SetParent(null);
        viewWin.CloseUi();
        maskWin.CloseUi();
        GameManager.uiManager.ShowUiWindow<Tip_Window>("��λ���");

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
