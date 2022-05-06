using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frame.View
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class LuaUIBehavior : UIBase
    {
        private UIType _luaFormType;
        protected override List<string> m_ExcuteMsgs
        {
            get
            {
                if (_luaExcuteList == null)
                    _luaExcuteList = new List<string>();
                return _luaExcuteList;
            }
        }
        protected List<string> _luaExcuteList;
        private string _uiFormName;
        protected override string m_uiFormName { get { return _uiFormName; } }
        private string _canvasName;
        protected override string m_canvasName { get{ return _canvasName; } }

        protected override UIType m_uiFormType
        {
            get
            {
                if (_luaFormType == null)
                {
                    Debug.LogError("uiFormType is null!");
                    _luaFormType = new UIType();
                }
                return _luaFormType;// luaenv.Global.Get<UIType>("uiFormType");
            }
        }


        public void LoadLuaString(string scriptName)
        {

            UILuaTool.LoadLuaScript(scriptName);
            _uiFormName = UILuaTool.LuaUIFormName(scriptName);
            _canvasName = UILuaTool.LuaCanvasName(_uiFormName);
            _luaExcuteList = new List<string>(UILuaTool.LuaExcuteKeys(_uiFormName));
            
            JsonData jd = JsonMapper.ToObject(UILuaTool.LuaUIType(_uiFormName));
            _luaFormType = new UIType();
            _luaFormType.IsClearStack = bool.Parse(jd["IsClearStack"].ToString());
            _luaFormType.IsNewCanvas = bool.Parse(jd["IsNewCanvas"].ToString());
            _luaFormType.UIForms_ShowMode = (UIFormShowMode)int.Parse(jd["UIForms_ShowMode"].ToString());
            _luaFormType.UIForms_Type = (UIFormType)int.Parse(jd["UIForms_Type"].ToString());
            _luaFormType.UIForm_LucencyType = (UIFormLucenyType)int.Parse(jd["UIForm_LucencyType"].ToString());
           
        }

        protected override void OnInit()
        {
            UILuaTool.OnInit(_uiFormName);
        }

        protected override void OnExcute(string msg, object[] body)
        {
            UILuaTool.OnExcute(_uiFormName,msg, body);
        }
        protected override void OnDisplay()
        {
            UILuaTool.OnDisplay(_uiFormName);
        }

        protected override void OnHide()
        {
            UILuaTool.OnHide(_uiFormName);
        }

        protected override void OnReDisplay()
        {
            UILuaTool.OnReDisplay(_uiFormName);
        }

        protected override void OnFreese()
        {
            UILuaTool.OnFreese(_uiFormName);
        }

        protected override void OnRelease(bool isRecycle)
        {
            UILuaTool.OnRelease(_uiFormName);

        }

    }
}
