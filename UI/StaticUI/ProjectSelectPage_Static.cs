using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectItemData;
//using static UnityEditor.Progress;

public class ProjectSelectPage_Static : StaticUi
{
    UserData lastUserInfoData;
    private List<ProjectItem_Element> nowProjectItemEntities = new List<ProjectItem_Element>();
    [Header("��Ŀ��ť����")]
    public Transform projectItemContainer;

    private void OnEnable()
    {
        if (lastUserInfoData != GameManager.dataManager.nowUserInfoData)
        {
            GenerateProjectInfo2ProjectButton();
            lastUserInfoData = GameManager.dataManager.nowUserInfoData;
        }
    }

    void GenerateProjectInfo2ProjectButton()
    {
        foreach (var item in nowProjectItemEntities)
        {
            item.CloseUi();
        }
        nowProjectItemEntities.Clear();
        foreach (var item in GameManager.dataManager.nowProjectItemDatas)
        {
            ProjectItem_Element element = GameManager.uiManager.GetUiElement<ProjectItem_Element>();
            element.SetAllTextValues(item.fullName, item.projectTypeName, item.amount, TimeTool.TimeStamp2Str(item.startTime), item.monomerNum, item.stageName);
            element.transform.SetParent(projectItemContainer);
            element.transform.localScale = Vector3.one;
            nowProjectItemEntities.Add(element);
            element.onButtonClick = new Action(() =>
            {
                StartCoroutine(OpenProject(item));
            });
        }


        /// <summary>
        /// �����Ŀ��ť���򿪶�Ӧ��Ŀ�ĵ����ģ��ѡ��
        /// </summary>
        /// <param name="projectselection"></param>
        IEnumerator OpenProject(Datum projectSelection)
        {
            MaskOnly_Window uiWin = GameManager.uiManager.ShowUiWindow<MaskOnly_Window>();
            HttpTool.projectToken = "";
            GameManager.dataManager.nowProjectItemData = projectSelection;
            RequestClass targetData = null;
            IEnumerator requestModelSelectInfo = HttpTool.GetRequest<RequestClass>(HttpTool.loginToken,UrlTail.projectToken, (result) =>
            {
                targetData = result;
            }, new KeyValuePair<string, string>("projectId", projectSelection.id));
            yield return StartCoroutine(requestModelSelectInfo);
            TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(targetData.data.ToString());
            HttpTool.projectToken = tokenData.token;
            IEnumerator requestModelTagInfo = HttpTool.GetRequest<RequestClass>(HttpTool.projectToken, UrlTail.projectModelType, (result) =>
            {
                targetData = result;
            });
            yield return StartCoroutine(requestModelTagInfo);

            if (targetData != null && targetData.data != null)
            {
                BuildingTypeData data = JsonConvert.DeserializeObject<BuildingTypeData>(targetData.data.ToString());
                GameManager.dataManager.nowBuildingTypeData = data;
                UiManager.NowBook.ChangePageTo(3);
            }


            uiWin.CloseUi();

        }
    }
}
