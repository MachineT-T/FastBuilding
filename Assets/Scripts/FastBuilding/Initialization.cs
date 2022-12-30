using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Initialization : MonoBehaviour
{
    float length = 0, wide = 0, height = 0;//地面的长宽高
    public InputField lengthInput, wideInput, heightInput;//长宽高输入框
    GameObject[,] front, back, up, down, left, right;//地面GameObject
    public Material mat;//地面材质
    void Start()
    {
        //默认创建长宽高为50的平面
        createPlane(50, 50, 50);

        Camera.main.transform.position = new Vector3(length / 2, height / 2, -wide * 1.5f);//将相机放置在场景的中间位置

        //将文本修改事件绑定
        lengthInput.onEndEdit.AddListener(delegate { editLength(); });
        wideInput.onEndEdit.AddListener(delegate { editWide(); });
        heightInput.onEndEdit.AddListener(delegate { editHeight(); });
    }

    //创建地面Plane
    public void createPlane(int x, int y, int z)
    {
        //销毁原地面Plane
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                Destroy(down[i, j]);
                Destroy(up[i, j]);
            }
        }
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Destroy(front[i, j]);
                Destroy(back[i, j]);
            }
        }
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                Destroy(left[i, j]);
                Destroy(right[i, j]);
            }
        }

        //修改长宽高
        length = x;
        wide = y;
        height = z;

        //创建地面Plane
        down = new GameObject[(int)length, (int)wide];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                //将GameObject转换为Plane
                down[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //设置Plane的大小
                down[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //设置Plane的位置
                down[i, j].transform.position = new Vector3(i, -0.5f, j);
                //为地面设置标签防止被删除
                down[i, j].tag = "Ground";
                //设置地面材质
                down[i, j].GetComponent<Renderer>().material = mat;
                //为地面画分割线
                //down[i, j].AddComponent<ShowGroundCollider>();
            }
        }
        up = new GameObject[(int)length, (int)wide];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                //将GameObject转换为Plane
                up[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //设置Plane的大小
                up[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //设置Plane的位置
                up[i, j].transform.position = new Vector3(i, height - 0.5f, j);
                //设置Plane的旋转角度
                up[i, j].transform.rotation = Quaternion.Euler(180, 0, 0);
                //为地面设置标签防止被删除
                up[i, j].tag = "Ground";
                //设置地面材质
                up[i, j].GetComponent<Renderer>().material = mat;
                //为地面画分割线
                //up[i, j].AddComponent<ShowGroundCollider>();
            }
        }
        front = new GameObject[(int)length, (int)height];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                //将GameObject转换为Plane
                front[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //设置Plane的大小
                front[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //设置Plane的位置
                front[i, j].transform.position = new Vector3(i, j, wide - 0.5f);
                //设置Plane的旋转角度
                front[i, j].transform.rotation = Quaternion.Euler(-90, 0, 0);
                //为地面设置标签防止被删除
                front[i, j].tag = "Ground";
                //设置地面材质
                front[i, j].GetComponent<Renderer>().material = mat;
                //为地面画分割线
                //front[i, j].AddComponent<ShowGroundCollider>();
            }
        }
        back = new GameObject[(int)length, (int)height];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                //将GameObject转换为Plane
                back[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //设置Plane的大小
                back[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //设置Plane的位置
                back[i, j].transform.position = new Vector3(i, j, -0.5f);
                //设置Plane的旋转角度
                back[i, j].transform.rotation = Quaternion.Euler(90, 0, 0);
                //为地面设置标签防止被删除
                back[i, j].tag = "Ground";
                //设置地面材质
                back[i, j].GetComponent<Renderer>().material = mat;
                //为地面画分割线
                //back[i, j].AddComponent<ShowGroundCollider>();
            }
        }
        left = new GameObject[(int)height, (int)wide];
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                //将GameObject转换为Plane
                left[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //设置Plane的大小
                left[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //设置Plane的位置
                left[i, j].transform.position = new Vector3(-0.5f, i, j);
                //设置Plane的旋转角度
                left[i, j].transform.rotation = Quaternion.Euler(0, 0, -90);
                //为地面设置标签防止被删除
                left[i, j].tag = "Ground";
                //设置地面材质
                left[i, j].GetComponent<Renderer>().material = mat;
                //为地面画分割线
                //left[i, j].AddComponent<ShowGroundCollider>();
            }
        }
        right = new GameObject[(int)height, (int)wide];
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                //将GameObject转换为Plane
                right[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //设置Plane的大小
                right[i, j].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //设置Plane的位置
                right[i, j].transform.position = new Vector3(length - 0.5f, i, j);
                //设置Plane的旋转角度
                right[i, j].transform.rotation = Quaternion.Euler(0, 0, 90);
                //为地面设置标签防止被删除
                right[i, j].tag = "Ground";
                //设置地面材质
                right[i, j].GetComponent<Renderer>().material = mat;
                //为地面画分割线
                //right[i, j].AddComponent<ShowGroundCollider>();
            }
        }
    }

    //编辑长度
    private void editLength()
    {
        //确定选中方块的移动
        MoveMode.ConfirmMoving();
        //清空选择列表
        SelectBlock.ClearSelected();
        createPlane(int.Parse(lengthInput.text), (int)wide, (int)height);
        Scene.editLength(int.Parse(lengthInput.text));
    }

    //编辑宽度
    private void editWide()
    {
        //确定选中方块的移动
        MoveMode.ConfirmMoving();
        //清空选择列表
        SelectBlock.ClearSelected();
        createPlane((int)length, int.Parse(wideInput.text), (int)height);
        Scene.editWide(int.Parse(wideInput.text));
    }

    //编辑高度
    private void editHeight()
    {
        //确定选中方块的移动
        MoveMode.ConfirmMoving();
        //清空选择列表
        SelectBlock.ClearSelected();
        createPlane((int)length, (int)wide, int.Parse(heightInput.text));
        Scene.editHeight(int.Parse(heightInput.text));
    }
}
