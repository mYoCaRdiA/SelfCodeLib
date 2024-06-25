using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToShowSelectPage_Static : StaticUi
{
    public Button button3d;
    public Button buttonAr;
    public Button backButtton;

    private void Awake()
    {
        button3d.onClick.AddListener(() => {
            Debug.Log("打开3d模型:"+GameManager.dataManager.nowModelInfoItem.LoadPath.ModelPath);
        });
        buttonAr.onClick.AddListener(() => {
            
            SceneManager.LoadSceneAsync("NowVersion");
            
            this.gameObject.SetActive(false);
            //GameObject.Find("Canvas").transform.Find("MainPanel_Static").gameObject.SetActive(true);
            
        });
        backButtton.onClick.AddListener(() => {
            UiManager.NowBook.ChangePageTo(3);
        });
    }

}
