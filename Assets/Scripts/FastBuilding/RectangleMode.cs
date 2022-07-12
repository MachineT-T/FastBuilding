using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleMode : MonoBehaviour
{
    //保存选择的材质
    public Material mat;
    //记录StartHit是否有效
    bool IsStartHit;
    //记录EndHit是否有效
    bool IsEndHit;

    //保存左键按下时的射线检测信息
    RaycastHit StartHit;
    //保存左线松开时的射线检测信息
    RaycastHit EndHit;

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
        //判断是否为搭建模式并且鼠标不在UI按钮上
        if (Scene.mode != Scene.Mode.rectangle || Scene.TestUI())
        {
            return;
        }

        //判断左键按下
        if (Input.GetMouseButtonDown(0))
        {
            //先标记两次射线检测无效使旧信息无效化
            IsStartHit = false;
            IsEndHit = false;
            //射线检测
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                StartHit = hit;
                //标记开始射线检测有效
                IsStartHit = true;
            }
        }

        //在按住左键拖动时记录最后射线检测的结果
        if (Input.GetMouseButton(0))
        {
            //射线检测
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                EndHit = hit;
                //标记结束射线检测有效
                IsEndHit = true;
            }
        }

        //左键松开时，如果两个射线检测结果都有效则将范围内未放置方块的位置放置对应方块并选中放置的所有方块
        if (Input.GetMouseButtonUp(0))
        {
            //判断两次射线检测信息是否有效
            if (IsStartHit && IsEndHit)
            {
                Vector3 StartPos = GetPos(StartHit), EndPos = GetPos(EndHit);
                //获取范围的坐标
                int x1 = (int)Mathf.Min(StartPos.x, EndPos.x);
                int x2 = (int)Mathf.Max(StartPos.x, EndPos.x);
                int y1 = (int)Mathf.Min(StartPos.y, EndPos.y);
                int y2 = (int)Mathf.Max(StartPos.y, EndPos.y);
                int z1 = (int)Mathf.Min(StartPos.z, EndPos.z);
                int z2 = (int)Mathf.Max(StartPos.z, EndPos.z);

                //清空选择的方块数组
                SelectBlock.ClearSelected();
                //获取选中的方块列表的引用
                ArrayList selected = SelectBlock.getSelected();
                //获取场景中的方块信息
                GameObject[,,] blocks = Scene.getBlocks();

                for (int i = x1; i <= x2; ++i)
                {
                    for (int j = y1; j <= y2; ++j)
                    {
                        for (int k = z1; k <= z2; ++k)
                        {
                            //如果该位置没有方块则将该位置加入选择区域并添加方块
                            if (!Scene.TestBlocks(i, j, k))
                            {
                                //创建方块对象
                                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                //设置方块材质
                                obj.GetComponent<Renderer>().material = mat;
                                //设置方块位置
                                obj.transform.position = new Vector3(i, j, k);
                                //将方块添加进方块信息中
                                blocks[i, j, k] = obj;
                                Scene.setBlocks(i, j, k, true);
                                //将方块添加进选择列表中
                                selected.Add(obj);
                                //为选中的方块画线
                                obj.AddComponent<ShowBoxCollider>();
                            }
                        }
                    }
                }
                //使用完射线检测信息后将射线检测信息无效化
                IsStartHit = false;
                IsEndHit = false;
            }
        }
    }
}

