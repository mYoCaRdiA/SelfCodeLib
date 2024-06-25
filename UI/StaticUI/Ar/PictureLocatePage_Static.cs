using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PictureLocatePage_Static : StaticUi
{
    public Button returnButton;
    ARTrackedImageManager trackedImageManager;
    ProjectLocateData projectLocateData;
    List<ARTrackedImage> allARTrackedImages = new List<ARTrackedImage>();


    public void Ini()
    {

        trackedImageManager = GameManager.arManager.arSession.GetComponent<ARTrackedImageManager>();
        returnButton.onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageTo(1);
        });


        //if (GameManager.dataManager.nowProjectId == "")
        //{
        //    GameManager.uiManager.ShowUiWindow<Tip_Window>("初始化图片定位失败，项目id为空");
        //    Destroy(this);
        //}
        //else
        //{
        if (projectLocateData == null)
        {
            string jsonData = PlayerPrefs.GetString(GameManager.dataManager.nowProjectId);
            if (jsonData == "")
            {
                projectLocateData = new ProjectLocateData(GameManager.dataManager.nowProjectId);
            }
            else
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new Vector3Converter());
                jsonSerializerSettings.Converters.Add(new QuaternionConverter());
                projectLocateData = JsonConvert.DeserializeObject<ProjectLocateData>(jsonData, jsonSerializerSettings);
            }
        }
        //}
        trackedImageManager.enabled = false;
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "NowVersion")
        {
            returnButton.gameObject.SetActive(false);
        }
        else
        {
            returnButton.gameObject.SetActive(true);

        }
        trackedImageManager.enabled = true;

        GameManager.arManager.SetTrackingMeshType(TrackingMeshType.InVisiable);
        //GameManager.arManager.SetAllModelAlpha(0f);
        GameManager.arManager.targetSceneModel.gameObject.SetActive(false);
        GameManager.arManager.arPublicPanel_Static.LockArMeshDropdown(true);

    }

    private void OnDisable()
    {
        trackedImageManager.enabled = false;
        GameManager.arManager.targetSceneModel.gameObject.SetActive(true);
        GameManager.arManager.arPublicPanel_Static.LockArMeshDropdown(false);
        foreach (var item in allARTrackedImages)
        {
            item.gameObject.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateInfo(trackedImage);
        }


        void UpdateInfo(ARTrackedImage trackedImage)
        {

            trackedImage.gameObject.SetActive(true);
            if (!allARTrackedImages.Contains(trackedImage))
            {
                allARTrackedImages.Add(trackedImage);
            }
            ArImageUi_Element arImageUi_Element = trackedImage.GetComponent<ArImageUi_Element>();
            //Vector2 size = trackedImage.size * 100f;
            // trackedImage.referenceImage.name
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);
                trackedImage.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                trackedImage.transform.GetChild(0).gameObject.SetActive(false);
            }



            Vector2 size = new Vector2(arImageUi_Element.GetRealWidth(), arImageUi_Element.GetRealHeight());
            arImageUi_Element.infoText.text = "图片名称:" + trackedImage.referenceImage.name + "\n" + "请务必在黑块完全对齐后，点击下方按钮，以定位或上传定位数据";


            string pictureName = trackedImage.referenceImage.name;


            //算出当前模型相对ar图的坐标和旋转四元素
            Vector3 modelLocalPosition = trackedImage.transform.InverseTransformPoint(GameManager.arManager.targetSceneModel.transform.position);
            Quaternion modelLocalRotation = Quaternion.Inverse(trackedImage.transform.rotation) * GameManager.arManager.targetSceneModel.transform.rotation;
            LocateData locateData = new LocateData(modelLocalPosition, modelLocalRotation);


            Action locate = new Action(() =>
            {
                if (projectLocateData.allData.ContainsKey(pictureName))
                {
                    LocateData locateData = projectLocateData.allData[pictureName];
                    GameManager.arManager.targetSceneModel.transform.position = trackedImage.transform.TransformPoint(locateData.positionFromPicture);
                    GameManager.arManager.targetSceneModel.transform.rotation = trackedImage.transform.rotation * locateData.rotationFromPicture;
                    GameManager.uiManager.ShowUiWindow<Tip_Window>("定位成功");
                    if (SceneManager.GetActiveScene().name == "NowVersion")
                    {
                        return;
                    }
                    else
                    {
                        UiManager.NowBook.ChangePageTo(1);

                    }

                }
                else
                {
                    GameManager.uiManager.ShowUiWindow<Tip_Window>("无法同步定位，尚未更新此图定位数据");
                }
                // GameManager.uiManager.ShowUiWindow<Tip_Window>("111111111111");

            });


            Action update = new Action(() =>
            {
                ConfirmTip_Window.IniData iniData = new ConfirmTip_Window.IniData("是否确定更新定位图（" + pictureName + "）的数据？", () =>
                {
                    projectLocateData.UpdateData(pictureName, locateData);
                    Debug.Log("更新完毕：" + pictureName);
                });
                GameManager.uiManager.ShowUiWindow<ConfirmTip_Window>(iniData);
            });


            Action clear = new Action(() =>
            {
                ConfirmTip_Window.IniData iniData = new ConfirmTip_Window.IniData("是否确定清空定位图（" + pictureName + "）的数据？", () =>
                {
                    projectLocateData.ClearData(pictureName);
                    Debug.Log("清空完毕：" + pictureName);
                });
                GameManager.uiManager.ShowUiWindow<ConfirmTip_Window>(iniData);
            });


            arImageUi_Element.SetButtonEvents(locate, update, clear);



        }
    }



    public class ProjectLocateData
    {
        public string projectId = "";

        public Dictionary<string, LocateData> allData = new Dictionary<string, LocateData>();

        JsonSerializerSettings jsonSerializerSettings;

        public ProjectLocateData(string projectId)
        {
            this.projectId = projectId;
            jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Vector3Converter());
            jsonSerializerSettings.Converters.Add(new QuaternionConverter());
            UpdatePlayerPrefsData();
        }

        public void UpdateData(string keyNum, LocateData data)
        {
            int index;
            if (!int.TryParse(keyNum, out index))
            {
                Debug.LogError("key值不正确，不是个数字");
                return;
            }

            if (allData.ContainsKey(keyNum))
            {
                allData[keyNum] = data;
            }
            else
            {
                allData.Add(keyNum, data);
            }

            UpdatePlayerPrefsData();
        }

        public void ClearData(string keyNum)
        {
            int index;
            if (!int.TryParse(keyNum, out index))
            {
                Debug.LogError("key值不正确，不是个数字");
                return;
            }

            if (allData.ContainsKey(keyNum))
            {
                allData.Remove(keyNum);
            }

            UpdatePlayerPrefsData();
        }

        void UpdatePlayerPrefsData()
        {
            if (projectId != "")
            {
                PlayerPrefs.SetString(projectId, JsonConvert.SerializeObject(this, jsonSerializerSettings)); ;
            }
        }
    }

    public class LocateData
    {
        public Vector3 positionFromPicture;
        public Quaternion rotationFromPicture;
        public LocateData(Vector3 positionFromPicture, Quaternion rotationFromPicture)
        {
            this.positionFromPicture = positionFromPicture;
            this.rotationFromPicture = rotationFromPicture;
        }
    }
}


