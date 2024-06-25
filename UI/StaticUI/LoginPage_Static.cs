using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class LoginPage_Static : StaticUi
{
    [Header("账号密码输入模块")]
    public InputField AccountInput;
    public InputField PasswordInput;
    public Button loginButton;
    public Toggle saveAccountToggle;


    void Start()
    {
        ReloadNumAndPassword();
        loginButton.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            GameManager.instance.StartCoroutine(Login());
        });
    }


    // Update is called once per frame
    IEnumerator Login()
    {
        UiWindow uiWin = GameManager.uiManager.ShowUiWindow<MaskOnly_Window>();

        if (saveAccountToggle.isOn)
        {
            SaveNumAndPassword();
        }
        else
        {
            ClearNumAndPassword();
        }

        string AccountNumber = AccountInput.text;
        string PassWordNumber = PasswordInput.text;

        WWWForm form = new WWWForm();
        form.AddField("account", AccountNumber);
        form.AddField("password", HashTool.GenerateMD5(PassWordNumber));

        RequestClass requestEntity = null;
        IEnumerator post = HttpTool.PostRequest<RequestClass>(HttpTool.loginToken, UrlTail.login, (result) => { requestEntity = result; }, form);
        yield return StartCoroutine(post);

        if (requestEntity != null)
        {
            if (requestEntity.code == 200)
            {
                GameManager.dataManager.loginRequestClass = requestEntity;
                TokenData loginData = JsonConvert.DeserializeObject<TokenData>(requestEntity.data.ToString());
                HttpTool.loginToken = loginData.token;


                if (GameManager.dataManager.nowUserName == "" || GameManager.dataManager.nowUserName != AccountInput.text)
                {

                    RequestClass projectItemRequestData = null;
                    IEnumerator get = HttpTool.GetRequest<RequestClass>(HttpTool.loginToken, UrlTail.projectGantt, (result) => { projectItemRequestData = result; });
                    yield return StartCoroutine(get);
                    if (projectItemRequestData != null)
                    {
                        ProjectItemData projectItemData = JsonConvert.DeserializeObject<ProjectItemData>(projectItemRequestData.data.ToString());
                        Debug.Log("项目数据加载成功，数量为:" + projectItemData.list.Count);
                        GameManager.dataManager.nowProjectItemDatas = projectItemData.list;

                        RequestClass userInfoRequestData = null;
                        IEnumerator getUserInfo = HttpTool.GetRequest<RequestClass>(HttpTool.loginToken, UrlTail.currentUser, (result) => { userInfoRequestData = result; });
                        yield return StartCoroutine(getUserInfo);
                        if (userInfoRequestData == null)
                        {
                            GameManager.dataManager.nowUserName = "";
                            GameManager.uiManager.ShowUiWindow<Tip_Window>("已登录,但获取用户信息失败，请重试");
                        }
                        else
                        {
                            GameManager.dataManager.nowUserInfoData = JsonConvert.DeserializeObject<UserData>(userInfoRequestData.data.ToString());
                            UiManager.NowBook.ChangePageTo(2);
                            GameManager.dataManager.nowUserName = AccountInput.text;
                        }
                    }
                    else
                    {
                        GameManager.uiManager.ShowUiWindow<Tip_Window>("获取项目表数据失败,请重试");
                    }
                }
                else
                {
                    UiManager.NowBook.ChangePageTo(2);
                }
            }
            else
            {
                GameManager.uiManager.ShowUiWindow<Tip_Window>(requestEntity.msg);
            }
        }
        else
        {
            GameManager.uiManager.ShowUiWindow<Tip_Window>("网络无法接通，请重试");
        }
       
        uiWin.CloseUi();
    }


    /// <summary>
    /// 保存账号密码
    /// </summary>
    private void SaveNumAndPassword()
    {
        PlayerPrefs.SetString(nameof(AccountInput), AccountInput.text);
        PlayerPrefs.SetString(nameof(PasswordInput), PasswordInput.text);
        PlayerPrefs.SetInt(nameof(saveAccountToggle), saveAccountToggle.isOn?1:0);
    }

    private void ClearNumAndPassword()
    {
        PlayerPrefs.SetString(nameof(AccountInput), "");
        PlayerPrefs.SetString(nameof(PasswordInput), "");
        PlayerPrefs.SetInt(nameof(saveAccountToggle), 0);
    }


    /// <summary>
    /// 填充记录的账号密码
    /// </summary>
    private void ReloadNumAndPassword()
    {
        AccountInput.text = PlayerPrefs.GetString(nameof(AccountInput));
        PasswordInput.text = PlayerPrefs.GetString(nameof(PasswordInput));
        saveAccountToggle.isOn= PlayerPrefs.GetInt(nameof(saveAccountToggle))==0?false:true;
    }


}
