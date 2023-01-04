using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShow : MonoBehaviour
{
    public GameObject SonList;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //显示子菜单
    public void showList()
    {
        SonList.SetActive(true);
    }

    //隐藏子菜单
    public void hideList()
    {
        SonList.SetActive(false);
    }
}
