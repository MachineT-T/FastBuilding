using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorOff : MonoBehaviour
{
    //点击关闭镜像按钮
    public void ClickMirrorOff()
    {
        MirrorX.OffMIrrorX();
        MirrorY.OffMIrrorY();
        MirrorZ.OffMIrrorZ();
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
