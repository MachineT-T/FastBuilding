using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[SerializeField]
public class SaveData
{
    ///保存方块的材质路径
    public string[,,] BlocksMatPath;
    //保存位置上是否有方块的信息
    public bool[,,] HavingBlocks;
    //保存场景的大小
    public int length, height, wide;
    public SaveData(GameObject[,,] SaveBlocks, bool[,,] SaveHavingBlocks)
    {
        length = Scene.length;
        height = Scene.height;
        wide = Scene.wide;
        HavingBlocks = SaveHavingBlocks;
        BlocksMatPath = new string[length, height, wide];
        //将方块的材质路径信息提取出来
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < wide; k++)
                {
                    if (SaveHavingBlocks[i, j, k])
                    {
                        BlocksMatPath[i, j, k] = SaveBlocks[i, j, k].GetComponent<MaterialPath>().MatPath;
                    }
                }
            }
        }
    }

    public SaveData()
    {

    }
}
