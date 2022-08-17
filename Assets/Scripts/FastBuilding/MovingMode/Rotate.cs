using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    //选中方块群的中心
    protected int a, b, c;

    //计算选中方块的中心
    protected void getCenter()
    {
        const int inf = 1000000;

        //记录方块位置坐标的最大最小值
        int MinX = inf, MinY = inf, MinZ = inf;
        int MaxX = 0, MaxY = 0, MaxZ = 0;

        //获取选中方块列表的引用
        ArrayList selected = SelectBlock.getSelected();

        //遍历选中方块
        for (int i = 0; i < selected.Count; i++)
        {
            //获取方块位置坐标的最大最小值
            MinX = Mathf.Min(MinX, (int)((GameObject)selected[i]).transform.position.x);
            MaxX = Mathf.Max(MaxX, (int)((GameObject)selected[i]).transform.position.x);

            MinY = Mathf.Min(MinY, (int)((GameObject)selected[i]).transform.position.y);
            MaxY = Mathf.Max(MaxY, (int)((GameObject)selected[i]).transform.position.y);

            MinZ = Mathf.Min(MinZ, (int)((GameObject)selected[i]).transform.position.z);
            MaxZ = Mathf.Max(MaxZ, (int)((GameObject)selected[i]).transform.position.z);
        }

        //计算选中方块的中心
        a = (MinX + MaxX) / 2;
        b = (MinY + MaxY) / 2;
        c = (MinZ + MaxZ) / 2;
    }

    //将超出搭建范围的位置修改回搭建范围内
    protected Vector3 CorrectPos(Vector3 pos)
    {
        int length = Scene.length, wide = Scene.wide, height = Scene.height;

        pos.x = pos.x % length;
        if (pos.x < 0)
        {
            pos.x += length;
        }

        pos.y = pos.y % height;
        if (pos.y < 0)
        {
            pos.y += height;
        }

        pos.z = pos.z % wide;
        if (pos.z < 0)
        {
            pos.z += wide;
        }

        return pos;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
