using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMode : MonoBehaviour
{
    //判断按下左键时是否选中了被选择的方块
    bool HitSelect = false;
    //记录按下鼠标时的位置
    Vector3 StartPos;
    //记录当前鼠标位置
    Vector3 CurrentPos;
    //判断是否已经确定移动的方向
    bool DirectionFlag = false;
    //移动的方向枚举变量
    enum Direction { UpAndDown, ForwardAndBack, LeftAndRight };
    //物体移动的方向
    Direction direction;
    //记录选中方块的初始位置
    static ArrayList BlockInitPos = new ArrayList();
    //记录选中方块开始移动的位置
    static ArrayList BlockStartPos = new ArrayList();
    //记录选中方块的当前位置
    static ArrayList BlockCurrentPos = new ArrayList();
    //鼠标移动距离与方块移动距离的换算比率
    const float MoveDistanceRatio = 100.0f;
    //方块移动距离
    int MoveDistance;
    //用于获取选中方块列表的引用
    static ArrayList selected = new ArrayList();
    //用于获取场景方块引用
    static GameObject[,,] blocks = new GameObject[0, 0, 0];

    //记录选中方块的初始位置
    public static void RecordBlockInitPos()
    {
        //更新选中方块的引用
        selected = SelectBlock.getSelected();
        //清空初始位置列表
        BlockInitPos.Clear();
        for (int i = 0; i < selected.Count; i++)
        {
            BlockInitPos.Add(((GameObject)selected[i]).transform.position);
        }
    }

    //将超出搭建范围的位置修改回搭建范围内
    Vector3 CorrectPos(Vector3 pos)
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

    //确定方块的移动,将选中方块新位置上的旧方块销毁，使用选中方块来代替
    public static void ConfirmMoving()
    {
        //更新选中方块的引用
        selected = SelectBlock.getSelected();
        //更新场景方块信息的引用
        blocks = Scene.getBlocks();

        for (int i = 0; i < selected.Count; i++)
        {
            GameObject temp = (GameObject)selected[i];
            int x = (int)temp.transform.position.x, y = (int)temp.transform.position.y, z = (int)temp.transform.position.z;

            //判断选中方块当前位置上是否原本就有方块
            if (Scene.TestBlocks(x, y, z))
            {
                //判断旧方块是否在选中方块列表中防止误销毁
                if (!selected.Contains(blocks[x, y, z]))
                {
                    //旧方块未被选中则销毁旧方块
                    Destroy(blocks[x, y, z]);
                }
            }

            //将方块信息保存到新位置上
            blocks[x, y, z] = temp;
            Scene.setBlocks(x, y, z, true);

            //判断是否方块发生了移动但是原位置仍是原来的方块信息
            int xx = (int)((Vector3)BlockInitPos[i]).x, yy = (int)((Vector3)BlockInitPos[i]).y, zz = (int)((Vector3)BlockInitPos[i]).z;
            if (blocks[xx, yy, zz] == blocks[x, y, z] && (xx != x || yy != y || zz != z))
            {
                //令原位置的方块信息指向null
                blocks[(int)((Vector3)BlockInitPos[i]).x, (int)((Vector3)BlockInitPos[i]).y, (int)((Vector3)BlockInitPos[i]).z] = null;
                //设置原位置无方块
                Scene.setBlocks((int)((Vector3)BlockInitPos[i]).x, (int)((Vector3)BlockInitPos[i]).y, (int)((Vector3)BlockInitPos[i]).z, false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //判断是否为移动模式并且鼠标不在UI按钮上
        if (Scene.mode != Scene.Mode.move || Scene.TestUI() || Scene.SelectingAxis)
        {
            return;
        }

        //获取选中方块列表的引用
        selected = SelectBlock.getSelected();
        //获取场景方块引用
        blocks = Scene.getBlocks();

        //判断按下左键
        if (Input.GetMouseButtonDown(0))
        {
            //射线检测
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //判断射线检测到的物体是否在选择列表中
                if (selected.Contains(hit.transform.gameObject))
                {
                    HitSelect = true;
                    //记录按下鼠标左键时位置
                    StartPos = Input.mousePosition;
                    //记录选中方块的初始位置
                    for (int i = 0; i < selected.Count; i++)
                    {
                        BlockStartPos.Add(((GameObject)selected[i]).transform.position);
                        BlockCurrentPos.Add(((GameObject)selected[i]).transform.position);
                    }
                }
            }
        }

        //如果按下左键时未选中被选择的方块则不继续进行后续操作
        if (!HitSelect)
        {
            return;
        }

        //判断按住左键
        if (Input.GetMouseButton(0))
        {
            //记录鼠标当前位置
            CurrentPos = Input.mousePosition;
            //将鼠标移动的屏幕坐标向量转换为世界坐标向量
            // Vector3 MouseMoveVector = Camera.main.ScreenToWorldPoint(new Vector3(CurrentPos.x, CurrentPos.y, Camera.main.transform.position.z))
            // - Camera.main.ScreenToWorldPoint(new Vector3(StartPos.x, StartPos.y, Camera.main.transform.position.z));
            Vector3 MouseCurrentPos = Camera.main.ScreenToWorldPoint(new Vector3(CurrentPos.x, CurrentPos.y, Camera.main.nearClipPlane));
            Vector3 MouseStartPos = Camera.main.ScreenToWorldPoint(new Vector3(StartPos.x, StartPos.y, Camera.main.nearClipPlane));
            Vector3 MouseMoveVector = MouseCurrentPos - MouseStartPos;
            //计算鼠标移动距离
            MoveDistance = (int)(Vector3.Distance(StartPos, CurrentPos));

            //如果鼠标移动的距离大于一定像素值则确定物体移动的方向
            if (MoveDistance >= 30 && !DirectionFlag)
            {
                //计算鼠标移动向量与三条坐标轴的角度
                float AngleX = Mathf.Min(Vector3.Angle(MouseMoveVector, Vector3.left), Vector3.Angle(MouseMoveVector, Vector3.right));
                float AngleY = Mathf.Min(Vector3.Angle(MouseMoveVector, Vector3.up), Vector3.Angle(MouseMoveVector, Vector3.down));
                float AngleZ = Mathf.Min(Vector3.Angle(MouseMoveVector, Vector3.forward), Vector3.Angle(MouseMoveVector, Vector3.back));
                //判断鼠标移动方向与哪条坐标轴角度最小，取该坐标轴为移动方向
                if (AngleX <= AngleY && AngleX <= AngleZ)
                {
                    direction = Direction.LeftAndRight;
                }
                else if (AngleY <= AngleX && AngleY <= AngleZ)
                {
                    direction = Direction.UpAndDown;
                }
                else if (AngleZ <= AngleX && AngleZ <= AngleY)
                {
                    direction = Direction.ForwardAndBack;
                }
                //标记已确定移动方向
                DirectionFlag = true;
            }

            //确定了移动的方向后可以进行移动操作
            if (DirectionFlag)
            {
                //物体移动向量
                Vector3 BlockMoveVector = new Vector3(0, 0, 0);

                //根据移动方向的不同，以及鼠标正向反向移动求物体移动向量
                switch (direction)
                {
                    case Direction.LeftAndRight:
                        BlockMoveVector.x = (int)((MouseCurrentPos.x - MouseStartPos.x) * MoveDistanceRatio);
                        break;
                    case Direction.UpAndDown:
                        BlockMoveVector.y = (int)((MouseCurrentPos.y - MouseStartPos.y) * MoveDistanceRatio);
                        break;
                    case Direction.ForwardAndBack:
                        BlockMoveVector.z = (int)((MouseCurrentPos.z - MouseStartPos.z) * MoveDistanceRatio);
                        break;
                }
                Debug.Log("MouseCurrentPos=" + MouseCurrentPos);
                Debug.Log("MouseStartPos=" + MouseStartPos);
                Debug.Log("BlockMoveVector=" + BlockMoveVector);
                //通过移动向量求出物体当前位置
                for (int i = 0; i < BlockStartPos.Count; i++)
                {
                    //移动前先判断前一帧的物体位置原本是否有方块，有则恢复显示
                    Vector3 temp = (Vector3)BlockCurrentPos[i];
                    if (Scene.TestBlocks((int)temp.x, (int)temp.y, (int)temp.z))
                    {
                        blocks[(int)temp.x, (int)temp.y, (int)temp.z].SetActive(true);
                    }

                    //计算物体当前位置
                    BlockCurrentPos[i] = CorrectPos((Vector3)BlockStartPos[i] + BlockMoveVector);

                    //判断当前物体位置是否已有方块
                    temp = (Vector3)BlockCurrentPos[i];
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
                    ((GameObject)selected[i]).transform.position = (Vector3)BlockCurrentPos[i];
                }
            }
        }

        //判断松开左键
        if (Input.GetMouseButtonUp(0))
        {
            //重新设置未选中被选择的方块
            HitSelect = false;
            //重新设置未确定移动方向
            DirectionFlag = false;

            //清空保存位置的列表
            BlockStartPos.Clear();
            BlockCurrentPos.Clear();
        }
    }
}
