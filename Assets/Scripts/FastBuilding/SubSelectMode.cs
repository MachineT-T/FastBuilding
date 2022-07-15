using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSelectMode : MonoBehaviour
{
    //记录StartHit是否有效
    bool IsStartHit;
    //记录EndHit是否有效
    bool IsEndHit;

    //保存左键按下时的射线检测信息
    RaycastHit StartHit;
    //保存左线松开时的射线检测信息
    RaycastHit EndHit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //判断是否为减选模式并且鼠标不在UI按钮上
        if (Scene.mode != Scene.Mode.SubSelect || Scene.TestUI())
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

            //判断两次射线检测信息是否有效
            if (IsStartHit && IsEndHit)
            {
                Vector3 StartPos = StartHit.transform.position, EndPos = EndHit.transform.position;
                //获取范围的坐标
                int x1 = (int)Mathf.Min(StartPos.x, EndPos.x);
                int x2 = (int)Mathf.Max(StartPos.x, EndPos.x);
                int y1 = (int)Mathf.Min(StartPos.y, EndPos.y);
                int y2 = (int)Mathf.Max(StartPos.y, EndPos.y);
                int z1 = (int)Mathf.Min(StartPos.z, EndPos.z);
                int z2 = (int)Mathf.Max(StartPos.z, EndPos.z);

                //为减选的方块重新画线
                TempSelect.DrawSelected();
                //获取选中的方块列表的引用
                ArrayList selected = SelectBlock.getSelected();
                ArrayList temp = TempSelect.getSelected();
                //获取场景中的方块信息
                GameObject[,,] blocks = Scene.getBlocks();

                for (int i = x1; i <= x2; ++i)
                {
                    for (int j = y1; j <= y2; ++j)
                    {
                        for (int k = z1; k <= z2; ++k)
                        {
                            //判断在该位置是否有方块
                            if (Scene.TestBlocks(i, j, k))
                            {
                                //判断方块是否已在选择列表中
                                if (selected.Contains(blocks[i, j, k]) && !temp.Contains(blocks[i, j, k]))
                                {
                                    //将方块添加进选择列表中
                                    temp.Add(blocks[i, j, k]);
                                    //为减选的方块去掉画线
                                    Destroy(blocks[i, j, k].GetComponent("ShowBoxCollider"));
                                }
                            }
                        }
                    }
                }

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //使用完射线检测信息后将射线检测信息无效化
            IsStartHit = false;
            IsEndHit = false;
            //获取选中的方块列表的引用
            ArrayList selected = SelectBlock.getSelected();
            ArrayList temp = TempSelect.getSelected();
            //将暂时选择列表中的所有方块从选择列表中去掉
            for (int i = 0; i < temp.Count; ++i)
            {
                selected.Remove(temp[i]);
            }
            //清空暂时选择列表
            temp.Clear();
        }
    }
}
