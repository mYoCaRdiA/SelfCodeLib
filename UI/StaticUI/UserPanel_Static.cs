
using UnityEngine;
using UnityEngine.UI;

public class UserPanel_Static : StaticUi
{
    public Text nameText;
    public Button listButton;
    public GameObject listParent;
    public Button userInfo;
    public Button aboutPlatform;
    public Button exitLogin;
    public Button backGroundButton;

    private void Awake()
    {
       
        userInfo.onClick.AddListener(() =>
        {
            listParent.SetActive(false);
            GameManager.uiManager.ShowUiWindow<UserInfo_Window>();
        });

        aboutPlatform.onClick.AddListener(() =>
        {
            listParent.SetActive(false);
            GameManager.uiManager.ShowUiWindow<Platform_Window>();
        });

        listButton.onClick.AddListener(() =>
        {
            bool target = listParent.activeSelf ? false : true;
            Debug.Log(target);
            listParent.SetActive(target);
        });

        exitLogin.onClick.AddListener(() =>
        {
            listParent.SetActive(false);
            UiManager.NowBook.ChangePageTo(1);
        });

        backGroundButton.onClick.AddListener(() =>
        {
            listParent.SetActive(false);
        });
    }
    
    private void OnEnable()
    {
        listParent.SetActive(false);
        nameText.text = GameManager.dataManager.nowUserInfoData.userInfo.userName;
    }
}
