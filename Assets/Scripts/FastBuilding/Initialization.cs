using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    int length = 50, wide = 50;//地面的长宽
    GameObject[,] ground;//地面GameObject
    void Start()
    {
        Camera.main.transform.position = new Vector3(length / 2, wide / 2, 0);//将相机放置在地面的中间位置

        //调整相机角度使相机向下面对地面
        float EulerX = Camera.main.transform.eulerAngles.x;
        float EulerY = Camera.main.transform.eulerAngles.y;
        Camera.main.transform.rotation = Quaternion.Euler(EulerX + 60, EulerY, 0);

        //创建地面Cube
        ground = new GameObject[length, wide];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                //将GameObject转换为Cube
                ground[i, j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //设置Cube的位置
                ground[i, j].transform.position = new Vector3(i, 1, j);
                //为地面设置标签防止被删除
                ground[i, j].tag = "Ground";
            }
        }
    }
}
