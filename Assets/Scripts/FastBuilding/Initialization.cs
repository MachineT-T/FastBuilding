using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    int length = 50, wide = 50;
    GameObject[,] ground;
    void Start()
    {
        Camera.main.transform.position = new Vector3(length / 2, wide / 2, 0);
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
