using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectBlock : MonoBehaviour
{
    //用于保存选择的方块
    static ArrayList selected = new ArrayList();

    public static ArrayList getSelected()
    {
        return selected;
    }

    //清空选择的方块列表
    public static void ClearSelected()
    {
        for (int i = 0; i < selected.Count; ++i)
        {
            GameObject obj = (GameObject)selected[i];
            Destroy(obj.GetComponent("ShowBoxCollider"));
        }
        selected.Clear();
    }

    //删除选中的方块
    public static void DeleteSelected()
    {
        //删除对应gameobject
        while (selected.Count != 0)
        {
            //获取选择列表中gameobject
            GameObject obj = (GameObject)selected[0];
            //从列表中删除该项
            selected.RemoveAt(0);
            //设置该位置不存在方块
            Scene.setBlocks((int)obj.transform.position.x, (int)obj.transform.position.y, (int)obj.transform.position.z, false);
            //销毁此gameobject
            Destroy(obj);
        }
    }

    //全选
    public static void SelectAll()
    {
        //获取场景中的方块信息
        GameObject[,,] blocks = Scene.getBlocks();
        //更换选择的方块时需要先确定选中方块的移动
        MoveMode.ConfirmMoving();
        //先清空选择列表
        ClearSelected();
        //将场景中所有方块添加进选择列表中
        for (int i = 0; i < Scene.length; i++)
        {
            for (int j = 0; j < Scene.height; j++)
            {
                for (int k = 0; k < Scene.wide; k++)
                {
                    if (Scene.TestBlocks(i, j, k))
                    {
                        //将方块添加进选择列表中
                        selected.Add(blocks[i, j, k]);
                        //为选中的方块画线
                        blocks[i, j, k].AddComponent<ShowBoxCollider>();
                    }
                }
            }
        }
        //记录选中方块的初始位置
        MoveMode.RecordBlockInitPos();
    }

    //选择当前选中材质的所有方块
    public static void SelectSameMaterial()
    {
        //获取场景中的方块信息
        GameObject[,,] blocks = Scene.getBlocks();
        //更换选择的方块时需要先确定选中方块的移动
        MoveMode.ConfirmMoving();
        //先清空选择列表
        ClearSelected();
        //遍历场景中所有方块
        for (int i = 0; i < Scene.length; i++)
        {
            for (int j = 0; j < Scene.height; j++)
            {
                for (int k = 0; k < Scene.wide; k++)
                {
                    if (Scene.TestBlocks(i, j, k))
                    {
                        //如果该位置方块材质与选中的材质相同
                        if (blocks[i, j, k].GetComponent<MaterialPath>().MatPath == AssetDatabase.GetAssetPath(BuildMode.mat))
                        {
                            //将方块添加进选择列表中
                            selected.Add(blocks[i, j, k]);
                            //为选中的方块画线
                            blocks[i, j, k].AddComponent<ShowBoxCollider>();
                        }
                    }
                }
            }
        }
        //记录选中方块的初始位置
        MoveMode.RecordBlockInitPos();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //判断按下delete键
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSelected();
        }
    }
}