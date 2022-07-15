using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSelect : MonoBehaviour
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

    //为选择的方块重新画线
    public static void DrawSelected()
    {
        for (int i = 0; i < selected.Count; ++i)
        {
            GameObject obj = (GameObject)selected[i];
            obj.AddComponent<ShowBoxCollider>();
        }
        selected.Clear();
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
