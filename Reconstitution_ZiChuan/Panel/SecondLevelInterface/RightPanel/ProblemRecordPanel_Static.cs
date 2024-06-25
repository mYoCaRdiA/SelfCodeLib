using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProblemRecordPanel_Static : StaticUi
{
    public Button btnClose;
    public Button btnFind;
    public Button btnReset;
    public Transform problemContent;
    public Panel_RecordNews panel_RecordNews;
    public Transform mainCanvas;
    public ProblemDetailPanel problemDetailPanel;
    public Transform problemDetailPanelParent;
    public Dropdown typeDropDown;
    public TMP_InputField timeInput;




    public Transform canvasLoaction { get; set; }
    private GetArProblemListBack getArProblemListBack;
    private Dictionary<Panel_RecordNews, ProblemDetailPanel> dic = new Dictionary<Panel_RecordNews, ProblemDetailPanel>();
    private Dictionary<string, string> typeDic = new Dictionary<string, string>();
    // Start is called before the first frame update
    void OnEnable()
    {
        GetProblemType();
        Ini();

        canvasLoaction = this.transform.parent.transform;

        btnClose.onClick.AddListener(() => { PanelManager.instance.CloseSecondPanel(); });
        btnFind.onClick.AddListener(() =>
        {
            FindByTypeAndTime();
        });
        btnReset.onClick.AddListener(() =>
        {
            ResetList();
        });

    }
    void GetProblemType()
    {
        IEnumerator workInfoRoutine = HttpTool.GetRequest<RequestClass>(HttpTool.projectToken, UrlTail.getProblemListNoPage, (xxx) =>
        {
            List<string> typeList = new List<string>();
            typeList.Add("问题类型");

            GetArProblemTypeBack getArProblemTypeBack = JsonConvert.DeserializeObject<GetArProblemTypeBack>(xxx.data.ToString());

            foreach (var item in getArProblemTypeBack.list)
            {

                typeList.Add(item.QueClassification);
                typeDic.Add(item.Id, item.QueClassification);
            }
            GenarateTypeDropDown(typeList);

        });

        StartCoroutine(workInfoRoutine);
    }
    void Ini()
    {
        foreach (Transform child in problemContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in problemDetailPanelParent)
        {
            Destroy(child.gameObject);
        }


        IEnumerator workInfoRoutine = HttpTool.GetRequest<RequestClass>(HttpTool.projectToken, UrlTail.getArProblemList, (xxx) =>
        {
            getArProblemListBack = JsonConvert.DeserializeObject<GetArProblemListBack>(xxx.data.ToString());

            foreach (var item in getArProblemListBack.list)
            {

                GenarateContent(item);

            }

        }, new KeyValuePair<string, string>("type", ""), new KeyValuePair<string, string>("date", ""));

        StartCoroutine(workInfoRoutine);

    }
    public void FindByTypeAndTime()
    {
        foreach (Transform child in problemContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in problemDetailPanelParent)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in getArProblemListBack.list)
        {
            string time;
            string month = item.ProblemDeadline.Split('/')[0];
            if (int.Parse(month) < 10)
            {
                month = month.Insert(0, "0");

            }
            time = item.ProblemDeadline.Split('/')[2].Substring(0, 4) + "-" + month + "-" + item.ProblemDeadline.Split('/')[1];
            if (typeDic.GetValueOrDefault(item.ProblemType) == typeDropDown.options[typeDropDown.value].text)
            {
                if (timeInput.text == "")
                {
                    GenarateContent(item);
                }
                else
                {
                    if (timeInput.text == time)
                    {
                        //2/14/2024 12:00:00 AM
                        Debug.Log("时间正确");
                        GenarateContent(item);
                    }
                }


            }
        }
    }
    public void GenarateContent(GetArProblemList data)
    {
        Panel_RecordNews newPrefab = Instantiate<Panel_RecordNews>(panel_RecordNews, problemContent);

        newPrefab.createText.text = "提  交  人: " + data.CreatorUserName;
        newPrefab.createTime.text = "提交时间: " + data.ProblemDeadline;
        newPrefab.description.text = "问题描述: " + data.ProblemDescription;

        ProblemDetailPanel detailPrefab = Instantiate<ProblemDetailPanel>(problemDetailPanel, mainCanvas);
        dic.Add(newPrefab, detailPrefab);
        detailPrefab.transform.SetParent(problemDetailPanelParent);
        detailPrefab.responsiblePerson.text = data.RectifyPersonName;
        detailPrefab.type.text = typeDic.GetValueOrDefault(data.ProblemType);
        detailPrefab.description.text = data.ProblemDescription;
        detailPrefab.partName.text = data.ProblemLocation;
        detailPrefab.deadLine.text = data.ProblemDeadline;
        detailPrefab.rectificationOpinions.text = data.RectifyOpinions;


        detailPrefab.gameObject.SetActive(false);
        newPrefab.button.onClick.AddListener(() =>
        {
            dic.GetValueOrDefault(newPrefab).gameObject.SetActive(true);
        });
        // 调整Content大小
        LayoutRebuilder.ForceRebuildLayoutImmediate(problemContent.GetComponent<RectTransform>());

        float allDelta = 0;
        foreach (RectTransform rect in problemContent)
        {
            allDelta += rect.sizeDelta.y;
        }
        allDelta += 1;
        problemContent.GetComponent<RectTransform>().sizeDelta = new Vector2(problemContent.GetComponent<RectTransform>().sizeDelta.x, allDelta);

    }
    private void GenarateTypeDropDown(List<string> options)
    {
        typeDropDown.ClearOptions();
        typeDropDown.AddOptions(options);
    }
    void ResetList()
    {
        Ini();
        timeInput.text = string.Empty;
        typeDropDown.value = 0;
    }
}


public class GetArProblemTypeBack
{
    public List<GetArProblemType> list { get; set; }

}
public class GetArProblemType
{
    public string QueClassification { get; set; }
    public string Id { get; set; }
}
public class GetArProblemListBack
{
    public List<GetArProblemList> list { get; set; }

}
public class GetArProblemList
{
    public string CreatorUserName { get; set; }
    public string RectifyPersonName { get; set; }
    public string ProblemDeadline { get; set; }
    public string ProblemType { get; set; }
    public string ProblemDescription { get; set; }

    public string ProblemPicture { get; set; }
    public string RectifyOpinions { get; set; }
    public string ProblemLocation { get; set; }
}
