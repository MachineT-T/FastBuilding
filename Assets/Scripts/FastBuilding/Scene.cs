using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    //整个场景的长宽高
    public static int length = 50, wide = 50, height = 50;//地面的长宽高
    //用于管理整个场景每个位置的数组
    static GameObject[,,] blocks;
    //保存对应位置上是否存在方块
    static bool[,,] HavingBlocks;

    //模式类型
    public enum Mode { build, select };

    //当前模式
    public static Mode mode = Mode.build;

    //鼠标是否在UI中
    static bool IsInUI;

    //获取场景方块信息的引用
    public static GameObject[,,] getBlocks()
    {
        return blocks;
    }

    //判断对应位置上是否存在方块
    public static bool TestBlocks(int x, int y, int z)
    {
        return HavingBlocks[x, y, z];
    }

    //设置对应位置上是否存在方块
    public static void setBlocks(int x, int y, int z, bool t)
    {
        HavingBlocks[x, y, z] = t;
    }

    //鼠标进入按钮
    public static void InUI()
    {
        IsInUI = true;
    }

    //鼠标离开按钮
    public static void OutUI()
    {
        IsInUI = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        HavingBlocks = new bool[length, height, wide];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 0; k < wide; ++k)
                {
                    HavingBlocks[i, j, k] = false;
                }
            }
        }
        blocks = new GameObject[length, height, wide];
    }


    // Update is called once per frame
    void Update()
    {

    }
}
