using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectBlock : MonoBehaviour
{
    //保存鼠标指针是否在UI按钮上，如果在UI按钮上则左键点击不进行射线检测
    public bool IsInUI = false;

    //保存选中的方块
    Transform obj;
    //保存按下左键的时间
    float StartTime;
    //保存抬起左键的时间
    float EndTime;

    //选中方块
    void Select()
    {
        //如果在UI按钮上则不进行射线检测
        if (IsInUI)
        {
            return;
        }
        //射线检测
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            obj = hit.transform;
            Debug.Log("SelectBlock");
        }
    }

    //添加方块后选中添加的方块
    public void SelectAdding(Transform transform)
    {
        obj = transform;
    }

    //x轴旋转
    public void RotateX()
    {
        obj.rotation *= Quaternion.AngleAxis(90, Vector3.right);
        Debug.Log(obj.rotation);
    }

    //y轴旋转
    public void RotateY()
    {
        obj.rotation *= Quaternion.AngleAxis(90, Vector3.up);
        Debug.Log(obj.rotation);
    }

    //z轴旋转
    public void RotateZ()
    {
        obj.rotation *= Quaternion.AngleAxis(90, Vector3.forward);
        Debug.Log(obj.rotation);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //判断左键按下
        if (Input.GetMouseButtonDown(0))
        {
            StartTime = Time.time;//记录按下左键时间
        }

        //判断左键抬起
        if (Input.GetMouseButtonUp(0))
        {
            EndTime = Time.time;//记录左键抬起的时间
            //如果按下左键时间间隔小于0.2秒则判定为点击
            if (EndTime - StartTime < 0.2f)
            {
                Select();
            }
        }
    }
}
