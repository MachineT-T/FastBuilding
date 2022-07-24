using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorY : MonoBehaviour
{
    /*
        所有镜像模式的脚本必须在搭建模式的脚本后运行
        否则会出bug
    */

    //判断是否为Y轴镜像模式
    public static bool MirrorYMode = false;
    //判断是否为选定对称轴的状态
    static bool SelectY = false;
    //记录对称面
    static float Y;
    //场景长宽
    static float length, wide;
    //地面GameObject
    static GameObject[,] up, down;
    //对称面材质
    public Material mat;

    //点击Y轴镜像按钮
    public void ClickMirrorY()
    {
        //如果处于X轴镜像模式则取消Y轴镜像模式
        if (MirrorYMode)
        {
            OffMIrrorY();
        }
        else if (SelectY)//如果处于选定对称轴的状态则取消该状态
        {
            SelectY = false;
            Scene.SelectingAxis = false;
        }
        else//否则进入选定对称轴的状态
        {
            SelectY = true;
            Scene.SelectingAxis = true;
        }
    }

    //清空对称面
    static void ClearMirrorY()
    {
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                Destroy(up[i, j]);
                Destroy(down[i, j]);
            }
        }
    }

    //根据获得的坐标返回镜像位置的y
    public static int GetPosY(int y)
    {
        return (int)(Y * 2 - y);
    }

    //关闭Y轴镜像模式
    public static void OffMIrrorY()
    {
        MirrorYMode = false;
        SelectY = false;
        Scene.SelectingAxis = false;
        ClearMirrorY();
    }

    // Start is called before the first frame update
    void Start()
    {
        length = Scene.length;
        wide = Scene.wide;
    }

    // Update is called once per frame
    void Update()
    {
        //如果不是选择对称轴的状态则不处理
        if (!SelectY || Scene.TestUI())
        {
            return;
        }

        //如果松开左键则进行碰撞检测，使用碰撞方块确定对称轴
        if (Input.GetMouseButtonUp(0))
        {
            //射线检测
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //记录对称面
                Y = (int)hit.transform.position.y + 0.5f;

                //取消选择状态并开启Y轴镜像模式
                SelectY = false;
                Scene.SelectingAxis = false;
                MirrorYMode = true;

                //创建对称轴显示平面
                down = new GameObject[(int)length, (int)wide];
                for (int i = 0; i < length; ++i)
                {
                    for (int j = 0; j < wide; ++j)
                    {
                        //将GameObject转换为Plane
                        down[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        //设置Plane的大小
                        down[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        //设置Plane的位置
                        down[i, j].transform.position = new Vector3(i, Y, j);
                        //设置无法被碰撞检测
                        down[i, j].layer = 2;
                        //设置地面材质
                        down[i, j].GetComponent<Renderer>().material = mat;
                    }
                }
                up = new GameObject[(int)length, (int)wide];
                for (int i = 0; i < length; ++i)
                {
                    for (int j = 0; j < wide; ++j)
                    {
                        //将GameObject转换为Plane
                        up[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        //设置Plane的大小
                        up[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        //设置Plane的位置
                        up[i, j].transform.position = new Vector3(i, Y, j);
                        //设置Plane的旋转角度
                        up[i, j].transform.rotation = Quaternion.Euler(180, 0, 0);
                        //设置无法被碰撞检测
                        up[i, j].layer = 2;
                        //设置地面材质
                        up[i, j].GetComponent<Renderer>().material = mat;
                    }
                }
            }
        }
    }
}
