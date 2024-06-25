using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public List<GameObject> secondPanels = new List<GameObject>();
    public List<GameObject> thirdPanels = new List<GameObject>();
    public List<GameObject> anotherPanels = new List<GameObject>();
    public static PanelManager instance;

    private void Awake()
    {
        instance = this; 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    /// <summary>
    /// 打开二级面板
    /// </summary>
    public void OpenSecondPanel(string uiPanelName)
    {

        if (UIPanel.currentSecondPanel == null)
        {
            for (int i = 0; i < secondPanels.Count; i++)
            {
                if (secondPanels[i].name == uiPanelName)
                {
                    secondPanels[i].SetActive(true);
                    UIPanel.currentSecondPanel = secondPanels[i];
                    return;
                }
            }
        }
        else
        {
            if (UIPanel.currentSecondPanel.activeSelf == false)
            {

                for (int i = 0; i < secondPanels.Count; i++)
                {
                    if (secondPanels[i].name == uiPanelName)
                    {
                        secondPanels[i].SetActive(true);
                        UIPanel.currentSecondPanel = secondPanels[i];
                        return;
                    }
                }
            }
            else
            {
                UIPanel.currentSecondPanel.SetActive(false);
                for (int i = 0; i < secondPanels.Count; i++)
                {
                    if (secondPanels[i].name == uiPanelName)
                    {

                        if (secondPanels[i] == UIPanel.currentSecondPanel)
                        {
                            UIPanel.currentSecondPanel.SetActive(false);
                            return;
                        }
                        else
                        {
                            UIPanel.currentSecondPanel.SetActive(false);
                            secondPanels[i].SetActive(true);
                            UIPanel.currentSecondPanel = secondPanels[i];
                        }

                    }
                }
            }
        }


    }
    /// <summary>
    /// 打开二级面板
    /// </summary>
    public void OpenSecondPanel(string uiPanelName,Transform transform)
    {
        if(UIPanel.currentSecondPanel == null)
        {
            for (int i = 0; i < secondPanels.Count; i++)
            {
                if (secondPanels[i].name == uiPanelName)
                {
                    secondPanels[i].SetActive(true);
                    UIPanel.currentSecondPanel = secondPanels[i];
                    return;
                }
            }
        }
        else
        {
            if (UIPanel.currentSecondPanel.activeSelf == false)
            {

                for (int i = 0; i < secondPanels.Count; i++)
                {
                    if (secondPanels[i].name == uiPanelName)
                    {
                        secondPanels[i].SetActive(true);
                        UIPanel.currentSecondPanel = secondPanels[i];
                        return;
                    }
                }
            }
            else
            {
                UIPanel.currentSecondPanel.SetActive(false);
                for (int i = 0; i < secondPanels.Count; i++)
                {
                    if (secondPanels[i].name == uiPanelName)
                    {

                        if (secondPanels[i] == UIPanel.currentSecondPanel)
                        {
                            UIPanel.currentSecondPanel.SetActive(false);
                            return;
                        }
                        else
                        {
                            UIPanel.currentSecondPanel.SetActive(false);
                            secondPanels[i].SetActive(true);
                            UIPanel.currentSecondPanel = secondPanels[i];
                        }

                    }
                }
            }
        }
        
        


    }

    /// <summary>
    /// 打开三级面板
    /// </summary>
    public void OpenThirdPanel(string uiPanelName)
    {

        if (UIPanel.currentThirdPanel == null)
        {
            for (int i = 0; i < thirdPanels.Count; i++)
            {
                if (thirdPanels[i].name == uiPanelName)
                {
                    thirdPanels[i].SetActive(true);
                    UIPanel.currentThirdPanel = thirdPanels[i];
                    return;
                }
            }
        }
        else
        {
            if (UIPanel.currentThirdPanel.activeSelf == false)
            {

                for (int i = 0; i < thirdPanels.Count; i++)
                {
                    if (thirdPanels[i].name == uiPanelName)
                    {
                        thirdPanels[i].SetActive(true);
                        UIPanel.currentThirdPanel = thirdPanels[i];
                        return;
                    }
                }
            }
            else
            {
                UIPanel.currentThirdPanel.SetActive(false);
                for (int i = 0; i < thirdPanels.Count; i++)
                {
                    if (thirdPanels[i].name == uiPanelName)
                    {

                        if (thirdPanels[i] == UIPanel.currentThirdPanel)
                        {
                            UIPanel.currentThirdPanel.SetActive(false);
                            return;
                        }
                        else
                        {
                            UIPanel.currentThirdPanel.SetActive(false);
                            thirdPanels[i].SetActive(true);
                            UIPanel.currentThirdPanel = thirdPanels[i];
                        }

                    }
                }
            }
        }


    }
    /// <summary>
    /// 打开三级面板
    /// </summary>
    public void OpenThirdPanel(string uiPanelName,Transform transform)
    {

        if (UIPanel.currentThirdPanel == null)
        {
            for (int i = 0; i < thirdPanels.Count; i++)
            {
                if (thirdPanels[i].name == uiPanelName)
                {
                    thirdPanels[i].SetActive(true);
                    UIPanel.currentThirdPanel = thirdPanels[i];
                    return;
                }
            }
        }
        else
        {
            if (UIPanel.currentThirdPanel.activeSelf == false)
            {

                for (int i = 0; i < thirdPanels.Count; i++)
                {
                    if (thirdPanels[i].name == uiPanelName)
                    {
                        thirdPanels[i].SetActive(true);
                        UIPanel.currentThirdPanel = thirdPanels[i];
                        return;
                    }
                }
            }
            else
            {
                UIPanel.currentThirdPanel.SetActive(false);
                for (int i = 0; i < thirdPanels.Count; i++)
                {
                    if (thirdPanels[i].name == uiPanelName)
                    {

                        if (thirdPanels[i] == UIPanel.currentThirdPanel)
                        {
                            UIPanel.currentThirdPanel.SetActive(false);
                            return;
                        }
                        else
                        {
                            UIPanel.currentThirdPanel.SetActive(false);
                            thirdPanels[i].SetActive(true);
                            UIPanel.currentThirdPanel = thirdPanels[i];
                        }

                    }
                }
            }
        }


    }
    /// <summary>
    /// 打开其他面板
    /// </summary>
    public void OpenAnotherPanel(string uiPanelName)
    {

        if (UIPanel.currentAnotherPanel == null)
        {
            for (int i = 0; i < anotherPanels.Count; i++)
            {
                if (anotherPanels[i].name == uiPanelName)
                {
                    anotherPanels[i].SetActive(true);
                    UIPanel.currentAnotherPanel = anotherPanels[i];
                    return;
                }
            }
        }
        else
        {
            if (UIPanel.currentAnotherPanel.activeSelf == false)
            {

                for (int i = 0; i < anotherPanels.Count; i++)
                {
                    if (anotherPanels[i].name == uiPanelName)
                    {
                        anotherPanels[i].SetActive(true);
                        UIPanel.currentAnotherPanel = anotherPanels[i];
                        return;
                    }
                }
            }
            else
            {
                UIPanel.currentAnotherPanel.SetActive(false);
                for (int i = 0; i < anotherPanels.Count; i++)
                {
                    if (anotherPanels[i].name == uiPanelName)
                    {

                        if (anotherPanels[i] == UIPanel.currentAnotherPanel)
                        {
                            UIPanel.currentAnotherPanel.SetActive(false);
                            return;
                        }
                        else
                        {
                            UIPanel.currentAnotherPanel.SetActive(false);
                            anotherPanels[i].SetActive(true);
                            UIPanel.currentAnotherPanel = anotherPanels[i];
                        }

                    }
                }
            }
        }


    }
    /// <summary>
    /// 打开其他面板
    /// </summary>
    public void OpenAnotherPanel(string uiPanelName,Transform transform)
    {

        if (UIPanel.currentAnotherPanel == null)
        {
            for (int i = 0; i < anotherPanels.Count; i++)
            {
                if (anotherPanels[i].name == uiPanelName)
                {
                    anotherPanels[i].SetActive(true);
                    UIPanel.currentAnotherPanel = anotherPanels[i];
                    return;
                }
            }
        }
        else
        {
            if (UIPanel.currentAnotherPanel.activeSelf == false)
            {

                for (int i = 0; i < anotherPanels.Count; i++)
                {
                    if (anotherPanels[i].name == uiPanelName)
                    {
                        anotherPanels[i].SetActive(true);
                        UIPanel.currentAnotherPanel = anotherPanels[i];
                        return;
                    }
                }
            }
            else
            {
                UIPanel.currentAnotherPanel.SetActive(false);
                for (int i = 0; i < anotherPanels.Count; i++)
                {
                    if (anotherPanels[i].name == uiPanelName)
                    {

                        if (anotherPanels[i] == UIPanel.currentAnotherPanel)
                        {
                            UIPanel.currentAnotherPanel.SetActive(false);
                            return;
                        }
                        else
                        {
                            UIPanel.currentAnotherPanel.SetActive(false);
                            anotherPanels[i].SetActive(true);
                            UIPanel.currentAnotherPanel = anotherPanels[i];
                        }

                    }
                }
            }
        }


    }
    /// <summary>
    /// 关闭二级面板
    /// </summary>
    public void CloseSecondPanel()
    {
        UIPanel.currentSecondPanel.SetActive(false);
    }
    /// <summary>
    /// 关闭三级面板
    /// </summary>
    public void CloseThirdPanel()
    {
        UIPanel.currentThirdPanel.SetActive(false);
    }
}
public static class UIPanel
{
    public static GameObject currentSecondPanel;
    public static GameObject currentThirdPanel;
    public static GameObject currentAnotherPanel;
}
