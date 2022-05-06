using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideOrShowButton : MonoBehaviour
{
    public Button button;
    public GameObject SonList;
    private bool HideOrShowState = false;

    void Awake()
    {
        button.onClick.AddListener(HideOrShow);
    }

    public void HideOrShow()
    {
        HideOrShowState = !HideOrShowState;
        SonList.SetActive(HideOrShowState);
    }
}
