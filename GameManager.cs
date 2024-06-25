using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Manager
{

    public static GameManager instance;



    //其他manager
    public static DataManager dataManager;
    public static UiManager uiManager;
    public static ArManager arManager;
    public static ArManagerClone arManagerClone;


    public override void Ini()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            //Manager[] manager = transform.GetComponents<Manager>();
            //foreach (var item in manager)
            //{
            //    item.enabled = true;
            //}
            //切换场景前释放对象池
            SceneManager.sceneUnloaded += (scene) =>
            {
                GameObjectPoolTool.Release();
            };
            instance = this;
        }
    }

}


public abstract class Manager : MonoBehaviour
{
    void Awake()
    {
        //Debug.Log("dasdas");
        Ini();
    }
    private void OnDisable()
    {

    }
    public abstract void Ini();
}



