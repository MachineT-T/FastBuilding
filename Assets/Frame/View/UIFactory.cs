using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Frame.View
{
    public class UIFactory : MonoBehaviour
    {
        public const string DEFAULT_CANVAS = "defalutCanvas";
        public static string configName = "UIPrefabConfig";
        public static Frame.Model.FMAction<string>.GObject<string> LoadString;
        public static Frame.Model.FMAction<GameObject>.GObject<string> LoadPrefab;
        public static Frame.Model.FMAction.VAction<string, string> ReleaseObj;
        public static Frame.Model.FMAction.VAction<UIBase> OnLoadExcuteKey;
        public static Frame.Model.FMAction.VAction<UIBase, UIPrefabConfigNode> CheckLuaUIForm;

        private static Dictionary<string, System.Type> uibaseTypes;
        private static Dictionary<string, UIPrefabConfigNode> m_PrefabsDict;

        private static void InitUIBaseType()
        {
            if (uibaseTypes == null)
            {
                uibaseTypes = new Dictionary<string, System.Type>();
                var types = AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(a => a.GetTypes()).ToArray();
                for (int i = 0; i < types.Length; i++)
                {
                    System.Type v = types[i];// System.Type.GetType(autoRegistorList[i]);
                    if (v.IsSubclassOf(typeof(UIBase)))
                        uibaseTypes[v.FullName] = v;
                }
            }

        }
        public static void OnLoad()
        {
            InitUIBaseType();
            if (m_PrefabsDict == null)
                m_PrefabsDict = new Dictionary<string, UIPrefabConfigNode>();
            LoadConfigure(configName);
        }

        public static void LoadConfigure(string configName)
        {
            try
            {
                string content = "";
                if (LoadString != null)
                {
                    content = LoadString(configName);
                    if (UIFactory.ReleaseObj != null)
                        UIFactory.ReleaseObj("TextAsset", configName);
                }
                else
                {
                    TextAsset uiConfig = Resources.Load<TextAsset>("UIConfigs/" + configName);
                    if (uiConfig != null)
                        content = uiConfig.text;
                }
                UIPrefabConfigInfo info = JsonUtility.FromJson<UIPrefabConfigInfo>(content);
                for (int i = 0; i < info.UIPrefabInfo.Count; i++)
                {
                    m_PrefabsDict[info.UIPrefabInfo[i].UIFormName] = info.UIPrefabInfo[i];
                }
            }
            catch { }
        }

        public static void ReleaseForm(string formName)
        {
            if (m_PrefabsDict.ContainsKey(formName))
            {
                if (ReleaseObj != null)
                    ReleaseObj("GameObject", m_PrefabsDict[formName].UIFormPrefabName);
            }
        }

        public static GameObject LoadUIMask()
        {
            GameObject prefab = null;
            if (LoadPrefab != null)
            {
                prefab = (GameObject)LoadPrefab("UIMaskPanel");
            }
            prefab = Resources.Load<GameObject>("UIPrefabs/UIMaskPanel");
            if (prefab == null)
                return null;
            GameObject go = GameObject.Instantiate(prefab);
            go.name = "UIMaskPanel";
            return go;
        }


        /// <summary>
        /// 获取预设
        /// </summary>
        /// <param name="uiFormName"></param>
        /// <returns></returns>
        public static GameObject LoadUIPrefab(string uiFormName)
        {
            UIPrefabConfigNode uiPrefabInfoNode = null;
            m_PrefabsDict.TryGetValue(uiFormName, out uiPrefabInfoNode);
            if (uiPrefabInfoNode == null)
            {
                Debug.LogError(uiFormName + ":UIFormName is null!");
                return null;
            }

            GameObject prefab = null;
            if (LoadPrefab != null)
            {
                prefab = (GameObject)LoadPrefab(uiPrefabInfoNode.UIFormPrefabName);
            }
            else
            {
                prefab = Resources.Load<GameObject>("UIPrefabs/" + uiPrefabInfoNode.UIFormPrefabName);
            }
            if (prefab == null)
            {
                Debug.LogError("prefab is null!" + uiPrefabInfoNode.UIFormPrefabName);
                return null;
            }

            GameObject go = GameObject.Instantiate(prefab);
            go.name = prefab.name;

            Component uibase = go.GetComponent<UIBase>();

            if (uibase == null)
            {
                System.Type monoType = null;
                if (!uiPrefabInfoNode.UIFormLuaScript)
                {
                    uibaseTypes.TryGetValue(uiPrefabInfoNode.UIFormClassName, out monoType);
                    //monoType = System.Type.GetType(uiPrefabInfoNode.UIFormClassName);
                }
                else
                {
                    uibaseTypes.TryGetValue("Frame.View.LuaUIBehavior", out monoType);
                    //monoType = System.Type.GetType("Frame.View.LuaUIBehavior");
                }
                if (monoType == null)
                {
                    Debug.LogError("monoType is null!:" + uiPrefabInfoNode.UIFormLuaScript);
                    return null;
                }
                uibase = go.AddComponent(monoType);
                if (CheckLuaUIForm != null)
                {
                    CheckLuaUIForm((UIBase)uibase, uiPrefabInfoNode);
                }
            }
            return uibase.gameObject;
        }

        public static IHandleUIManager CreateInternalUIManager()
        {
            return new InternalUIManager();
        }
        public static T GetParentComponentScript<T>(Transform go) where T : Component
        {
            T t = null;
            if (go.parent != null)
            {
                t = go.parent.GetComponent<T>();
                if (go.root != go.parent && t == null)
                {
                    t = GetParentComponentScript<T>(go.parent);
                }
            }
            return t;
        }
    }
}