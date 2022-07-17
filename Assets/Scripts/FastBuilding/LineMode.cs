using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMode : MonoBehaviour
{
    //鼠标在按下左键时的位置
    Vector3 StartPos;
    //鼠标当前位置
    Vector3 CurrentPos;
    //记录碰撞是否有效
    bool flag;
    //保存选择的材质
    public Material mat;

    //通过碰撞信息获取选择的位置
    Vector3 GetPos(RaycastHit hit)
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
        else//碰撞点是plane
        {
            NewPos += hit.normal * 0.5f;
        }
        return NewPos;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*通过按下左键位置和鼠标当前位置计算出对应的直线方程
        直线x每变化1作一次射线检测
        碰撞到的位置放置方块*/

        //判断是否为线形模式并且鼠标不在UI按钮上
        if (Scene.mode != Scene.Mode.line || Scene.TestUI())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //保存线段起点屏幕位置
            StartPos = Input.mousePosition;
            //清空选择列表
            SelectBlock.ClearSelected();
        }

        if (Input.GetMouseButton(0))
        {
            //保存线段终点屏幕位置
            CurrentPos = Input.mousePosition;

            //通过线段起点终点坐标计算直线方程
            float k = (CurrentPos.y - StartPos.y) / (CurrentPos.x - StartPos.x), b = StartPos.y - k * StartPos.x;

            //每帧都先删除原本渲染的方块并重新渲染
            SelectBlock.DeleteSelected();
            //获取选中的方块列表的引用
            ArrayList selected = SelectBlock.getSelected();
            //获取场景中的方块信息
            GameObject[,,] blocks = Scene.getBlocks();

            //遍历直线上的点
            for (float x = Mathf.Min(StartPos.x, CurrentPos.x); x <= Mathf.Max(StartPos.x, CurrentPos.x); x++)
            {
                float y = k * x + b;
                Vector3 ray = new Vector3(x, y, 0);
                //射线检测
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(ray), out hit))
                {
                    //获取碰撞方块位置
                    Vector3 pos = GetPos(hit);
                    //判断是否在搭建范围内
                    if (Scene.TestPos((int)pos.x, (int)pos.y, (int)pos.z))
                    {
                        //创建方块对象
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //设置方块材质
                        obj.GetComponent<Renderer>().material = mat;
                        //设置方块位置
                        obj.transform.position = pos;
                        //将方块添加进方块信息中
                        blocks[(int)pos.x, (int)pos.y, (int)pos.z] = obj;
                        Scene.setBlocks((int)pos.x, (int)pos.y, (int)pos.z, true);
                        //将方块添加进选择列表中
                        selected.Add(obj);
                        //为选中的方块画线
                        obj.AddComponent<ShowBoxCollider>();
                        //将方块设置为不能被射线检测
                        obj.layer = 2;
                    }
                }
            }
        }

        //松开左键后确定选中的方块,重新设置方块为可被射线检测
        if (Input.GetMouseButtonUp(0))
        {
            //获取选中的方块列表的引用
            ArrayList selected = SelectBlock.getSelected();
            for (int i = 0; i < selected.Count; ++i)
            {
                GameObject obj = (GameObject)selected[i];
                obj.layer = 0;
            }
        }
    }
}
