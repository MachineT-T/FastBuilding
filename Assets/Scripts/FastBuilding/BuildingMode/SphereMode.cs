using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMode : BuildMode
{
    //鼠标在屏幕上移动距离和球形半径的换算比率
    const float ratio = 0.01f;
    //球形的半径
    float radius;
    //鼠标在按下左键时的位置
    Vector3 StartPos;
    //鼠标当前位置
    Vector3 CurrentPos;
    //记录边界关键点
    Vector3 KeyPoint;
    //记录碰撞信息
    RaycastHit hit;
    //记录碰撞是否有效
    bool flag;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //如果不是球形模式或者鼠标在UI按钮上，则直接返回
        if (Scene.mode != Scene.Mode.sphere || Scene.TestUI() || Scene.SelectingAxis)
        {
            return;
        }

        //左键按下时
        if (Input.GetMouseButtonDown(0))
        {
            //记录按下左键时的鼠标位置
            StartPos = Input.mousePosition;
            //记录碰撞点信息
            flag = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (flag)
            {
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
            radius = Vector3.Distance(CurrentPos, StartPos) * ratio;

            /*遍历一个包围球体的正方体范围的所有方块
            对于每个范围内的方块，代入球体方程判断是否符合
            若符合球体方程则将该方块位置作为球体的组成方块之一*/
            /*计算遍历的正方体坐标范围
            KeyPoint位置看做正方体的底面中点
            正方体坐标范围的起始点和终点一定为底面或顶面的四个顶点之一
            因此只要用底面四个顶点判断一下
            再利用法向量计算出顶面中点判断顶面四个顶点
            即可求出正方体坐标范围*/
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0, z1 = 0, z2 = 0;
            if (hit.normal.x == 1.0f)
            {
                x1 = (int)(KeyPoint.x);
                x2 = (int)(KeyPoint.x + Mathf.Ceil(radius) * 2);
                y1 = (int)(KeyPoint.y - Mathf.Ceil(radius));
                y2 = (int)(KeyPoint.y + Mathf.Ceil(radius));
                z1 = (int)(KeyPoint.z - Mathf.Ceil(radius));
                z2 = (int)(KeyPoint.z + Mathf.Ceil(radius));
            }
            else if (hit.normal.x == -1.0f)
            {
                x1 = (int)(KeyPoint.x - Mathf.Ceil(radius) * 2);
                x2 = (int)(KeyPoint.x);
                y1 = (int)(KeyPoint.y - Mathf.Ceil(radius));
                y2 = (int)(KeyPoint.y + Mathf.Ceil(radius));
                z1 = (int)(KeyPoint.z - Mathf.Ceil(radius));
                z2 = (int)(KeyPoint.z + Mathf.Ceil(radius));
            }
            else if (hit.normal.y == 1.0f)
            {
                x1 = (int)(KeyPoint.x - Mathf.Ceil(radius));
                x2 = (int)(KeyPoint.x + Mathf.Ceil(radius));
                y1 = (int)(KeyPoint.y);
                y2 = (int)(KeyPoint.y + Mathf.Ceil(radius) * 2);
                z1 = (int)(KeyPoint.z - Mathf.Ceil(radius));
                z2 = (int)(KeyPoint.z + Mathf.Ceil(radius));
            }
            else if (hit.normal.y == -1.0f)
            {
                x1 = (int)(KeyPoint.x - Mathf.Ceil(radius));
                x2 = (int)(KeyPoint.x + Mathf.Ceil(radius));
                y1 = (int)(KeyPoint.y - Mathf.Ceil(radius) * 2);
                y2 = (int)(KeyPoint.y);
                z1 = (int)(KeyPoint.z - Mathf.Ceil(radius));
                z2 = (int)(KeyPoint.z + Mathf.Ceil(radius));
            }
            else if (hit.normal.z == 1.0f)
            {
                x1 = (int)(KeyPoint.x - Mathf.Ceil(radius));
                x2 = (int)(KeyPoint.x + Mathf.Ceil(radius));
                y1 = (int)(KeyPoint.y - Mathf.Ceil(radius));
                y2 = (int)(KeyPoint.y + Mathf.Ceil(radius));
                z1 = (int)(KeyPoint.z);
                z2 = (int)(KeyPoint.z + Mathf.Ceil(radius) * 2);
            }
            else if (hit.normal.z == -1.0f)
            {
                x1 = (int)(KeyPoint.x - Mathf.Ceil(radius));
                x2 = (int)(KeyPoint.x + Mathf.Ceil(radius));
                y1 = (int)(KeyPoint.y - Mathf.Ceil(radius));
                y2 = (int)(KeyPoint.y + Mathf.Ceil(radius));
                z1 = (int)(KeyPoint.z - Mathf.Ceil(radius) * 2);
                z2 = (int)(KeyPoint.z);
            }
            //求球心
            Vector3 o = KeyPoint + radius * hit.normal;
            float x0 = o.x, y0 = o.y, z0 = o.z;

            //每帧都先删除原本渲染的方块并重新渲染
            SelectBlock.DeleteSelected();

            //遍历正方体范围内的所有方块
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    for (int z = z1; z <= z2; z++)
                    {
                        if (x * x - 2 * x0 * x + x0 * x0 + y * y - 2 * y0 * y + y0 * y0 + z * z - 2 * z0 * z + z0 * z0 - radius * radius <= 0)
                        {
                            build(x, y, z);
                        }
                    }
                }
            }
        }

        //松开左键后确定选中的方块,重新设置方块为可被射线检测
        if (Input.GetMouseButtonUp(0))
        {
            finish();
        }
    }
}
