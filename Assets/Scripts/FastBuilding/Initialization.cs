using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    int length = 50, wide = 50;//地面的长宽
    GameObject[,] ground;//地面GameObject
    public GameObject MovingCamera;//相机的Target
    void Start()
    {
        MovingCamera.transform.position = new Vector3(length / 2, wide / 2, 0);//将相机Target放置在地面的中间位置

        //创建地面Cube
        ground = new GameObject[length, wide];
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < wide; ++j)
            {
                ground[i, j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ground[i, j].transform.position = new Vector3(i, 1, j);
            }
        }
    }
}
