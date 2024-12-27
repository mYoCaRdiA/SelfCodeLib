using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMove : MonoBehaviour
{
    public float moveSpeed = 10;
    public float viewSpeed = 100;

    [Header("最大上移角度")]
    [Range(0, 90f)]
    public float xAngleMax = 90f;
    [Header("最小下移角度")]
    [Range(300, 359f)]
    public float xAngleMin = 300;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Move();
            ViewChange();
        }
    }


    void Move()
    {
        Vector3 moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.E))
        {
            moveDir += Vector3.up;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            moveDir += Vector3.down;
        }

        Vector3 aimPos = moveDir * moveSpeed * Time.deltaTime + transform.position;
        
        transform.position = aimPos;
    }




    void ViewChange()
    {

        Vector2 mouseOffset = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 aimVec2 = mouseOffset * Time.deltaTime;

        float x;
        float y;
        //Input.touchCount != 0

        y = aimVec2.y;
        x = aimVec2.x;


        if (x != 0 || y != 0)
        {
            Vector3 nowAngle = transform.eulerAngles;
            nowAngle.y += viewSpeed * x;

            //---------------------------------
            float addValue = -viewSpeed * y;
            float xTemp = transform.eulerAngles.x + addValue;


            xTemp = xTemp % 360;
            if (xTemp < 0)
            {
                xTemp = 360 + xTemp;
            }
            if (!(xTemp > xAngleMax && xTemp < xAngleMin))
            {
                nowAngle.x = xTemp;
            }

            transform.eulerAngles = nowAngle;
        }
    }
}
