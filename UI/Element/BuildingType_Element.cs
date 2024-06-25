using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingType_Element : UiElement
{
    public string ButtonString
    {
        get { return text.text; }
    
    }
    Button button;
    public Text text;

    public Action onAim;
    public Action onDisAim;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>(true);
        button.onClick.AddListener(() =>
        {
            if (text.color == Color.blue)
            {
                text.color = Color.black;
                onDisAim?.Invoke();
            }
            else
            {
                text.color = Color.blue;
                onAim?.Invoke();
            }
        });

    }

    private void OnEnable()
    {
        //RestoreDefaultColor();
    }

    public void RestoreDefaultColor()
    {
        text.color = Color.black;
    }

    public void SetTextStr(string str)
    {
        text.text = str;
    }

}
