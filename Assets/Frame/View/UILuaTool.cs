using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Frame.View
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class UILuaCallCSharp
    {
        private static Dictionary<string, Type> typeDic;
        public static void OnInit()
        {
            typeDic = new Dictionary<string, Type>();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                System.Type v = types[i];// System.Type.GetType(autoRegistorList[i]);
                typeDic[v.FullName] = v;
            }
        }
        public static void OnExcute(string msg, string typeName, object[] body)
        {
            if (typeDic.ContainsKey(typeName) && UILuaTool.UIMgrHandle != null)
            {
                try
                {
                    //object data = JsonUtility.FromJson(body, typeDic[typeName]);
                    UILuaTool.UIMgrHandle.Excute(msg, body);
                }
                catch (Exception e) { Debug.LogError(e.Message + ":" + e.StackTrace); }
            }
        }
    }

    public class UILuaTool
    {
        #region Lua相关操作
        public static IHandleUIManager UIMgrHandle;
#if HOTFIX_ENABLE
        public static XLua.LuaEnv luaenv = new XLua.LuaEnv();
       
        private static XLua.LuaTable scriptEnv;
#endif
#if HOTFIX_ENABLE
        [XLua.CSharpCallLua]
#endif
        public delegate string luaStr(string v1);
#if HOTFIX_ENABLE
        [XLua.CSharpCallLua]
#endif
        public delegate string[] luaArrayStr(string v1);

        private static luaStr _luauiFormName;
        private static luaStr _luacanvasName;
        private static luaStr _luaUiFormType;
        private static luaArrayStr _luaExcuteKeys;
        private static System.Action<string> luaOnInit;
        private static System.Action<string, string, object[]> luaOnExcute;
        private static System.Action<string> luaOnDisplay;
        private static System.Action<string> luaOnHide;
        private static System.Action<string> luaOnReDisplay;
        private static System.Action<string> luaOnFreese;
        private static System.Action<string> luaOnRelease;
        private static System.Action<string> luaOnLoadExcuteKey;
        private static System.Action<string, string> luaOnLoad;

        public static void OnLoadExcuteKey(string msg)
        {
            if (luaOnLoadExcuteKey != null)
                luaOnLoadExcuteKey(msg);
        }
        public static void OnInit(string _uiFormName)
        {
            if (luaOnInit != null)
                luaOnInit(_uiFormName);
        }
        public static void OnLoad(string scrptName, string _uiFormNameStr)
        {
            if (luaOnLoad != null)
                luaOnLoad(scrptName, _uiFormNameStr);
        }
        public static void OnExcute(string _uiFormName, string msg, object[] body)
        {
            if (luaOnExcute != null)
                luaOnExcute(_uiFormName, msg, body);
        }
        public static void OnDisplay(string _uiFormName)
        {
            if (luaOnDisplay != null)
                luaOnDisplay(_uiFormName);
        }

        public static void OnHide(string _uiFormName)
        {
            if (luaOnHide != null)
                luaOnHide(_uiFormName);
        }

        public static void OnReDisplay(string _uiFormName)
        {
            if (luaOnReDisplay != null)
                luaOnReDisplay(_uiFormName);
        }

        public static void OnFreese(string _uiFormName)
        {
            if (luaOnFreese != null)
                luaOnFreese(_uiFormName);
        }

        public static void OnRelease(string _uiFormName)
        {
            if (luaOnRelease != null)
                luaOnRelease(_uiFormName);

        }

        public static void LuaInit()
        {
#if HOTFIX_ENABLE
            if(scriptEnv == null)
            {
                scriptEnv = UIFactory.luaenv.NewTable();
                XLua.LuaTable meta = UIFactory.luaenv.NewTable();
                meta.Set("__index", UIFactory.luaenv.Global);
                scriptEnv.SetMetaTable(meta);
                meta.Dispose();
                scriptEnv.Set("self", this);
                string content = (string)UIFactory.LoadString("LuaUIManager");
                UIManager.luaenv.DoString(content, "LuaUIManager", scriptEnv);
                scriptEnv.Get("excuteKeys", out _luaExcuteKeys);
                scriptEnv.Get("uiFormType", out _luaUiFormType);
                scriptEnv.Get("OnLoad", out luaOnLoad);
                scriptEnv.Get("OnInit", out luaOnInit);
                scriptEnv.Get("OnExcute", out luaOnExcute);
                scriptEnv.Get("OnDisplay", out luaOnDisplay);
                scriptEnv.Get("OnHide", out luaOnHide);
                scriptEnv.Get("OnReDisplay", out luaOnReDisplay);
                scriptEnv.Get("OnFreese", out luaOnFreese);
                scriptEnv.Get("OnRelease", out luaOnRelease);
                scriptEnv.Get("OnLoadExcuteKey", out luaOnLoadExcuteKey);
                scriptEnv.Get("uiFormName", out _luauiFormName);
                scriptEnv.Get("canvasName", out _luacanvasName);
            }
#endif
            UILuaCallCSharp.OnInit();
        }

        public static string[] LuaExcuteKeys(string _uiFormName)
        {
            if (_luaExcuteKeys != null)
                return _luaExcuteKeys(_uiFormName);
            return new string[0];
        }
        public static string LuaUIType(string _uiFormName)
        {
            if (_luaUiFormType != null)
                return _luaUiFormType(_uiFormName);
            return null;
        }
        public static string LuaUIFormName(string scriptName)
        {
            if (_luauiFormName != null)
                return _luauiFormName(scriptName);
            return null;
        }
        public static string LuaCanvasName(string _uiFormName)
        {
            if (_luacanvasName != null)
                return _luacanvasName(_uiFormName);
            return null;
        }
        public static void LoadLuaScript(string scriptName)
        {
            if (UIFactory.LoadString != null)
            {
                string str = (string)UIFactory.LoadString(scriptName);
                if (!string.IsNullOrEmpty(str))
                    OnLoad(scriptName, str);
                else
                    Debug.Log(scriptName + "is null!");
                if (UIFactory.ReleaseObj != null)
                    UIFactory.ReleaseObj("TextAsset", scriptName);
            }
            else
            {
                TextAsset luaStr = Resources.Load<TextAsset>("UIConfigs/" + scriptName);
                if (luaStr != null)
                    OnLoad(scriptName, luaStr.text);
                else
                    Debug.Log(scriptName + "is null!");
            }
        }

        #endregion

    }
}
