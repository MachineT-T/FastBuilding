using UnityEngine;
using System.Collections.Generic;
using System;

namespace Frame.View
{

    public class UIManager : MonoBehaviour, IHandleUIManager
    {
        private static volatile Frame.View.UIManager instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static Frame.View.UIManager Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }
                if (instance == null)
                {

                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            Frame.View.UIManager[] instance1 = FindObjectsOfType<Frame.View.UIManager>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(Frame.View.UIManager).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<Frame.View.UIManager>();
                    instance.InitView();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }
        private object lockObj = new object();
        private void Awake()
        {
            Frame.View.UIManager t = gameObject.GetComponent<Frame.View.UIManager>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(Frame.View.UIManager).FullName;
                instance = t;
                InitView();
                _applicationIsQuitting = false;
            }
            else if (singletonObj != gameObject)
            {
                MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
                if (monos.Length > 1)
                {
                    Destroy(t);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }


        private IHandleUIManager m_UIMgr;


        private void OnDestroy()
        {
            if(singletonObj == gameObject && !_applicationIsQuitting)
                ReleaseView();
        }

        public void InitView()
        {
            m_UIMgr = UIFactory.CreateInternalUIManager();
            m_UIMgr.InitView();
            UILuaTool.UIMgrHandle = m_UIMgr;
            UIFactory.OnLoad();
            UILuaTool.LuaInit();
            UIFactory.OnLoadExcuteKey = (uiBase) =>
            {
                string[] msgs = uiBase.excuteMsgs.ToArray();
                for (int i = 0; i < msgs.Length; i++)
                {
                    if (uiBase.GetType().Name.Contains("LuaUIBehavior"))
                    {
                        UILuaTool.OnLoadExcuteKey(msgs[i]);
                    }
                }
            };
            UIFactory.CheckLuaUIForm = (v1, v2) => 
            {
                if (v2.UIFormLuaScript)
                {
                    if (v1  is Frame.View.LuaUIBehavior)
                    {
                        LuaUIBehavior luib = (LuaUIBehavior)v1;
                        luib.LoadLuaString(v2.UIFormClassName);
                    }
                    else
                    {
                        Debug.LogError(v2.UIFormClassName + ":LoadLuaString(bool isAssetBundle, string dirType, string scriptName, string scriptPath) is null");
                    }
                }
            };
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="uIFormName"></param>
        public void ShowUIForm(string uIFormName)
        {

            m_UIMgr.ShowUIForm(uIFormName);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="uIFormName"></param>
        public void CloseUIForm(string uIFormName)
        {
            m_UIMgr.CloseUIForm(uIFormName);
        }

        public void Excute(string InfoState,params object[] data)
        {
            m_UIMgr.Excute(InfoState, data);
        }

        public void AddUIFormToCatch(UIBase uIBase)
        {
            m_UIMgr.AddUIFormToCatch(uIBase);
        }

        public void RemoveUIFormToCatch(string uIFormName)
        {
            m_UIMgr.RemoveUIFormToCatch(uIFormName);
        }

        public void CloseAllUIForm()
        {
            m_UIMgr.CloseAllUIForm();
        }

        public void ReleaseView()
        {
            _applicationIsQuitting = true;
            m_UIMgr.ReleaseView();
        }

        public void RegisterExcute(string uiforname, string[] exctes)
        {
            m_UIMgr.RegisterExcute(uiforname, exctes);
        }

        public void UnRegisterExcute(string uiforname, string[] exctes)
        {
            m_UIMgr.UnRegisterExcute(uiforname, exctes);
        }

        public void ShowUIForm(string uIFormName, string msg, params object[] body)
        {
            m_UIMgr.ShowUIForm(uIFormName, msg, body);
        }

        public void ReleaseForm(string uIFormName)
        {
            m_UIMgr.ReleaseForm(uIFormName);
        }

        public void LoadUIFormSync(string uIFormName)
        {
            m_UIMgr.LoadUIFormSync(uIFormName);
        }

        public void LoadUIFormAsyn(string uIFormName, Action<string> callback)
        {
            m_UIMgr.LoadUIFormAsyn(uIFormName, callback);
        }
    }
}