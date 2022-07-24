using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMode : BuildMode
{
    //鼠标在按下左键时的位置
    Vector3 StartPos;
    //鼠标当前位置
    Vector3 CurrentPos;
    //记录碰撞是否有效
    bool flag;

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
        if (Scene.mode != Scene.Mode.line || Scene.TestUI() || Scene.SelectingAxis)
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
                    build((int)pos.x, (int)pos.y, (int)pos.z);
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
