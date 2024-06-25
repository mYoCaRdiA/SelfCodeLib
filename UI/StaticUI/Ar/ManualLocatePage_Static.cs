using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManualLocatePage_Static : StaticUi
{

    public PrefabInfo rotateBallModel;
    private Transform rotateBall;


    public Button[] speedButtons;

    public QiJoystick joystick;
    public QiJoystick joystick2;

    public Text speedShow;
    public float speed;
    public float speedBeiLv = 1;


    public Button upButton;
    public Button downButton;
    public Button centreButton;
    public Button returnButton;

    public Slider speedRate;

    Transform arCameraTrans;
    RaiseType raiseType = RaiseType.None;


    public RawImage fangdaImage; // 用于显示放大内容的Image组件
    public RectTransform fangdaMask;
    public float fangdaRate = 2f; // 放大倍数
    public Vector2 fangDaOffset = Vector2.zero;
    public bool firstClickOnScene = false;
    Texture2D cameraTexture;

    public Vector3 lastMouseDownPos;

    enum RaiseType
    {
        Up,
        Down,
        None
    }

    public void ChangeSpeedBeiLv(int beiLv)
    {
        speedBeiLv = beiLv;
    }

    private void OnEnable()
    {
        raiseType = RaiseType.None;
        if (rotateBall != null)
        {
            rotateBall.gameObject.SetActive(true);

        }
    }

    private void OnDisable()
    {
        if (rotateBall != null)
        {
            rotateBall.gameObject.SetActive(false);
        }
    }

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
        arCameraTrans = GameManager.arManager.arSession.GetComponentInChildren<Camera>().transform;
        EventTriggerTool.SetTriggerEvent(upButton.image, () =>
        {
            raiseType = RaiseType.Up;
        }, EventTriggerType.PointerDown);
        EventTriggerTool.SetTriggerEvent(upButton.image, () =>
        {
            if (raiseType == RaiseType.Up)
            {
                raiseType = RaiseType.None;
            }
        }, EventTriggerType.PointerUp);

        EventTriggerTool.SetTriggerEvent(downButton.image, () =>
        {
            raiseType = RaiseType.Down;
        }, EventTriggerType.PointerDown);
        EventTriggerTool.SetTriggerEvent(downButton.image, () =>
        {
            if (raiseType == RaiseType.Down)
            {
                raiseType = RaiseType.None;
            }
        }, EventTriggerType.PointerUp);

        centreButton.onClick.AddListener(() =>
        {
            GameManager.arManager.SetSceneModelToCentre();
        });

        returnButton.onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageTo(1);
        });



        for (int i = 0; i < speedButtons.Length; i++)
        {
            Button temp = speedButtons[i];
            temp.onClick.AddListener(() =>
            {
                foreach (var item in speedButtons)
                {
                    item.image.color = Color.white;
                }
                temp.image.color = Color.green;
            });
        }

        speedButtons[0].onClick.Invoke();




        cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fangdaImage.texture = cameraTexture;
        fangdaImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        fangdaImage.transform.localScale *= fangdaRate;



    }



    void Raise()
    {
        if (raiseType != RaiseType.None)
        {
            if (raiseType == RaiseType.Up)
            {
                Vector3 targetMoveOffset = Vector3.up * Time.deltaTime * speed * speedRate.value * speedBeiLv;
                GameManager.arManager.targetSceneModel.position += targetMoveOffset;
            }
            else
            {
                Vector3 targetMoveOffset = Vector3.down * Time.deltaTime * speed * speedRate.value * speedBeiLv;
                GameManager.arManager.targetSceneModel.position += targetMoveOffset;
            }
        }
    }

    private void Update()
    {
        Move();
        Raise();
        Rotate();
        ReflashSpeedRate();
        SetRotatePoint();
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

            fangdaMask.transform.position += (Vector3)fangDaOffset;
            fangdaMask.gameObject.SetActive(true);
        }
        else
        {
            fangdaMask.gameObject.SetActive(false);
            firstClickOnScene = false;
        }
    }

    private IEnumerator RenderScreenToTextureNextFrame()
    {
        // 等待下一帧
        yield return new WaitForEndOfFrame();

        // 读取屏幕上的像素数据到Texture2D中
        cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        cameraTexture.Apply();
    }


    void SetRotatePoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMouseDownPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Input.mousePosition;
            if (lastMouseDownPos == mousePos)
            {
                if (!GameManager.uiManager.IsOnUIElement(mousePos))
                {
                    MyArHitInfo hitInfo = ArEngine.ScreenRaycastGenerateModel(mousePos);
                    if (hitInfo != null)
                    {
                        Vector3 position = hitInfo.pointPostion;
                        if (rotateBall == null)
                        {
                            rotateBall = GameObjectPoolTool.GetFromPoolForce(false, rotateBallModel.path).transform;
                            rotateBall.position = GameManager.arManager.targetSceneModel.position;
                            rotateBall.rotation = Quaternion.identity;
                            rotateBall.parent = GameManager.arManager.targetSceneModel;
                            rotateBall.gameObject.SetActive(true);
                        }
                        rotateBall.position = position;
                        //GameManager.uiManager.ShowUiWindow<ViewTip_Window>("设定旋转中心为:" + position );
                    }
                }
            }
        }
    }


    void ReflashSpeedRate()
    {
        speedShow.text = "当前移动速率:" + (speedRate.value * 100).ToString("0.00") + "%";
    }

    private void Move()
    {
        Vector3 cameraRight = arCameraTrans.right;
        cameraRight = new Vector3(cameraRight.x, 0, cameraRight.z);
        cameraRight = cameraRight.normalized;

        Vector3 cameraForward = arCameraTrans.forward;
        cameraForward = new Vector3(cameraForward.x, 0, cameraForward.z);
        cameraForward = cameraForward.normalized;

        Vector3 targetMoveOffset = (cameraRight * joystick.value.x + cameraForward * joystick.value.y);
        targetMoveOffset = targetMoveOffset * Time.deltaTime * speed * speedRate.value * speedBeiLv;

        GameManager.arManager.targetSceneModel.position += targetMoveOffset;
    }

    private void Rotate()
    {
        Vector3 targetRotateOffset = Vector3.up * joystick2.value.x * Time.deltaTime * speed * speedRate.value * speedBeiLv * 10;
        if (rotateBall != null)
        {
            GameManager.arManager.targetSceneModel.RotateAround(new Vector3(rotateBall.position.x, GameManager.arManager.targetSceneModel.position.y, rotateBall.position.z), rotateBall.up, targetRotateOffset.y);
        }
        else
        {
            GameManager.arManager.targetSceneModel.eulerAngles += targetRotateOffset;
        }
    }
}
