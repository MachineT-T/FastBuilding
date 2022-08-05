using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : MonoBehaviour
{
    //保存选择的材质
    public Material mat;

    //通过碰撞信息获取选择的位置
    protected Vector3 GetPos(RaycastHit hit)
    {
        float TransformX = hit.transform.position.x, TransformY = hit.transform.position.y, TransformZ = hit.transform.position.z;
        float PointX = hit.point.x, PointY = hit.point.y, PointZ = hit.point.z;
        //判断碰撞点在碰撞物体的哪个方向并设置新方块的位置
        Vector3 NewPos = hit.transform.position;
        if (PointX == TransformX + 0.5)
        {
            NewPos.x++;
        }
        else if (PointX == TransformX - 0.5)
        {
            NewPos.x--;
        }
        else if (PointY == TransformY + 0.5)
        {
            NewPos.y++;
        }
        else if (PointY == TransformY - 0.5)
        {
            NewPos.y--;
        }
        else if (PointZ == TransformZ + 0.5)
        {
            NewPos.z++;
        }
        else if (PointZ == TransformZ - 0.5)
        {
            NewPos.z--;
        }
        else//碰撞点是plane
        {
            NewPos += hit.normal * 0.5f;
        }
        return NewPos;
    }

    //尝试在(x,y,z)位置放置方块
    protected void build(int x, int y, int z)
    {
        place(x, y, z);
        //如果打开了X轴镜像模式
        if (MirrorX.MirrorXMode)
        {
            place(MirrorX.GetPosX(x), y, z);
        }
        //如果打开了Y轴镜像模式
        if (MirrorY.MirrorYMode)
        {
            place(x, MirrorY.GetPosY(y), z);
        }
        //如果打开了Z轴镜像模式
        if (MirrorZ.MirrorZMode)
        {
            place(x, y, MirrorZ.GetPosZ(z));
        }
    }

    //尝试在(x,y,z)位置放置方块
    protected void place(int x, int y, int z)
    {
        //获取选中的方块列表的引用
        ArrayList selected = SelectBlock.getSelected();
        //获取场景中的方块信息
        GameObject[,,] blocks = Scene.getBlocks();

        //判断该方块在搭建范围内
        if (Scene.TestPos(x, y, z))
        {
            //如果该位置没有方块则将该位置加入选择区域并添加方块
            if (!Scene.TestBlocks(x, y, z))
            {
                //创建方块对象
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //设置方块材质
                obj.GetComponent<Renderer>().material = mat;
                //设置方块位置
                obj.transform.position = new Vector3(x, y, z);
                //将方块添加进方块信息中
                blocks[x, y, z] = obj;
                Scene.setBlocks(x, y, z, true);
                //将方块添加进选择列表中
                selected.Add(obj);
                //为选中的方块画线
                obj.AddComponent<ShowBoxCollider>();
                //将方块设置为不能被射线检测
                obj.layer = 2;
            }
        }
    }

    //松开鼠标完成搭建
    protected void finish()
    {
        //获取选中的方块列表的引用
        ArrayList selected = SelectBlock.getSelected();
        for (int i = 0; i < selected.Count; ++i)
        {
            GameObject obj = (GameObject)selected[i];
            obj.layer = 0;
        }
        //为移动模式保存选中方块的初始位置
        MoveMode.RecordBlockInitPos();
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
