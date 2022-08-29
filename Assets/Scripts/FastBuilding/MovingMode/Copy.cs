using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copy : MonoBehaviour
{
    //点击复制按钮
    public void ClickCopy()
    {
        //获取选中方块列表的引用
        ArrayList selected = SelectBlock.getSelected();
        //创建暂时存放新方块的列表
        ArrayList temp = new ArrayList();

        //创建选中方块的复制体并放进暂存列表中
        for (int i = 0; i < selected.Count; ++i)
        {
            //创建方块对象
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //设置方块材质
            obj.GetComponent<Renderer>().material = ((GameObject)selected[i]).GetComponent<Renderer>().material;
            //设置方块位置
            obj.transform.position = ((GameObject)selected[i]).transform.position;
            //为选中的方块画线
            obj.AddComponent<ShowBoxCollider>();
            //隐藏原方块
            ((GameObject)selected[i]).SetActive(false);
            //将方块加入暂存列表中
            temp.Add(obj);
        }

        //确认原方块移动
        MoveMode.ConfirmMoving();
        //清除原选中列表
        SelectBlock.ClearSelected();
        //将暂存列表中的方块放入选中列表中
        for (int i = 0; i < temp.Count; ++i)
        {
            selected.Add(temp[i]);
        }
        //为移动模式保存选中方块的初始位置
        MoveMode.RecordBlockInitPos();

        //清除暂存列表
        temp.Clear();
        //转换为移动模式
        Scene.ToMoveMode();
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
