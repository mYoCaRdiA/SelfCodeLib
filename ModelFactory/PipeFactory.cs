

using static PipeConstructData;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using System;
using System.IO;

public class PipeFactory
{
    public static IEnumerator GenerateUrlPipes(string modelUrlPath, Action<float> progressUpdate = null, Transform root = null)
    {
        if (progressUpdate != null) progressUpdate.Invoke(0);
        GameObject fatherGameObj = new GameObject("�ܵ�ģ��");
        Transform fatherTrans = fatherGameObj.transform;
        fatherTrans.parent = root;

        string dataPath = IoTools.Model_IO.GetModelDownloadPath(modelUrlPath);
        string jsonPath = Path.Combine(dataPath, "sysmodelList", "sysmodelList.json");

        string jsonString = IoTools.ReadFileString(jsonPath);
        if (jsonString != "")
        {
            PipeConstructData pipeConstructData = JsonConvert.DeserializeObject<PipeConstructData>(jsonString);
            CreatePipes(pipeConstructData.circularMeps, fatherTrans);
            if (progressUpdate != null) progressUpdate(30);
            CreatePipes(pipeConstructData.rectMeps.Where(o => o.type == "���").ToArray(), fatherTrans);
            if (progressUpdate != null) progressUpdate(60);
            CreatePipes(pipeConstructData.rectMeps.Where(o => o.type == "�ż�").ToArray(), fatherTrans);
            if (progressUpdate != null) progressUpdate(100);
            //����Ҫ��
            //CreatePipes(pipeConstructData.ellipseMeps);
            Debug.Log("�ܵ��������");
        }
        else
        {
            Debug.LogError("jsonΪ���ַ���");
        }
        yield return null;
    }

    /// <summary>
    /// �����ܵ�ģ��
    /// </summary>
    /// <param name="array"></param>
    /// <param name="modelPath"></param>
    static void CreatePipes(PipelineClass[] array, Transform modelParent = null)
    {
        for (int i = 0; i < array.Length; i++)
        {
            string type = array[i].type;
            float width = array[i].width;
            float height = array[i].height;
            float diameter = array[i].diameter;
            float length = array[i].length;
            string _name = array[i].name;

            Vector3 startpPoint = new Vector3(-array[i].startPoint.X * 0.3048f, array[i].startPoint.Z * 0.3048f, -array[i].startPoint.Y * 0.3048f);
            Vector3 endPoint = new Vector3(-array[i].endPoint.X * 0.3048f, array[i].endPoint.Z * 0.3048f, -array[i].endPoint.Y * 0.3048f);
            Vector3 basex = array[i].base_x == null ? (new Vector3()) : (new Vector3(array[i].base_x.X, array[i].base_x.Z, array[i].base_x.Y));
            string colorStr = array[i].color;
            string[] strArray = colorStr.Split(',');
            Color color = new Color(float.Parse(strArray[0]) / 255, float.Parse(strArray[1]) / 255, float.Parse(strArray[2]) / 255);

            Transform temp = null;
            switch (type)
            {
                case "�ܵ�":
                    temp = CreateCircularPipe(_name, startpPoint, endPoint, color, diameter, modelParent);
                    break;
                case "���":
                    temp = CreateWindPipe(_name, startpPoint, endPoint, color, basex, width, height, modelParent);
                    break;
                case "�ż�":
                    temp = CreateQiaoJia(_name, startpPoint, endPoint, color, basex, width, height, modelParent);
                    break;
            }
            if (temp == null)
            {
                continue;
            }
            //modelInfoDic.Add(temp, new ModelInfo { modelPath = modelPath });
            //Renderer renderer = temp.GetComponent<Renderer>();
            // Material[] mats = renderer.materials;
            //for (int j = 0; j < mats.Length; j++)
            //{
            //    ModelControl.allModelMaterialList.Add(new HHMaterial(mats[j], renderer.gameObject, renderer.name, -1));
            //}
        }
    }



    /// <summary>
    /// �������㴴��һ���ܵ�
    /// </summary>
    /// <param name="startPosition">��ʼλ��</param>
    /// <param name="endPosition">����λ��</param>
    static Transform CreateCircularPipe(string name, Vector3 startPosition, Vector3 endPosition, Color color, float diameter = 0.1f, Transform parent = null)
    {
        Vector3 rightPosition = (startPosition + endPosition) / 2;
        Vector3 rightRotation = endPosition - startPosition;
        float HalfLength = Vector3.Distance(startPosition, endPosition) / 2;
        float LThickness = diameter;//�ߵĴ�ϸ

        Transform _guandao = GameObjectPoolTool.GetFromPoolForce(true, "Assets/Resources/ModelTemp/Pipe/Pipeline.prefab").transform;
        _guandao.SetParent(parent);
        _guandao.position = rightPosition;
        _guandao.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
        _guandao.localScale = new Vector3(LThickness * 0.3f, HalfLength, LThickness * 0.3f);
        _guandao.name = name;
       

        Material mat = _guandao.GetComponent<MeshRenderer>().material;
        if (mat != null)
        {
            mat.color = color;
        }

        return _guandao;
    }
    static Transform CreateWindPipe(string name, Vector3 startPosition, Vector3 endPosition, Color color, Vector3 basex, float width = 0.1f, float height = 0.1f, Transform parent = null)
    {
        Vector3 rightRotation = endPosition - startPosition;
        Vector3 centerPosition = (startPosition + endPosition) / 2;
        float dis = Vector3.Distance(startPosition, endPosition);

        #region ����
        //���ɹ���
        Transform _guandao = GameObjectPoolTool.GetFromPoolForce(true, "Assets/Resources/ModelTemp/Pipe/WindPipe.prefab").transform;
        _guandao.SetParent(parent);
        _guandao.transform.position = centerPosition;
        //���� 
        float initLength = _guandao.GetComponent<MeshFilter>().mesh.bounds.size.x;
        float scale = dis / initLength;
        float wScale = 0.3048f / _guandao.GetComponent<MeshFilter>().mesh.bounds.size.z;
        float hScale = 0.3048f / _guandao.GetComponent<MeshFilter>().mesh.bounds.size.y;
        _guandao.transform.localScale = new Vector3(scale, height * hScale, width * wScale);
        //�ƶ�λ�� 
        _guandao.SetParent(parent);
        _guandao.transform.position = centerPosition;
        //ˮƽ��ת
        Vector3 s_point = new Vector3(startPosition.x, 0, startPosition.z);
        Vector3 e_point = new Vector3(endPosition.x, 0, endPosition.z);
        float angle = Vector3.Angle(e_point - s_point, Vector3.right);
        //Debug.LogError(angle);
        _guandao.transform.rotation = Quaternion.Euler(0, -angle, 0);
        //��б��ת
        float angle2 = Vector3.Angle(endPosition - startPosition, e_point - s_point);
        _guandao.transform.rotation = Quaternion.Euler(0, (endPosition.z < startPosition.z ? 1 : -1) * angle, (endPosition.y > startPosition.y ? 1 : -1) * angle2);
        _guandao.name = name;

        //
        if ((Mathf.Abs((e_point - s_point).x) < 0.01 && Mathf.Abs((e_point - s_point).z) < 0.01))
        {
            float angle3 = Vector3.Angle(Vector3.forward, basex) * (basex.x > 0 ? 1 : -1);
            _guandao.transform.rotation = Quaternion.Euler(0, angle3, 90);
        }
        #endregion

        Material mat = _guandao.GetComponent<MeshRenderer>().material;
        if (mat != null)
        {
            mat.color = color;
        }

        return _guandao;
    }
    static Transform CreateQiaoJia(string name, Vector3 startPosition, Vector3 endPosition, Color color, Vector3 basex, float width = 0.1f, float height = 0.1f, Transform parent = null)
    {
        Vector3 initPosition = Vector3.zero;
        Vector3 centerPosition = (startPosition + endPosition) / 2;
        float dis = Vector3.Distance(startPosition, endPosition);

        Transform _guandao = GameObjectPoolTool.GetFromPoolForce(true, "Assets/Resources/ModelTemp/Pipe/QiaoJia.prefab").transform;
        _guandao.SetParent(parent);
        _guandao.transform.position = centerPosition;

        //���� 

        float initLength = _guandao.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.x;
        float scale = dis / initLength;
        float wScale = 0.3048f / _guandao.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.z;
        float hScale = 0.3048f / _guandao.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y;
        _guandao.transform.localScale = new Vector3(scale, height * hScale, width * wScale);

        //�ƶ�λ�� 
        _guandao.SetParent(parent);
        _guandao.transform.position = centerPosition;

        //ˮƽ��ת
        Vector3 s_point = new Vector3(startPosition.x, 0, startPosition.z);
        Vector3 e_point = new Vector3(endPosition.x, 0, endPosition.z);
        float angle = Vector3.Angle(e_point - s_point, Vector3.right);
        //Debug.LogError(angle);
        _guandao.transform.rotation = Quaternion.Euler(0, -angle, 0);

        //��б��ת
        float angle2 = Vector3.Angle(endPosition - startPosition, e_point - s_point);
        _guandao.transform.rotation = Quaternion.Euler(0, (endPosition.z < startPosition.z ? 1 : -1) * angle, (endPosition.y > startPosition.y ? 1 : -1) * angle2);
        if ((Mathf.Abs((e_point - s_point).x) < 0.01 && Mathf.Abs((e_point - s_point).z) < 0.01))
        {
            float angle3 = Vector3.Angle(Vector3.forward, basex) * (basex.x < 0 ? 1 : -1);
            _guandao.transform.rotation = Quaternion.Euler(0, angle3, 90);
        }
        _guandao.GetChild(0).name = name;

        Material mat = _guandao.GetChild(0).GetComponent<MeshRenderer>().material;
        if (mat != null)
        {
            mat.color = color;
        }

        return _guandao.GetChild(0);

    }
}
