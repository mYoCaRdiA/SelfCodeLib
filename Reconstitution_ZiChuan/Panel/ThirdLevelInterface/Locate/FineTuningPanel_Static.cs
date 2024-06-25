using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public enum tranControlType
{
    LeftMove,
    RightMove,
    UpMove,
    BelowMove,
    ForwardMove,
    BackMove,
    LeftRotate,
    RightRotate,
    UpRotate,
    BelowRotate,
    ForwardRotate,
    BackRotate
}

/// <summary>
/// 微调脚本
/// </summary>
public class FineTuningPanel_Static : StaticUi
{
    
    public Slider coefficientSlider;
    public InputField coefficientInput;
    private Transform curCamera;


    public Dropdown lengthUnitDropdown;
    public Dropdown angleUnitDropdown;

    private Transform curModelParent;

    private Transform centerTran;

    //基数因子
    private float rotateDivisor = 1.0f;
    private float moveDivisor = 1.0f;
    //滑动条因子
    private float coefficientValue;
    //单位因子
    private float lengthUnitCoefficient;
    private float angleUnitCoefficient;

    private Coroutine coroutine;

    void Start()
    {
        coefficientInput.onEndEdit.AddListener(WriteValue);
        coefficientSlider.onValueChanged.AddListener(ChangeSliderValue);
        coefficientSlider.value = coefficientSlider.minValue;
        curModelParent = GameObject.Find("ModelParent").transform; 
            curCamera = GameObject.Find("AR Session Origin").transform;
        lengthUnitDropdown.onValueChanged.AddListener(ChangeLengthUnit);
        lengthUnitDropdown.value = 0;
        lengthUnitCoefficient = Mathf.Pow(10, -3);

        angleUnitDropdown.onValueChanged.AddListener(ChangeAnglehUnit);
        angleUnitDropdown.value = 0;
        angleUnitCoefficient = 1 / Mathf.Pow(60, 0);

        

    }

    private void OnDisable()
    {
        if (centerTran != null)
        {
            Destroy(centerTran.gameObject);
        }
    }

    

    /// <summary>
    /// 创建三维坐标系
    /// </summary>
    /// <param name="name"></param>
    /// <param name="axis"></param>
    /// <param name="color"></param>
    /// <param name="parent"></param>
    private void CreateArrow(string name, Vector3 axis, Color color, Transform parent)
    {
        GameObject _obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AxisPrefab"));
        _obj.transform.SetParent(parent);
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.forward = axis;
        _obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    }

    /// <summary>
    /// 修改长度的单位
    /// </summary>
    /// <param name="index"></param>
    private void ChangeLengthUnit(int index)
    {
        //特殊操作，列表中米是3，所有减3
        int value = index - 3;

        //第四个是千米，是3次方
        if (value > 0)
        {
            value = 3;
        }

        lengthUnitCoefficient = Mathf.Pow(10, value);
    }

    /// <summary>
    /// 修改角度的单位
    /// </summary>
    private void ChangeAnglehUnit(int index)
    {
        angleUnitCoefficient = 1 / Mathf.Pow(60, index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    public void ModelTranController1(int type)
    {
        coroutine = StartCoroutine(longClick(type));
    }

    public void EndModelTranController1()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    /// <summary>
    /// 长按逻辑
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerator longClick(int type)
    {
        float intervalTime = 0.7f;

        while (true)
        {
            Vector3 direction = Vector3.forward;
            bool isMove = true;
            Transform temp = null;
            Vector3 initCenter = Vector3.zero;
            Vector3 offset = Vector3.zero;

            if (centerTran == null)
            {
                temp = curModelParent;
            }
            else
            {
                temp = centerTran;
                offset = centerTran.position - curModelParent.position;
            }

            switch (type)
            {
                case (int)tranControlType.LeftMove:
                    direction = -temp.right;
                    break;
                case (int)tranControlType.RightMove:
                    direction = temp.right;
                    break;
                case (int)tranControlType.UpMove:
                    direction = temp.up;
                    break;
                case (int)tranControlType.BelowMove:
                    direction = -temp.up;
                    break;
                case (int)tranControlType.ForwardMove:
                    direction = temp.forward;
                    break;
                case (int)tranControlType.BackMove:
                    direction = -temp.forward;
                    break;
                case (int)tranControlType.LeftRotate:
                    direction = -temp.up;
                    isMove = false;
                    break;
                case (int)tranControlType.RightRotate:
                    direction = temp.up;
                    isMove = false;
                    break;

            }

            if (isMove)
            {
                if (centerTran == null)
                {
                    curCamera.position -= direction * coefficientValue * lengthUnitCoefficient * moveDivisor;
                }
                else
                {

                    curCamera.position -= direction * coefficientValue * lengthUnitCoefficient * moveDivisor;
                }
            }
            else
            {
                if (centerTran == null)
                {

                    curCamera.eulerAngles -= direction * coefficientValue * angleUnitCoefficient * rotateDivisor;
                }
                else
                {
                    centerTran.Rotate(direction * coefficientValue * angleUnitCoefficient * rotateDivisor, Space.World);

                    curCamera.RotateAround(centerTran.position, direction, coefficientValue * angleUnitCoefficient * rotateDivisor);
                }
            }

            yield return new WaitForSeconds(intervalTime);
            intervalTime = 0.001f;
        }
    }




    /// <summary>
    /// 修改计算的基数
    /// </summary>
    /// <param name="_value"></param>
    private void ChangeSliderValue(float _value)
    {
        coefficientValue = _value;
        coefficientInput.text = _value.ToString("f0");
    }

    private void WriteValue(string str)
    {
        float value = 0;
        if (float.TryParse(str, out value))
        {
            if (value < coefficientSlider.minValue)
                value = coefficientSlider.minValue;
            if (value > coefficientSlider.maxValue)
                value = coefficientSlider.maxValue;


            coefficientSlider.value = value;
        }
    }


}
