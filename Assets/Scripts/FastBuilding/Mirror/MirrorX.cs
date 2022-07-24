using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorX : MonoBehaviour
{
    /*
        所有镜像模式的脚本必须在搭建模式的脚本后运行
        否则会出bug
    */

    //判断是否为X轴镜像模式
    public static bool MirrorXMode = false;
    //判断是否为选定对称轴的状态
    static bool SelectX = false;
    //记录对称面
    static float X;
    //场景宽高
    static float height, wide;
    //地面GameObject
    static GameObject[,] left, right;
    //对称面材质
    public Material mat;

    //点击X轴镜像按钮
    public void ClickMirrorX()
    {
        //如果处于X轴镜像模式则取消X轴镜像模式
        if (MirrorXMode)
        {
            OffMIrrorX();
        }
        else if (SelectX)//如果处于选定对称轴的状态则取消该状态
        {
            SelectX = false;
            Scene.SelectingAxis = false;
        }
        else//否则进入选定对称轴的状态
        {
            SelectX = true;
            Scene.SelectingAxis = true;
        }
    }

    //清空对称面
    static void ClearMirrorX()
    {
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                Destroy(left[i, j]);
                Destroy(right[i, j]);
            }
        }
    }

    //根据获得的坐标返回镜像位置的x
    public static int GetPosX(int x)
    {
        return (int)(X * 2 - x);
    }

    //关闭X轴镜像模式
    public static void OffMIrrorX()
    {
        MirrorXMode = false;
        SelectX = false;
        Scene.SelectingAxis = false;
        ClearMirrorX();
    }

    // Start is called before the first frame update
    void Start()
    {
        height = Scene.height;
        wide = Scene.wide;
    }

    // Update is called once per frame
    void Update()
    {
        //如果不是选择对称轴的状态则不处理
        if (!SelectX || Scene.TestUI())
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
                X = (int)hit.transform.position.x + 0.5f;

                //取消选择状态并开启X轴镜像模式
                SelectX = false;
                Scene.SelectingAxis = false;
                MirrorXMode = true;

                //创建对称轴显示平面
                left = new GameObject[(int)height, (int)wide];
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < wide; ++j)
                    {
                        //将GameObject转换为Plane
                        left[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        //设置Plane的大小
                        left[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        //设置Plane的位置
                        left[i, j].transform.position = new Vector3(X, i, j);
                        //设置Plane的旋转角度
                        left[i, j].transform.rotation = Quaternion.Euler(0, 0, -90);
                        //设置无法被碰撞检测
                        left[i, j].layer = 2;
                        //设置地面材质
                        left[i, j].GetComponent<Renderer>().material = mat;
                    }
                }
                right = new GameObject[(int)height, (int)wide];
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < wide; ++j)
                    {
                        //将GameObject转换为Plane
                        right[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        //设置Plane的大小
                        right[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        //设置Plane的位置
                        right[i, j].transform.position = new Vector3(X, i, j);
                        //设置Plane的旋转角度
                        right[i, j].transform.rotation = Quaternion.Euler(0, 0, 90);
                        //设置无法被碰撞检测
                        right[i, j].layer = 2;
                        //设置地面材质
                        right[i, j].GetComponent<Renderer>().material = mat;
                    }
                }
            }
        }
    }
}
