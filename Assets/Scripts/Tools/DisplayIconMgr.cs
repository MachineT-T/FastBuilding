using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VR_ChuangKe.Share
{
    
    /// <summary>
    /// 资源的信息显示
    /// </summary>
    public class DisplayIconMgr 
    {
        private static DisplayIconMgr play_singleton = default(DisplayIconMgr);
        private static DisplayIconMgr edit_singleton = default(DisplayIconMgr);
        private static object s_objectLock = new object();
        public static DisplayIconMgr Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEngine.Application.isPlaying)
                {
                    if (DisplayIconMgr.play_singleton != null)
                    {
                        DisplayIconMgr.play_singleton = null;
                    }
                    if (DisplayIconMgr.edit_singleton == null)
                    {
                        object obj;
                        Monitor.Enter(obj = DisplayIconMgr.s_objectLock);//加锁防止多线程创建单例
                        try
                        {
                            if (DisplayIconMgr.edit_singleton == null)
                            {
                                DisplayIconMgr.edit_singleton = ((default(DisplayIconMgr) == null) ? Activator.CreateInstance<DisplayIconMgr>() : default(DisplayIconMgr));//创建单例的实例
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj);
                        }
                    }
                    return DisplayIconMgr.edit_singleton;
                }
                else
                {
                    if (DisplayIconMgr.edit_singleton != null)
                    {
                        DisplayIconMgr.edit_singleton = null;
                    }
                    if (DisplayIconMgr.play_singleton == null)
                    {
                        object obj;
                        Monitor.Enter(obj = DisplayIconMgr.s_objectLock);//加锁防止多线程创建单例
                        try
                        {
                            if (DisplayIconMgr.play_singleton == null)
                            {
                                DisplayIconMgr.play_singleton = ((default(DisplayIconMgr) == null) ? Activator.CreateInstance<DisplayIconMgr>() : default(DisplayIconMgr));//创建单例的实例
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj);
                        }
                    }
                    return DisplayIconMgr.play_singleton;
                }

#else
                if (DisplayIconMgr.play_singleton == null)
                    {
                        object obj;
                        Monitor.Enter(obj = DisplayIconMgr.s_objectLock);//加锁防止多线程创建单例
                        try
                        {
                            if (DisplayIconMgr.play_singleton == null)
                            {
                                DisplayIconMgr.play_singleton = ((default(DisplayIconMgr) == null) ? Activator.CreateInstance<DisplayIconMgr>() : default(DisplayIconMgr));//创建单例的实例
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj);
                        }
                    }
                    return DisplayIconMgr.play_singleton;
#endif
            }
        }
        private Dictionary<string, string> inforDict;
        public DisplayIconMgr()
        {
            inforDict = new Dictionary<string, string>();
        }

        public void updateDisplayIcon(string name,string icon)
        {
            inforDict[name] = icon;        
        }

        public void delDisplayIcon(string name)
        {
            inforDict.Remove(name);
        }

        public void release()
        {
            inforDict.Clear();
            if (UnityEngine.Application.isPlaying)
            {
                play_singleton = null;
            }
        }

        public string getIcon(string i_key)
        {
            string icon = null;
            if (!inforDict.TryGetValue(i_key, out icon))
            {
                icon = readIconConfig(i_key);
            }
            return icon; 
        }

        public Texture getTexture(string i_key)
        {
            string icon = getIcon(i_key);
            if (!string.IsNullOrEmpty(icon))
            {
                Texture tex = ResLibaryMgr.Instance.GetTexture2d(icon);
                if (tex != null)
                    return tex;
                tex = FileTool.readLocalTexture2d(icon);
                return tex;
            }
            return null;
            
        }
        public void release(string i_key)
        {
            string icon = getIcon(i_key);
            if (!string.IsNullOrEmpty(icon))
            {
                ResLibaryMgr.Instance.releaseObj(ResLibary.ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D],icon);
            }
        }
        private string readIconConfig(string i_key)
        {
            string i_value = null;
            string str = ResLibaryMgr.Instance.GetTextAsset(i_key + "_config");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    if (isjson(UnityTool.Decrypt(str)))
                        str = UnityTool.Decrypt(str);
                    JsonData jd = JsonMapper.ToObject(str);
                    i_value = jd["icon"].ToString();
                    inforDict[jd["name"].ToString()] = i_value;
                }
                catch { }
            }
            return i_value;
        }
        protected bool isjson(string content)
        {
            try
            {
                JsonData jd = JsonMapper.ToObject(content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
