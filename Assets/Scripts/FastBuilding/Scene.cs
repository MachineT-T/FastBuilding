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
    public enum Mode { rectangle, sphere, line, cylinder, pyramid, select, AddSelect, SubSelect };

    //当前模式
    public static Mode mode = Mode.line;

    //鼠标是否在UI中
    static bool IsInUI = false;

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

    //判断鼠标是否在UI按钮内
    public static bool TestUI()
    {
        return IsInUI;
    }

    //转换为矩形搭建模式
    public static void ToRectangleMode()
    {
        mode = Mode.rectangle;
    }

    //转换为球形搭建模式
    public static void ToSphereMode()
    {
        mode = Mode.sphere;
    }

    //转换为线性搭建模式
    public static void ToLineMode()
    {
        mode = Mode.line;
    }

    //转换为圆柱搭建模式
    public static void ToCylinderMode()
    {
        mode = Mode.cylinder;
    }

    //转换为金字塔搭建模式
    public static void ToPyramidMode()
    {
        mode = Mode.pyramid;
    }

    //转换为常规选择模式
    public static void ToSelectMode()
    {
        mode = Mode.select;
    }

    //转换为加选模式
    public static void ToAddSelectMode()
    {
        mode = Mode.AddSelect;
    }

    //转换为减选模式
    public static void ToSubSelectMode()
    {
        mode = Mode.SubSelect;
    }

    //判断方块是否在搭建范围内
    public static bool TestPos(int x, int y, int z)
    {
        if (0 <= x && x < length && 0 <= y && y < height && 0 <= z && z < wide)
        {
            return true;
        }
        return false;
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
