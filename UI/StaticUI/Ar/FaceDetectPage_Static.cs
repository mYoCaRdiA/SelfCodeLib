using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FaceDetectPage_Static : StaticUi
{
    public bool ifReflashToken = false;
    Detector yoloDetetor;
    public Button returnButton;
    // Start is called before the first frame update

    public void Ini()
    {
        returnButton.onClick.AddListener(() =>
        {
            UiManager.NowBook.ChangePageTo(1);
        });
        yoloDetetor = transform.GetComponentInChildren<Detector>();
        yoloDetetor.Ini();
        transform.SetAsFirstSibling();
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
        GameManager.arManager.SetTrackingMeshType(TrackingMeshType.InVisiable);
        GameManager.arManager.targetSceneModel.gameObject.SetActive(false);
        GameManager.arManager.arPublicPanel_Static.LockArMeshDropdown(true);
    }

    private void OnDisable()
    {
        GameManager.arManager.targetSceneModel.gameObject.SetActive(true);
        GameManager.arManager.arPublicPanel_Static.LockArMeshDropdown(false);
    }

   


    [ContextMenu("≤‚ ‘–≠≥Ã")]
    void Test()
    {
        StartCoroutine(TestWorkInfoInfo());
    }

    IEnumerator TestWorkInfoInfo()
    {
        WorkerData workerData = new WorkerData();
        IEnumerator workInfoRoutine = HttpTool.GetRequest<RequestClass>(HttpTool.projectToken, UrlTail.workerInfo, (xxx) =>
          {
              workerData = JsonConvert.DeserializeObject<WorkerData>(xxx.data.ToString());
          }, new KeyValuePair<string, string>("id", "360721199308154018"));
        yield return workInfoRoutine;
        GameManager.uiManager.ShowUiWindow<WorkerInfo_Window>(workerData);
    }

}
