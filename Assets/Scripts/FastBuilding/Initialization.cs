using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Initialization : MonoBehaviour
{
    float length = 50, wide = 50, height = 50;//地面的长宽高
    GameObject[,] front, back, up, down, left, right;//地面GameObject
    public Material mat;//地面材质
    void Start()
    {
        Camera.main.transform.position = new Vector3(length / 2, height / 2, -wide * 1.5f);//将相机放置在场景的中间位置

        //调整相机角度使相机向下面对地面
        // float EulerX = Camera.main.transform.eulerAngles.x;
        // float EulerY = Camera.main.transform.eulerAngles.y;
        // Camera.main.transform.rotation = Quaternion.Euler(EulerX + 20, EulerY, 0);

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
                front[i, j].transform.position = new Vector3(i, j, length - 0.5f);
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
}
