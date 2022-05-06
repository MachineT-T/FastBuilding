using Frame.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TestForm : UIWindows
{
    protected override List<string> m_ExcuteMsgs
    {
        get
        {
            if (_excuteMsgs == null)
                _excuteMsgs = new List<string>();
            return _excuteMsgs;
        }
    }
    private List<string> _excuteMsgs;
    protected override string m_canvasName
    {
        get
        {
            return "";
        }
    }

    protected override string m_uiFormName
    {
        get
        {
            return "TestForm";
        }
    }

    protected override UIType m_uiFormType
    {
        get
        {
            if (_uiFormType == null)
            {
                _uiFormType = new UIType();
                _uiFormType.UIForms_ShowMode = UIFormShowMode.Normal;
                _uiFormType.UIForms_Type = UIFormType.Normal;
                _uiFormType.UIForm_LucencyType = UIFormLucenyType.Pentrate;
            }
            return _uiFormType;
        }
    }
    private UIType _uiFormType;
    protected override void OnExcute(string msg, object[] body)
    {
        
    }

    protected override void OnInit()
    {
        Button btn = Container.transform.Find("btnPopup").GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            Frame.View.UIManager.Instance.ShowUIForm("TestForm2");
        });
    }

}
