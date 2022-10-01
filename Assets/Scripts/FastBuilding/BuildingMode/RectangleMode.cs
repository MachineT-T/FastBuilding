using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RectangleMode : BuildMode
{
    //记录StartHit是否有效
    bool IsStartHit;
    //记录EndHit是否有效
    bool IsEndHit;

    //保存左键按下时的射线检测信息
    RaycastHit StartHit;
    //保存左线松开时的射线检测信息
    RaycastHit EndHit;

    //记录当前渲染出的立方体的范围
    int NowX1, NowY1, NowZ1, NowX2, NowY2, NowZ2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //判断是否为矩形模式并且鼠标不在UI按钮上
        if (Scene.mode != Scene.Mode.rectangle || Scene.TestUI() || Scene.SelectingAxis)
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
                //更换选择的方块时需要先确定选中方块的移动
                MoveMode.ConfirmMoving();
                //清空选择列表
                SelectBlock.ClearSelected();
                //初始化当前渲染范围
                Vector3 pos = GetPos(StartHit);
                NowX1 = NowX2 = (int)pos.x;
                NowY1 = NowY2 = (int)pos.y;
                NowZ1 = NowZ2 = (int)pos.z;
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


                //每帧都先删除原本渲染的方块并重新渲染
                // SelectBlock.DeleteSelected();

                for (int i = Mathf.Min(x1, NowX1); i <= Mathf.Max(x2, NowX2); ++i)
                {
                    for (int j = Mathf.Min(y1, NowY1); j <= Mathf.Max(y2, NowY2); ++j)
                    {
                        for (int k = Mathf.Min(z1, NowZ1); k <= Mathf.Max(z2, NowZ2); ++k)
                        {
                            //如果是当前碰撞检测范围则搭建否则取消搭建
                            if (x1 <= i && i <= x2 && y1 <= j && j <= y2 && z1 <= k && k <= z2)
                            {
                                build(i, j, k);
                            }
                            else
                            {
                                CancelBuild(i, j, k);
                            }
                        }
                    }
                }
                //修改当前渲染出的立方体范围
                NowX1 = x1;
                NowX2 = x2;
                NowY1 = y1;
                NowY2 = y2;
                NowZ1 = z1;
                NowZ2 = z2;
            }
        }

        //使用完射线检测信息后将射线检测信息无效化并将方块设置为可被射线检测
        if (Input.GetMouseButtonUp(0))
        {
            IsStartHit = false;
            IsEndHit = false;
            finish();
        }
    }
}

