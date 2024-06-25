using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Platform_Window : UiWindow
{
    public string version = "1.0";
    public Text versionText;
    public Button closeButton;

    protected override void Awake()
    {
        base.Awake();
        closeButton.onClick.AddListener(() =>
        {
            CloseUi();
        });
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        versionText.text = version;
    }
}
