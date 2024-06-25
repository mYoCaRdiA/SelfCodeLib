
using UnityEngine;
using UnityEngine.UI;

public class ClearDistancePanel_Static : StaticUi
{
    public PrefabInfo ballPrefab;
    public LineRenderer line;


    private GameObject ballEntity;
    private GameObject ballEntity1;
    private GameObject disLabel;
    private Transform startTrans;
    private Transform endTrans;
    private Text disText;
    private GameObject startPoint;
    private GameObject endPoint;
    private bool existPoint = false;

    // Start is called before the first frame update
    [System.Obsolete]
    void OnEnable()
    {
        line.material = default;
        line.SetVertexCount(2);//设置两点
        line.SetColors(Color.yellow, Color.red); //设置直线颜色
        line.SetWidth(0.01f, 0.01f);//设置直线宽度
        line.gameObject.SetActive(false);
        GameObject prefab = Resources.Load<GameObject>("DistanceLabel");
        disLabel = Instantiate(prefab);
        disLabel.SetActive(false);
        startPoint = new GameObject("StartPoint");
        endPoint = new GameObject("EndPoint");
        startTrans = startPoint.transform;
        endTrans = endPoint.transform;
        disText = disLabel.transform.Find("Canvas/Text2").GetComponent<Text>();
        
    }
    public Tip_Window nowTipWin;
    // Update is called once per frame
    void Update()
    {
        Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("AR Camera").transform.forward);
        disLabel.transform.rotation = lookRotation;
        

        if (MainPanel_Static.allModelAlphaValue == 0 && MainPanel_Static.realModel == 1)
        {
            if (Input.GetMouseButtonDown(0) && GameManager.uiManager.IsOnUIElement(Input.mousePosition) == false)
            {
                nowTipWin = GameManager.uiManager.ShowUiWindow<Tip_Window>("模型和实景层都已关闭");
            }

        }
        else if (MainPanel_Static.allModelAlphaValue != 0 && MainPanel_Static.realModel == 1)
        {
            ModelMeasure_ClearDistance();
        }
        else if (MainPanel_Static.allModelAlphaValue == 0 && MainPanel_Static.realModel != 1)
        {
            RealMeasure_ClearDistance();
        }
        else
        {
            Measure_ClearDistance();
        }
        SetLabelSize();
    }
    void OnDisable()
    {
        Destroy(startPoint);
        Destroy(endPoint);
        Destroy(disLabel);
        Destroy(ballEntity);
        Destroy(ballEntity1);
    }
    public void RealMeasure_ClearDistance()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!existPoint)
            {
                MyArHitInfo hitInfo = default;
                PrefabInfo ballInfo = null;
                ballInfo = ballPrefab;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastTrackMesh(mousePos);
                ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                ballEntity.transform.position = hitInfo.pointPostion;
                line.SetPosition(0, hitInfo.pointPostion);
                line.SetPosition(1, hitInfo.pointPostion);
                startTrans.position = hitInfo.pointPostion;

                Vector3 Point = hitInfo.pointPostion;
                RaycastHit hit = HitRay(mousePos);
                Ray downRay = new Ray(Point + 0.3f * hit.normal, hit.normal);
                //射线碰到了物体
                RaycastHit hit1;
                if (Physics.Raycast(downRay, out hit1))
                {

                    ballEntity1 = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity1.transform.position = hit1.point;
                    line.SetPosition(1, hit1.point);
                    line.gameObject.SetActive(true);
                    endTrans.position = hit1.point;
                    SetLabelPosition(startTrans, endTrans);
                    SetLabelText(startTrans, endTrans);
                }
                existPoint = true;

            }
            else
            {
                Destroy(ballEntity);
                Destroy(ballEntity1);
                MyArHitInfo hitInfo = default;
                PrefabInfo ballInfo = null;
                ballInfo = ballPrefab;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastTrackMesh(mousePos);
                ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                ballEntity.transform.position = hitInfo.pointPostion;
                line.SetPosition(0, hitInfo.pointPostion);
                line.SetPosition(1, hitInfo.pointPostion);
                startTrans.position = hitInfo.pointPostion;

                Vector3 Point = hitInfo.pointPostion;
                RaycastHit hit = HitRay(mousePos);
                Ray downRay = new Ray(Point + 0.3f * hit.normal, hit.normal);
                //射线碰到了物体
                RaycastHit hit1;
                if (Physics.Raycast(downRay, out hit1))
                {

                    ballEntity1 = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity1.transform.position = hit1.point;
                    line.SetPosition(1, hit1.point);
                    line.gameObject.SetActive(true);
                    endTrans.position = hit1.point;
                    SetLabelPosition(startTrans, endTrans);
                    SetLabelText(startTrans, endTrans);
                }
                existPoint = true;
            }

        }



    }
    public void ModelMeasure_ClearDistance()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!existPoint)
            {
                MyArHitInfo hitInfo = default;
                PrefabInfo ballInfo = null;
                ballInfo = ballPrefab;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastGenerateModel(mousePos);
                ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                ballEntity.transform.position = hitInfo.pointPostion;
                line.SetPosition(0, hitInfo.pointPostion);
                line.SetPosition(1, hitInfo.pointPostion);
                startTrans.position = hitInfo.pointPostion;

                Vector3 Point = hitInfo.pointPostion;
                RaycastHit hit = HitRay(mousePos);
                Ray downRay = new Ray(Point + 0.3f * hit.normal, hit.normal);
                //射线碰到了物体
                RaycastHit hit1;
                if (Physics.Raycast(downRay, out hit1))
                {

                    ballEntity1 = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity1.transform.position = hit1.point;
                    line.SetPosition(1, hit1.point);
                    line.gameObject.SetActive(true);
                    endTrans.position = hit1.point;
                    SetLabelPosition(startTrans, endTrans);
                    SetLabelText(startTrans, endTrans);
                }
                existPoint = true;

            }
            else
            {
                Destroy(ballEntity);
                Destroy(ballEntity1);
                MyArHitInfo hitInfo = default;
                PrefabInfo ballInfo = null;
                ballInfo = ballPrefab;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastGenerateModel(mousePos);
                ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                ballEntity.transform.position = hitInfo.pointPostion;
                line.SetPosition(0, hitInfo.pointPostion);
                line.SetPosition(1, hitInfo.pointPostion);
                startTrans.position = hitInfo.pointPostion;

                Vector3 Point = hitInfo.pointPostion;
                RaycastHit hit = HitRay(mousePos);
                Ray downRay = new Ray(Point + 0.3f * hit.normal, hit.normal);
                //射线碰到了物体
                RaycastHit hit1;
                if (Physics.Raycast(downRay, out hit1))
                {

                    ballEntity1 = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity1.transform.position = hit1.point;
                    line.SetPosition(1, hit1.point);
                    line.gameObject.SetActive(true);
                    endTrans.position = hit1.point;
                    SetLabelPosition(startTrans, endTrans);
                    SetLabelText(startTrans, endTrans);
                }
                existPoint = true;
            }

        }



    }
    public void Measure_ClearDistance()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!existPoint)
            {
                MyArHitInfo hitInfo = default;
                PrefabInfo ballInfo = null;
                ballInfo = ballPrefab;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastAllLayer(mousePos);
                ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                ballEntity.transform.position = hitInfo.pointPostion;
                line.SetPosition(0, hitInfo.pointPostion);
                line.SetPosition(1, hitInfo.pointPostion);
                startTrans.position = hitInfo.pointPostion;

                Vector3 Point = hitInfo.pointPostion;
                RaycastHit hit = HitRay(mousePos);
                Ray downRay = new Ray(Point + 0.3f * hit.normal, hit.normal);
                //射线碰到了物体
                RaycastHit hit1;
                if (Physics.Raycast(downRay, out hit1))
                {

                    ballEntity1 = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity1.transform.position = hit1.point;
                    line.SetPosition(1, hit1.point);
                    line.gameObject.SetActive(true);
                    endTrans.position = hit1.point;
                    SetLabelPosition(startTrans, endTrans);
                    SetLabelText(startTrans, endTrans);
                }
                existPoint = true;

            }
            else
            {
                Destroy(ballEntity);
                Destroy(ballEntity1);
                MyArHitInfo hitInfo = default;
                PrefabInfo ballInfo = null;
                ballInfo = ballPrefab;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastAllLayer(mousePos);
                ballEntity = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                ballEntity.transform.position = hitInfo.pointPostion;
                line.SetPosition(0, hitInfo.pointPostion);
                line.SetPosition(1, hitInfo.pointPostion);
                startTrans.position = hitInfo.pointPostion;

                Vector3 Point = hitInfo.pointPostion;
                RaycastHit hit = HitRay(mousePos);
                Ray downRay = new Ray(Point + 0.3f * hit.normal, hit.normal);
                //射线碰到了物体
                RaycastHit hit1;
                if (Physics.Raycast(downRay, out hit1))
                {

                    ballEntity1 = GameObjectPoolTool.GetFromPoolForce(true, ballInfo.path);
                    ballEntity1.transform.position = hit1.point;
                    line.SetPosition(1, hit1.point);
                    line.gameObject.SetActive(true);
                    endTrans.position = hit1.point;
                    SetLabelPosition(startTrans, endTrans);
                    SetLabelText(startTrans, endTrans);
                }
                existPoint = true;
            }

        }



    }
    /// <summary>
    /// 返回射线
    /// </summary>
    /// <param name="screenPostion"></param>
    /// <returns></returns>
    private RaycastHit HitRay(Vector2 screenPostion)
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(screenPostion);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, -1))
        {
            return hitInfo;
        }
        else
        {
            return hitInfo;
        }
    }
    /// <summary>
    /// 设置标签位置
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void SetLabelPosition(Transform start, Transform end)
    {

        Vector3 StartPs = start.transform.position;
        Vector3 EndPs = end.transform.position;
        disLabel.transform.position = (StartPs + EndPs) / 2f;
        //disLabel.transform.position = StartPs;
        Vector3 dir = (GameObject.Find("AR Camera").transform.position - disLabel.transform.position).normalized;


        Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("AR Camera").transform.forward);
       


       
        disLabel.transform.rotation= lookRotation;
        
        SetLabelSize();
        disLabel.SetActive(true);


    }
    /// <summary>
    /// 设置标签的文本内容
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void SetLabelText(Transform start, Transform end)
    {
        string distance = Distance(start.transform.position, end.transform.position);
        //m_Text1.text = distance;
        disText.text = distance;
    }

    /// <summary>
    /// 计算两个点的距离
    /// </summary>
    /// <param name="StartPs"></param>
    /// <param name="endPs"></param>
    /// <returns></returns>
    private string Distance(Vector3 StartPs, Vector3 endPs)
    {
        float dis = (StartPs - endPs).magnitude;
        return (dis >= 1) ? dis.ToString("f2") + "m" : (dis * 100).ToString("f2") + "cm";
    }
    /// <summary>
    /// 设置标签的大小
    /// </summary>
    private void SetLabelSize()
    {


        float direction = (GameObject.Find("AR Camera").transform.position - disLabel.transform.position).magnitude;
        float size = direction * 0.04f;
        size = Mathf.Clamp(size, 0.02f, 50f);
        disLabel.transform.localScale = new Vector3(size, size, size);
    }
}
