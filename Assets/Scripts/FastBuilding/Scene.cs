using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    //整个场景的长宽高
    public static int length = 50, wide = 50, height = 50;//地面的长宽高
    //用于管理整个场景每个位置的数组
    static GameObject[,,] blocks;

    public static GameObject[,,] getBlocks()
    {
        return blocks;
    }

    public static void setBlocks(GameObject obj)
    {
        blocks[(int)obj.transform.position.x, (int)obj.transform.position.y, (int)obj.transform.position.z] = obj;
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
