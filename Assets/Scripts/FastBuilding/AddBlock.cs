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
    //获取SelectBlock用于放下方块后选中刚放下的方块
    public GameObject SelectBlock;

    //放下方块
    void PutDownBlock(Vector3 pos)
    {
        //创建新方块
        GameObject obj;
        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //设置方块材质
        obj.GetComponent<Renderer>().material = mat;

        //设置方块位置
        obj.transform.position = pos;

        //将创建方块的信息传递给Scene
        Scene.setBlocks(obj);

        //选择方块
        SelectBlock.GetComponent<SelectBlock>().SelectAdding(obj.transform);
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
                //射线检测
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    float TransformX = hit.transform.position.x, TransformY = hit.transform.position.y, TransformZ = hit.transform.position.z;
                    float PointX = hit.point.x, PointY = hit.point.y, PointZ = hit.point.z;
                    Debug.Log("TransformX=" + TransformX + " TransformY=" + TransformY + " TransformZ=" + TransformZ + " PointX=" + PointX + " PointY=" + PointY + " PointZ=" + PointX);
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
                    else//碰撞点是plane
                    {
                        Debug.Log("hit the ground");
                        NewPos += hit.normal * 0.5f;
                    }
                    //放下方块
                    PutDownBlock(NewPos);
                }
            }
        }
    }
}
