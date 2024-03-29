using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMode : BuildMode
{
    //鼠标在屏幕上移动距离和球形半径的换算比率
    const float RadiusRatio = 0.05f;
    //鼠标在屏幕上移动距离和圆柱体高度的换算比率
    const float HeightRatio = 0.05f;
    //球形的半径
    float radius;
    //圆柱体的高
    float height;
    //鼠标在按下左键时的位置
    Vector3 StartPos;
    //鼠标当前位置
    Vector3 CurrentPos;
    //记录边界关键点
    Vector3 KeyPoint;
    //记录底面关键点
    Vector3 TempPoint;
    //记录碰撞信息
    RaycastHit hit;
    //记录碰撞是否有效
    bool flag;
    /*记录当前的状态
    分为两个状态
    状态0：按下左键选定圆柱底面圆心，拖动控制底面半径，松开确定并进入状态1
    状态1：通过移动鼠标控制圆柱高度，点击左键确定高度*/
    bool state = false;
    //记录当前渲染了的层数
    int NowHeight;

    //生成或移除一层
    void BuildOrRemoveALayer(bool BuildOrRemove)
    {
        /*遍历一个包围底面圆形的正方形范围的所有方块
        对于每个范围内的方块，代入圆的方程判断是否符合
        若符合圆的方程则将该方块位置作为底面圆的组成方块之一*/
        int x1 = 0, x2 = 0, y1 = 0, y2 = 0, z1 = 0, z2 = 0;
        if (hit.normal.x == 1.0f || hit.normal.x == -1.0f)
        {
            x1 = (int)(KeyPoint.x);
            x2 = (int)(KeyPoint.x);
            y1 = (int)(KeyPoint.y - Mathf.Ceil(radius));
            y2 = (int)(KeyPoint.y + Mathf.Ceil(radius));
            z1 = (int)(KeyPoint.z - Mathf.Ceil(radius));
            z2 = (int)(KeyPoint.z + Mathf.Ceil(radius));
        }
        else if (hit.normal.y == 1.0f || hit.normal.y == -1.0f)
        {
            x1 = (int)(KeyPoint.x - Mathf.Ceil(radius));
            x2 = (int)(KeyPoint.x + Mathf.Ceil(radius));
            y1 = (int)(KeyPoint.y);
            y2 = (int)(KeyPoint.y);
            z1 = (int)(KeyPoint.z - Mathf.Ceil(radius));
            z2 = (int)(KeyPoint.z + Mathf.Ceil(radius));
        }
        else if (hit.normal.z == 1.0f || hit.normal.z == -1.0f)
        {
            x1 = (int)(KeyPoint.x - Mathf.Ceil(radius));
            x2 = (int)(KeyPoint.x + Mathf.Ceil(radius));
            y1 = (int)(KeyPoint.y - Mathf.Ceil(radius));
            y2 = (int)(KeyPoint.y + Mathf.Ceil(radius));
            z1 = (int)(KeyPoint.z);
            z2 = (int)(KeyPoint.z);
        }

        //求球心
        Vector3 o = KeyPoint;
        float x0 = o.x, y0 = o.y, z0 = o.z;

        //遍历正方体范围内的所有方块
        for (int x = x1; x <= x2; x++)
        {
            for (int y = y1; y <= y2; y++)
            {
                for (int z = z1; z <= z2; z++)
                {
                    if (x * x - 2 * x0 * x + x0 * x0 + y * y - 2 * y0 * y + y0 * y0 + z * z - 2 * z0 * z + z0 * z0 - radius * radius <= 0)
                    {
                        if (BuildOrRemove)
                        {
                            build(x, y, z);
                        }
                        else
                        {
                            remove(x, y, z);
                        }
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //如果不是球形模式或者鼠标在UI按钮上，则直接返回
        if (Scene.mode != Scene.Mode.cylinder || Scene.TestUI() || Scene.SelectingAxis)
        {
            return;
        }

        switch (state)
        {
            //状态0
            case false:
                //左键按下时
                if (Input.GetMouseButtonDown(0))
                {
                    //记录按下左键时的鼠标位置
                    StartPos = Input.mousePosition;
                    //记录碰撞点信息
                    flag = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
                    if (flag)
                    {
                        //更换选择的方块时需要先确定选中方块的移动
                        MoveMode.ConfirmMoving();
                        //清空选择列表
                        SelectBlock.ClearSelected();
                        KeyPoint = GetPos(hit);
                    }
                }

                //如果碰撞无效则不需要执行后面的操作直接返回
                if (!flag)
                {
                    return;
                }

                //左键按住时
                if (Input.GetMouseButton(0))
                {
                    //记录当前鼠标位置
                    CurrentPos = Input.mousePosition;
                    //计算球体半径
                    radius = Vector3.Distance(CurrentPos, StartPos) * RadiusRatio;

                    //每帧都先删除原本渲染的方块并重新渲染
                    SelectBlock.DeleteSelected();

                    BuildOrRemoveALayer(true);
                }

                //松开左键后确定圆柱体底面半径，转换状态
                if (Input.GetMouseButtonUp(0))
                {
                    //保存当前鼠标位置用于状态1
                    StartPos = Input.mousePosition;
                    //记录底面中点
                    TempPoint = KeyPoint;
                    //转换状态
                    state = true;
                    //记录当前渲染高度为0
                    NowHeight = 0;
                }
                break;


            //状态1
            case true:
                //记录当前鼠标位置
                CurrentPos = Input.mousePosition;
                //计算圆柱体高度
                height = Vector3.Distance(CurrentPos, StartPos) * HeightRatio;

                //每帧都先删除原本渲染的方块并重新渲染
                //SelectBlock.DeleteSelected();

                //将KeyPoint遍历每一层中点
                KeyPoint = TempPoint;
                for (int i = 0; i <= Mathf.Max(height, NowHeight); i++)
                {
                    //如果i<=height则搭建一层，否则移除一层
                    BuildOrRemoveALayer((bool)(i <= height));
                    KeyPoint += hit.normal;
                }
                //设置当前渲染层数
                NowHeight = (int)height;

                //点击左键后确定圆柱体，转换回状态0
                if (Input.GetMouseButtonUp(0))
                {
                    finish();
                    state = false;
                }
                break;
        }
    }
}
