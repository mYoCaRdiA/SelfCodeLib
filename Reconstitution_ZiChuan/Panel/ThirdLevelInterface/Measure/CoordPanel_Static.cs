using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

using UnityEngine;
using UnityEngine.UI;

public class CoordPanel_Static : StaticUi
{
    private bool existPoint = false;
    private GameObject _obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnDisable()
    {
        Destroy(_obj);
    }

    public Tip_Window nowTipWin;
    // Update is called once per frame
    void Update()
    {
        if(MainPanel_Static.allModelAlphaValue == 0 && MainPanel_Static.realModel == 1)
        {
            if (Input.GetMouseButtonDown(0) &&GameManager.uiManager.IsOnUIElement(Input.mousePosition)==false)
            {
                nowTipWin= GameManager.uiManager.ShowUiWindow<Tip_Window>("模型和实景层都已关闭");
            }
        }
        else if(MainPanel_Static.allModelAlphaValue != 0 && MainPanel_Static.realModel == 1)
        {
            ModelCoord();
            
        }
        else if (MainPanel_Static.allModelAlphaValue == 0 && MainPanel_Static.realModel != 1)
        {
            RealCoord();
           
        }
        else
        {
            Coord();
            
        }
    }
    private void Coord()
    {
        if (!existPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MyArHitInfo hitInfo = default;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastAllLayer(mousePos);
                _obj = Instantiate(Resources.Load<GameObject>("PositionPoint"));
                _obj.transform.SetParent(GameObject.Find("TestModel").transform);
                _obj.transform.position = hitInfo.pointPostion;
                _obj.transform.LookAt(Camera.main.transform);


                float dis = (Camera.main.transform.position - _obj.transform.position).magnitude;
                float size = dis * 0.04f;
                size = Mathf.Clamp(size, 0.02f, 50f);
                _obj.transform.localScale = new Vector3(size, size, size);

                Transform temp = _obj.transform.Find("PositionLabel/Canvas");
                //显示的是相对坐标
                //临时数据
                //string _modelPath = GameController.Instance.curController.GetModelPath();

                temp.Find("X/Text").GetComponent<Text>().text = "X ：" + hitInfo.pointPostion.x.ToString("f4");
                temp.Find("Y/Text").GetComponent<Text>().text = "Y ：" + hitInfo.pointPostion.y.ToString("f4");
                temp.Find("Z/Text").GetComponent<Text>().text = "Z ：" + hitInfo.pointPostion.z.ToString("f4");
                existPoint = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(_obj);
                MyArHitInfo hitInfo = default;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastAllLayer(mousePos);
                _obj = Instantiate(Resources.Load<GameObject>("PositionPoint"));
                _obj.transform.SetParent(GameObject.Find("TestModel").transform);
                _obj.transform.position = hitInfo.pointPostion;
                _obj.transform.LookAt(Camera.main.transform);


                float dis = (Camera.main.transform.position - _obj.transform.position).magnitude;
                float size = dis * 0.04f;
                size = Mathf.Clamp(size, 0.02f, 50f);
                _obj.transform.localScale = new Vector3(size, size, size);

                Transform temp = _obj.transform.Find("PositionLabel/Canvas");
                //显示的是相对坐标
                //临时数据
                //string _modelPath = GameController.Instance.curController.GetModelPath();

                temp.Find("X/Text").GetComponent<Text>().text = "X ：" + hitInfo.pointPostion.x.ToString("f4");
                temp.Find("Y/Text").GetComponent<Text>().text = "Y ：" + hitInfo.pointPostion.y.ToString("f4");
                temp.Find("Z/Text").GetComponent<Text>().text = "Z ：" + hitInfo.pointPostion.z.ToString("f4");
            }
        }
    }
    private void RealCoord()
    {
        if (!existPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MyArHitInfo hitInfo = default;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastTrackMesh(mousePos);
                _obj = Instantiate(Resources.Load<GameObject>("PositionPoint"));
                _obj.transform.SetParent(GameObject.Find("TestModel").transform);
                _obj.transform.position = hitInfo.pointPostion;
                _obj.transform.LookAt(Camera.main.transform);


                float dis = (Camera.main.transform.position - _obj.transform.position).magnitude;
                float size = dis * 0.04f;
                size = Mathf.Clamp(size, 0.02f, 50f);
                _obj.transform.localScale = new Vector3(size, size, size);

                Transform temp = _obj.transform.Find("PositionLabel/Canvas");
                //显示的是相对坐标
                //临时数据
                //string _modelPath = GameController.Instance.curController.GetModelPath();

                temp.Find("X/Text").GetComponent<Text>().text = "X ：" + hitInfo.pointPostion.x.ToString("f4");
                temp.Find("Y/Text").GetComponent<Text>().text = "Y ：" + hitInfo.pointPostion.y.ToString("f4");
                temp.Find("Z/Text").GetComponent<Text>().text = "Z ：" + hitInfo.pointPostion.z.ToString("f4");
                existPoint = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(_obj);
                MyArHitInfo hitInfo = default;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastTrackMesh(mousePos);
                _obj = Instantiate(Resources.Load<GameObject>("PositionPoint"));
                _obj.transform.SetParent(GameObject.Find("TestModel").transform);
                _obj.transform.position = hitInfo.pointPostion;
                _obj.transform.LookAt(Camera.main.transform);


                float dis = (Camera.main.transform.position - _obj.transform.position).magnitude;
                float size = dis * 0.04f;
                size = Mathf.Clamp(size, 0.02f, 50f);
                _obj.transform.localScale = new Vector3(size, size, size);

                Transform temp = _obj.transform.Find("PositionLabel/Canvas");
                //显示的是相对坐标
                //临时数据
                //string _modelPath = GameController.Instance.curController.GetModelPath();

                temp.Find("X/Text").GetComponent<Text>().text = "X ：" + hitInfo.pointPostion.x.ToString("f4");
                temp.Find("Y/Text").GetComponent<Text>().text = "Y ：" + hitInfo.pointPostion.y.ToString("f4");
                temp.Find("Z/Text").GetComponent<Text>().text = "Z ：" + hitInfo.pointPostion.z.ToString("f4");
            }
        }
    }
    private void ModelCoord()
    {
        if (!existPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MyArHitInfo hitInfo = default;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastGenerateModel(mousePos);
                _obj = Instantiate(Resources.Load<GameObject>("PositionPoint"));
                _obj.transform.SetParent(GameObject.Find("默认").transform);
                _obj.transform.position = hitInfo.pointPostion;
                _obj.transform.LookAt(Camera.main.transform);


                float dis = (Camera.main.transform.position - _obj.transform.position).magnitude;
                float size = dis * 0.04f;
                size = Mathf.Clamp(size, 0.02f, 50f);
                _obj.transform.localScale = new Vector3(size, size, size);

                Transform temp = _obj.transform.Find("PositionLabel/Canvas");
                //显示的是相对坐标
                //临时数据
                //string _modelPath = GameController.Instance.curController.GetModelPath();

                temp.Find("X/Text").GetComponent<Text>().text = "X ：" + hitInfo.pointPostion.x.ToString("f4");
                temp.Find("Y/Text").GetComponent<Text>().text = "Y ：" + hitInfo.pointPostion.y.ToString("f4");
                temp.Find("Z/Text").GetComponent<Text>().text = "Z ：" + hitInfo.pointPostion.z.ToString("f4");
                existPoint = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(_obj);
                MyArHitInfo hitInfo = default;
                Vector3 mousePos = Input.mousePosition;
                hitInfo = ArEngine.ScreenRaycastGenerateModel(mousePos);
                _obj = Instantiate(Resources.Load<GameObject>("PositionPoint"));
                _obj.transform.SetParent(GameObject.Find("默认").transform);
                _obj.transform.position = hitInfo.pointPostion;
                _obj.transform.LookAt(Camera.main.transform);


                float dis = (Camera.main.transform.position - _obj.transform.position).magnitude;
                float size = dis * 0.04f;
                size = Mathf.Clamp(size, 0.02f, 50f);
                _obj.transform.localScale = new Vector3(size, size, size);

                Transform temp = _obj.transform.Find("PositionLabel/Canvas");
                //显示的是相对坐标
                //临时数据
                //string _modelPath = GameController.Instance.curController.GetModelPath();

                temp.Find("X/Text").GetComponent<Text>().text = "X ：" + hitInfo.pointPostion.x.ToString("f4");
                temp.Find("Y/Text").GetComponent<Text>().text = "Y ：" + hitInfo.pointPostion.y.ToString("f4");
                temp.Find("Z/Text").GetComponent<Text>().text = "Z ：" + hitInfo.pointPostion.z.ToString("f4");
            }
        }
    }
}
