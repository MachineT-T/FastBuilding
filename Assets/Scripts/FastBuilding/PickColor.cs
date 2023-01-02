using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //判断是否为吸附模式并且鼠标不在UI按钮上
        if (Scene.mode != Scene.Mode.PickColor || Scene.TestUI() || Scene.SelectingAxis)
        {
            return;
        }

        //设置等待选择材质
        CubeSelectView.waitForSelect = true;
        //判断是否已选择材质
        if (CubeSelectView.selected)
        {
            //取消等待选择
            CubeSelectView.waitForSelect = false;
            //替换选中方块的材质
            BuildMode.changeMaterial();
            //恢复原模式
            Scene.ReturnLastMode();
        }
    }
}
