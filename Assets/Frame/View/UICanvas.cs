using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Frame.View
{
    public class UICanvas : UIWindows
    {
        protected override string m_canvasName
        {
            get
            {
                return UIFactory.DEFAULT_CANVAS;
            }
        }

        protected override string m_uiFormName
        {
            get
            {
                return UIFactory.DEFAULT_CANVAS;
            }
        }
        private UIType _uiFormType;
        protected override UIType m_uiFormType
        {
            get
            {
                if (_uiFormType == null)
                {
                    _uiFormType = new UIType();
                    _uiFormType.IsNewCanvas = true;
                    _uiFormType.UIForms_ShowMode = UIFormShowMode.Normal;
                    _uiFormType.UIForms_Type = UIFormType.Normal;
                    _uiFormType.UIForm_LucencyType = UIFormLucenyType.Pentrate;
                }
                return _uiFormType;
            }
        }
        private List<string> _excuteMsgs;
        protected override List<string> m_ExcuteMsgs
        {
            get
            {
                if (_excuteMsgs == null)
                    _excuteMsgs = new List<string>();
                return _excuteMsgs;
            }
        }

        //public static Transform sCanvas { get { return _sCanvas; } }
        //private static Transform _sCanvas;

        //public bool IsGCanvas = false;

        protected override void OnInit()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected override void OnExcute(string msg, object[] body)
        {
           
        }
    }
}
