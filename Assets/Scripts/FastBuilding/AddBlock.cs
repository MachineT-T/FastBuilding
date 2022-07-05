using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddBlock : MonoBehaviour
{
    //保存选择的材质
    public Material mat;
    //保存按下右键的时间
    float StartTime;
    //保存抬起右键的时间
    float EndTime;

    //放下方块
    void PutDownBlock()
    {
        //创建新方块
        GameObject obj = new GameObject();
        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //设置方块材质
        obj.GetComponent<Renderer>().material = mat;

        //射线检测
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            float TransformX = hit.transform.position.x, TransformY = hit.transform.position.y, TransformZ = hit.transform.position.z;
            float PointX = hit.point.x, PointY = hit.point.y, PointZ = hit.point.z;
            //判断碰撞点在碰撞物体的哪个方向并设置新方块的位置
            Vector3 NewPos = hit.transform.position;
            if (PointX == TransformX + 0.5)
            {
                NewPos.x++;
            }
            else if (PointX == TransformX - 0.5)
            {
                NewPos.x--;
            }
            else if (PointY == TransformY + 0.5)
            {
                NewPos.y++;
            }
            else if (PointY == TransformY - 0.5)
            {
                NewPos.y--;
            }
            else if (PointZ == TransformZ + 0.5)
            {
                NewPos.z++;
            }
            else if (PointZ == TransformZ - 0.5)
            {
                NewPos.z--;
            }
            obj.transform.position = NewPos;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //判断右键按下
        if (Input.GetMouseButtonDown(1))
        {
            StartTime = Time.time;//记录按下右键时间
        }

        //判断右键抬起
        if (Input.GetMouseButtonUp(1))
        {
            EndTime = Time.time;//记录右键抬起的时间
            //如果按下右键时间间隔小于0.2秒则判定为点击
            if (EndTime - StartTime < 0.2f)
            {
                //放下方块
                PutDownBlock();
            }
        }
    }
}
