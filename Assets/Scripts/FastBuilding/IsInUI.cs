using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInUI : MonoBehaviour
{
    public GameObject SelectBlock;

    //鼠标进入按钮
    public void InUI()
    {
        SelectBlock.GetComponent<SelectBlock>().IsInUI = true;
        Debug.Log("enterUI");
    }

    //鼠标离开按钮
    public void OutUI()
    {
        SelectBlock.GetComponent<SelectBlock>().IsInUI = false;
        Debug.Log("exitUI");
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
