using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectBlock : MonoBehaviour
{
    //保存鼠标指针是否在UI按钮上，如果在UI按钮上则左键点击不进行射线检测
    public bool IsInUI = false;

    //保存选中的方块
    Transform trans;
    //保存按下左键的时间
    float StartTime;
    //保存抬起左键的时间
    float EndTime;

    //保存左键按下时的射线检测信息
    RaycastHit StartHit;
    //保存左线松开时的射线检测信息
    RaycastHit EndHit;

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
            //如果射线碰撞的为地面则不选中
            if (IsGround(hit.transform))
            {
                return;
            }
            //去除原方块的线框
            if (trans != null)
            {
                Destroy(trans.gameObject.GetComponent("ShowBoxCollider"));
            }
            //保存选中的方块
            trans = hit.transform;
            //为选中的方块画线
            trans.gameObject.AddComponent<ShowBoxCollider>();
        }
    }

    //添加方块后选中添加的方块
    public void SelectAdding(Transform transform)
    {
        //去除原方块的线框
        if (trans != null)
        {
            Destroy(trans.gameObject.GetComponent("ShowBoxCollider"));
        }
        //保存选中的方块
        trans = transform;
        //为选中的方块画线
        trans.gameObject.AddComponent<ShowBoxCollider>();
    }

    //判断选中的方块是否为地面
    bool IsGround(Transform transform)
    {
        if (transform.gameObject.tag == "Ground")
        {
            return true;
        }
        return false;
    }

    //x轴旋转
    public void RotateX()
    {
        trans.rotation *= Quaternion.AngleAxis(90, Vector3.right);
    }

    //y轴旋转
    public void RotateY()
    {
        trans.rotation *= Quaternion.AngleAxis(90, Vector3.up);
    }

    //z轴旋转
    public void RotateZ()
    {
        trans.rotation *= Quaternion.AngleAxis(90, Vector3.forward);
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

        //判断按下delete键
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            //删除对应gameobject
            GameObject obj = trans.gameObject;
            if (!IsGround(trans))
            {
                Destroy(obj);
                //令trans指向null
                trans = null;
            }
        }
    }
}