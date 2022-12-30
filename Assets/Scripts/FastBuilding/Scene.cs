using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Scene : MonoBehaviour
{
    //整个场景的长宽高
    public static int length = 50, wide = 50, height = 50;//地面的长宽高
    //用于管理整个场景每个位置的数组
    static GameObject[,,] blocks;
    //保存对应位置上是否存在方块
    static bool[,,] HavingBlocks;

    //模式类型
    public enum Mode { rectangle, sphere, line, cylinder, pyramid, select, AddSelect, SubSelect, move };

    //判断是否在选定对称轴的状态
    public static bool SelectingAxis = false;

    //当前模式
    public static Mode mode = Mode.rectangle;

    //鼠标是否在UI中
    static bool IsInUI = false;

    //获取场景方块信息的引用
    public static GameObject[,,] getBlocks()
    {
        return blocks;
    }

    //获取导出信息
    public static SaveData getExportData()
    {
        SaveData ExportData = new SaveData(blocks, HavingBlocks);
        return ExportData;
    }

    //导入数据
    public static void ImportData(SaveData data)
    {
        //先删除场景当前数据
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < wide; k++)
                {
                    if (HavingBlocks[i, j, k])
                    {
                        Destroy(blocks[i, j, k]);
                    }
                }
            }
        }

        //复现导入的数据信息
        HavingBlocks = data.HavingBlocks;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < wide; k++)
                {
                    if (HavingBlocks[i, j, k])
                    {
                        //创建方块对象
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //设置方块材质
                        Material mat = AssetDatabase.LoadAssetAtPath<Material>(data.BlocksMatPath[i, j, k]);
                        obj.GetComponent<Renderer>().material = mat;
                        //设置方块位置
                        obj.transform.position = new Vector3(i, j, k);
                        //将方块添加进方块信息中
                        blocks[i, j, k] = obj;
                        //为选中的方块挂载记录材质路径的脚本
                        obj.AddComponent<MaterialPath>();
                        //在脚本中保存材质路径
                        obj.GetComponent<MaterialPath>().MatPath = data.BlocksMatPath[i, j, k];
                    }
                }
            }
        }
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

    //转换为移动模式
    public static void ToMoveMode()
    {
        mode = Mode.move;
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

    //修改长度
    public static void editLength(int newLength)
    {
        //创建新的方块数组
        GameObject[,,] newBlocks = new GameObject[newLength, height, wide];
        bool[,,] newHavingBlocks = new bool[newLength, height, wide];
        //如果新的长度比原长度大
        if (newLength > length)
        {
            //将原方块数组复制到新方块数组中
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newBlocks[i, j, k] = blocks[i, j, k];
                        newHavingBlocks[i, j, k] = HavingBlocks[i, j, k];
                    }
                }
            }
            //设置多出来的部分没有方块
            for (int i = length; i < newLength; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newHavingBlocks[i, j, k] = false;
                    }
                }
            }
        }
        //如果新的长度比原长度小
        else if (newLength < length)
        {
            //将原方块数组复制到新方块数组中
            for (int i = 0; i < newLength; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newBlocks[i, j, k] = blocks[i, j, k];
                        newHavingBlocks[i, j, k] = HavingBlocks[i, j, k];
                    }
                }
            }
            //销毁范围外的方块
            for (int i = newLength; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        Destroy(blocks[i, j, k]);
                    }
                }
            }
        }
        //如果长度不变
        else
        {
            return;
        }
        //用新数组取代旧数组
        blocks = newBlocks;
        HavingBlocks = newHavingBlocks;
        length = newLength;
    }

    //修改高度
    public static void editHeight(int newHeight)
    {
        //创建新的方块数组
        GameObject[,,] newBlocks = new GameObject[length, newHeight, wide];
        bool[,,] newHavingBlocks = new bool[length, newHeight, wide];
        //如果新的高度比原高度大
        if (newHeight > height)
        {
            //将原方块数组复制到新方块数组中
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newBlocks[i, j, k] = blocks[i, j, k];
                        newHavingBlocks[i, j, k] = HavingBlocks[i, j, k];
                    }
                }
            }
            //设置多出来的部分没有方块
            for (int i = 0; i < length; ++i)
            {
                for (int j = height; j < newHeight; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newHavingBlocks[i, j, k] = false;
                    }
                }
            }
        }
        //如果新的高度比原高度小
        else if (newHeight < height)
        {
            //将原方块数组复制到新方块数组中
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < newHeight; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newBlocks[i, j, k] = blocks[i, j, k];
                        newHavingBlocks[i, j, k] = HavingBlocks[i, j, k];
                    }
                }
            }
            //销毁范围外的方块
            for (int i = 0; i < length; ++i)
            {
                for (int j = newHeight; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        Destroy(blocks[i, j, k]);
                    }
                }
            }
        }
        //如果高度不变
        else
        {
            return;
        }
        //用新数组取代旧数组
        blocks = newBlocks;
        HavingBlocks = newHavingBlocks;
        height = newHeight;
    }

    //修改宽度
    public static void editWide(int newWide)
    {
        //创建新的方块数组
        GameObject[,,] newBlocks = new GameObject[length, height, newWide];
        bool[,,] newHavingBlocks = new bool[length, height, newWide];
        //如果新的宽度比原宽度大
        if (newWide > wide)
        {
            //将原方块数组复制到新方块数组中
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < wide; ++k)
                    {
                        newBlocks[i, j, k] = blocks[i, j, k];
                        newHavingBlocks[i, j, k] = HavingBlocks[i, j, k];
                    }
                }
            }
            //设置多出来的部分没有方块
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = wide; k < newWide; ++k)
                    {
                        newHavingBlocks[i, j, k] = false;
                    }
                }
            }
        }
        //如果新的宽度比原宽度小
        else if (newWide < wide)
        {
            //将原方块数组复制到新方块数组中
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = 0; k < newWide; ++k)
                    {
                        newBlocks[i, j, k] = blocks[i, j, k];
                        newHavingBlocks[i, j, k] = HavingBlocks[i, j, k];
                    }
                }
            }
            //销毁范围外的方块
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    for (int k = newWide; k < wide; ++k)
                    {
                        Destroy(blocks[i, j, k]);
                    }
                }
            }
        }
        //如果高度不变
        else
        {
            return;
        }
        //用新数组取代旧数组
        blocks = newBlocks;
        HavingBlocks = newHavingBlocks;
        wide = newWide;
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
