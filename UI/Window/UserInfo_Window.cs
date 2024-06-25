using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo_Window : UiWindow
{
    [Header("标签按钮和Book")]
    public Book book;
    public List<Button> pageButtons = new List<Button>();

    [Header("个人资料")]
    public InputField accountInput;
    public InputField organisationText;
    public InputField supervisorText;
    public InputField workPositionText;
    public InputField roleText;
    public InputField regdateDateText;
    public InputField lastLoginDateText;
    public InputField startJobDateText;
    void SetData()
    {
        // 将所有 InputField 的文本值设置为空
        accountInput.text = GameManager.dataManager.nowUserInfoData.userInfo.userAccount;
        organisationText.text = GameManager.dataManager.nowUserInfoData.userInfo.organizeName;
        supervisorText.text = "";
        workPositionText.text = "";
        roleText.text = GameManager.dataManager.nowUserInfoData.userInfo.userName;
        regdateDateText.text = "";
        lastLoginDateText.text = TimeTool.TimeStamp2Str(GameManager.dataManager.nowUserInfoData.userInfo.prevLoginTime);
        startJobDateText.text = "";
    }

    [Header("密码修改")]
    public InputField passwordText;
    public InputField confirmPsdText;
    public Button btnTrue;

    [Header("关闭按钮")]
    public Button closeButton;

    protected override void Awake()
    {
        base.Awake();
        foreach (var item in pageButtons)
        {
            item.onClick.AddListener(() =>
            {
                foreach (var but in pageButtons)
                {
                    if (but != item)
                    {
                        but.GetComponentInChildren<Text>().color = Color.black;
                    }
                    else
                    {
                        but.GetComponentInChildren<Text>().color = Color.blue;
                    }
                }
                book.ChangePageTo(pageButtons.IndexOf(item) + 1);
            });
        }

        closeButton.onClick.AddListener(() =>
        {
            CloseUi();
        });
    }




    protected override void OnEnable()
    {
        base.OnEnable();
        pageButtons[0].onClick.Invoke();
        SetData();
    }
}
