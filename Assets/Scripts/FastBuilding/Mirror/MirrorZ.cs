using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorZ : MonoBehaviour
{
    /*
        所有镜像模式的脚本必须在搭建模式的脚本后运行
        否则会出bug
    */

    //判断是否为Z轴镜像模式
    public static bool MirrorZMode = false;
    //判断是否为选定对称轴的状态
    static bool SelectZ = false;
    //记录对称面
    static float Z;
    //场景长高
    static float length, height;
    //地面GameObject
    static GameObject[,] front, back;
    //对称面材质
    public Material mat;

    //点击Z轴镜像按钮
    public void ClickMirrorZ()
    {
        //如果处于Z轴镜像模式则取消Z轴镜像模式
        if (MirrorZMode)
        {
            OffMIrrorZ();
        }
        else if (SelectZ)//如果处于选定对称轴的状态则取消该状态
        {
            SelectZ = false;
            Scene.SelectingAxis = false;
        }
        else//否则进入选定对称轴的状态
        {
            SelectZ = true;
            Scene.SelectingAxis = true;
        }
    }

    //清空对称面
    static void ClearMirrorZ()
    {
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Destroy(front[i, j]);
                Destroy(back[i, j]);
            }
        }
    }

    //根据获得的坐标返回镜像位置的z
    public static int GetPosZ(int z)
    {
        return (int)(Z * 2 - z);
    }

    //关闭Z轴镜像模式
    public static void OffMIrrorZ()
    {
        MirrorZMode = false;
        SelectZ = false;
        Scene.SelectingAxis = false;
        ClearMirrorZ();
    }

    // Start is called before the first frame update
    void Start()
    {
        length = Scene.length;
        height = Scene.height;
    }

    // Update is called once per frame
    void Update()
    {
        //如果不是选择对称轴的状态则不处理
        if (!SelectZ || Scene.TestUI())
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
                Z = (int)hit.transform.position.z + 0.5f;

                //取消选择状态并开启Z轴镜像模式
                SelectZ = false;
                Scene.SelectingAxis = false;
                MirrorZMode = true;

                //创建对称轴显示平面
                front = new GameObject[(int)length, (int)height];
                for (int i = 0; i < length; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        //将GameObject转换为Plane
                        front[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        //设置Plane的大小
                        front[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        //设置Plane的位置
                        front[i, j].transform.position = new Vector3(i, j, Z);
                        //设置Plane的旋转角度
                        front[i, j].transform.rotation = Quaternion.Euler(-90, 0, 0);
                        //设置无法被碰撞检测
                        front[i, j].layer = 2;
                        //设置地面材质
                        front[i, j].GetComponent<Renderer>().material = mat;
                    }
                }
                back = new GameObject[(int)length, (int)height];
                for (int i = 0; i < length; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        //将GameObject转换为Plane
                        back[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        //设置Plane的大小
                        back[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        //设置Plane的位置
                        back[i, j].transform.position = new Vector3(i, j, Z);
                        //设置Plane的旋转角度
                        back[i, j].transform.rotation = Quaternion.Euler(90, 0, 0);
                        //设置无法被碰撞检测
                        back[i, j].layer = 2;
                        //设置地面材质
                        back[i, j].GetComponent<Renderer>().material = mat;
                    }
                }
            }
        }
    }
}
