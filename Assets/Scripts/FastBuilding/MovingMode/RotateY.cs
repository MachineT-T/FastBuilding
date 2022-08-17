using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateY : Rotate
{
    //点击Y轴旋转按钮
    public void ClickRotateY()
    {
        //计算选中方块的中心
        getCenter();

        //获取选中方块的引用
        ArrayList selected = SelectBlock.getSelected();
        //获取场景方块信息的引用
        GameObject[,,] blocks = Scene.getBlocks();

        //旋转物体
        for (int i = 0; i < selected.Count; i++)
        {
            //移动前先判断前一帧的物体位置原本是否有方块，有则恢复显示
            Vector3 temp = ((GameObject)selected[i]).transform.position;
            if (Scene.TestBlocks((int)temp.x, (int)temp.y, (int)temp.z))
            {
                blocks[(int)temp.x, (int)temp.y, (int)temp.z].SetActive(true);
            }

            //计算物体当前位置
            Vector3 CurrentPos = temp;
            CurrentPos.x = temp.z + a - c;
            CurrentPos.z = -temp.x + a + c;
            //修正超出范围的方块
            CurrentPos = CorrectPos(CurrentPos);

            //判断当前物体位置是否已有方块
            temp = CurrentPos;
            if (Scene.TestBlocks((int)temp.x, (int)temp.y, (int)temp.z))
            {
                //判断已有的方块是否是被选择的方块
                if (!selected.Contains(blocks[(int)temp.x, (int)temp.y, (int)temp.z]))
                {
                    //隐藏符合上述两个条件的方块
                    blocks[(int)temp.x, (int)temp.y, (int)temp.z].SetActive(false);
                }
            }

            //将选中方块移动到对应位置上
            ((GameObject)selected[i]).transform.position = temp;
        }
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
