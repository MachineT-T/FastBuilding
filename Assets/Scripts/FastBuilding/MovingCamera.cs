using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    private int MouseWheelSensitivity = 5; //滚轮灵敏度设置

    private float xSpeed = 250.0f; //旋转视角时相机x轴转速
    private float ySpeed = 120.0f; //旋转视角时相机y轴转速

    private float EulerX = 0.0f; //存储相机的euler角
    private float EulerY = 0.0f; //存储相机的euler角

    private Quaternion storeRotation; //存储相机的姿态四元数
    private Vector3 initPosition; //平移时用于存储平移的起点位置
    private Vector3 cameraX; //相机的x轴方向向量
    private Vector3 cameraY; //相机的y轴方向向量

    private Vector3 initScreenPos; //中键刚按下时鼠标的屏幕坐标
    private Vector3 curScreenPos; //当前鼠标的屏幕坐标
    void Start()
    {
        //储存相机的旋转角以及四元数
        EulerX = transform.eulerAngles.x;
        EulerY = transform.eulerAngles.y;
        storeRotation = Quaternion.Euler(EulerX, EulerY, 0);
    }

    void Update()
    {
        //鼠标右键旋转功能
        if (Input.GetMouseButton(1))
        {
            EulerY += Input.GetAxis("Mouse X") * ySpeed * 0.02f;//鼠标在X轴上移动时，视角绕Y轴旋转
            EulerX -= Input.GetAxis("Mouse Y") * xSpeed * 0.02f;//鼠标在Y轴上移动时，视角绕X轴旋转

            //修改欧拉角改变后的四元数
            storeRotation = Quaternion.Euler(EulerX, EulerY, 0);

            //将改变后四元数应用到相机上
            transform.rotation = storeRotation;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0) //鼠标滚轮缩放功能
        {
            //获取相机Z轴方向，将相机的位置向Z轴方向位移
            transform.position += Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity * transform.forward;
        }

        //鼠标中键平移功能
        if (Input.GetMouseButtonDown(2))
        {
            //保存相机的X轴、Y轴方向
            cameraX = transform.right;
            cameraY = transform.up;

            //保存鼠标的初始位置
            initScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

            //保存平移前相机的初始位置
            initPosition = transform.position;

        }

        if (Input.GetMouseButton(2))
        {
            //保存鼠标当前位置
            curScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            //通过鼠标在两个方向上的位移乘相机的X轴、Y轴方向求出相机应做出的位移
            transform.position = initPosition - 0.01f * ((curScreenPos.x - initScreenPos.x) * cameraX + (curScreenPos.y - initScreenPos.y) * cameraY);
        }
    }
}