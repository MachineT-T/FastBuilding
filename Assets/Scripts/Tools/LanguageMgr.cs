using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.Threading;
using System.Linq;

namespace VR_ChuangKe.Share
{
    public class LanguageMgr
    {
        private static LanguageMgr play_singleton = default(LanguageMgr);
        private static LanguageMgr edit_singleton = default(LanguageMgr);
        private static object s_objectLock = new object();
        public static LanguageMgr Instance
        {
            get
            {
#if UNITY_EDITOR

                if (!UnityEngine.Application.isPlaying)
                {
                    if (LanguageMgr.play_singleton != null)
                    {
                        LanguageMgr.play_singleton = null;
                    }
                    if (LanguageMgr.edit_singleton == null)
                    {
                        object obj;
                        Monitor.Enter(obj = LanguageMgr.s_objectLock);//加锁防止多线程创建单例
                        try
                        {
                            if (LanguageMgr.edit_singleton == null)
                            {
                                LanguageMgr.edit_singleton = ((default(LanguageMgr) == null) ? Activator.CreateInstance<LanguageMgr>() : default(LanguageMgr));//创建单例的实例
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj);
                        }
                    }
                    return LanguageMgr.edit_singleton;
                }
                else
                {
                    if (LanguageMgr.edit_singleton != null)
                    {
                        LanguageMgr.edit_singleton = null;
                    }
                    if (LanguageMgr.play_singleton == null)
                    {
                        object obj;
                        Monitor.Enter(obj = LanguageMgr.s_objectLock);//加锁防止多线程创建单例
                        try
                        {
                            if (LanguageMgr.play_singleton == null)
                            {
                                LanguageMgr.play_singleton = ((default(LanguageMgr) == null) ? Activator.CreateInstance<LanguageMgr>() : default(LanguageMgr));//创建单例的实例
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj);
                        }
                    }
                    return LanguageMgr.play_singleton;
                }
#else
                if (LanguageMgr.play_singleton == null)
                    {
                        object obj;
                        Monitor.Enter(obj = LanguageMgr.s_objectLock);//加锁防止多线程创建单例
                        try
                        {
                            if (LanguageMgr.play_singleton == null)
                            {
                                LanguageMgr.play_singleton = ((default(LanguageMgr) == null) ? Activator.CreateInstance<LanguageMgr>() : default(LanguageMgr));//创建单例的实例
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj);
                        }
                    }
                    return LanguageMgr.play_singleton;

#endif
            }
        }

        public static string languageAssetName = "language";
        private string curLanguage = "CHS";
        private string[] _langs;
        private List<string> languageNames;
        private Dictionary<string, Dictionary<string, string>> languageDict;

        public LanguageMgr()
        {
            _langs = new string[] { "CHS" };
            languageNames = new List<string>();
            languageDict = new Dictionary<string, Dictionary<string, string>>();
        }

        public void loadLanguage()
        {
            string str = null;
            try
            {
                languageDict.Clear();
                JsonData js = JsonMapper.ToObject(str);
                for (int i = 0; i < js.Count; i++)
                {
                    readSingle(js[i].ToJson());
                }
            }
            catch (Exception e)
            {
            }
        }

        public void delTranslation(string t_key)
        {
            languageNames.Remove(t_key);
            languageDict.Remove(t_key);
        }
        public void release()
        {
            languageNames.Clear();
            languageDict.Clear();
            if (UnityEngine.Application.isPlaying)
            {
                play_singleton = null;
            }
        }

        public string getTranslationValue(string t_key)
        {
            if (t_key == null)
                return null;
            string t_value = null;
            if (!languageDict.ContainsKey(t_key))
                return t_key;
            if (!languageDict[t_key].TryGetValue(curLanguage, out t_value))
            {
                t_value = readTranslationConfig(t_key);
            }
            if (string.IsNullOrEmpty(t_value))
                t_value = t_key;
            return t_value;
        }
        public bool containTranslation(string t_key)
        {
            return languageDict.ContainsKey(t_key);
        }
        public bool containTranslation(string t_key, string t_value)
        {
            if (languageDict.ContainsKey(t_key))
            {
                return languageDict[t_key].ContainsValue(t_value);
            }
            return false;
        }
        public string getTranslationValue(string t_key, string defaultValue)
        {
            if (t_key == null)
                return defaultValue;
            string t_value = null;
            if (!languageDict.ContainsKey(t_key))
                return defaultValue;
            if (!languageDict[t_key].TryGetValue(curLanguage, out t_value))
            {
                t_value = readTranslationConfig(t_key);
            }
            if (string.IsNullOrEmpty(t_value))
                t_value = defaultValue;
            return t_value;
        }
        public string getTranslationValue(string t_key, string lang, string defaultValue)
        {
            if (t_key == null)
                return defaultValue;
            string t_value = null;
            if (!languageDict.ContainsKey(t_key))
                return defaultValue;
            if (!languageDict[t_key].TryGetValue(lang, out t_value))
            {
                t_value = readTranslationConfig(t_key);
            }
            if (string.IsNullOrEmpty(t_value))
                t_value = defaultValue;
            return t_value;
        }

        public void readSingle(string str)
        {
            try
            {
                JsonData jd = JsonMapper.ToObject(str);
                string _na = jd["name"].ToString();
                if (!languageDict.ContainsKey(_na))
                {
                    languageNames.Add(_na);
                    languageDict[_na] = new Dictionary<string, string>();
                }
                if (jd["display"].IsObject)
                {
                    string[] _keys = jd["display"].Keys.ToArray();
                    for (int i = 0; i < _keys.Length; i++)
                    {
                        languageDict[_na][_keys[i]] = jd["display"][_keys[i]].ToString();
                    }
                }
                else
                {
                    languageDict[_na]["CHS"] = jd["display"].ToString();
                }
            }
            catch { }
        }
        private string readTranslationConfig(string t_key)
        {
            string t_value = null;
            string str = ResLibaryMgr.Instance.GetTextAsset(t_key + "_config");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    if (isjson(UnityTool.Decrypt(str)))
                        str = UnityTool.Decrypt(str);
                    JsonData jd = JsonMapper.ToObject(str);
                    string _na = jd["name"].ToString();
                    if (!languageDict.ContainsKey(_na))
                    {
                        languageNames.Add(_na);
                        languageDict[_na] = new Dictionary<string, string>();
                    }
                    if (jd["display"].IsObject)
                    {
                        string[] _keys = jd["display"].Keys.ToArray();
                        for (int i = 0; i < _keys.Length; i++)
                        {
                            languageDict[_na][_keys[i]] = jd["display"][_keys[i]].ToString();
                            if (curLanguage == _keys[i])
                            {
                                t_value = jd["display"][_keys[i]].ToString();
                            }
                        }
                    }
                    else
                    {
                        t_value = jd["display"].ToString();
                        languageDict[_na]["CHS"] = t_value;
                    }
                }
                catch { }
            }
            return t_value;
        }
        public string[] getLanguages()
        {
            return _langs;
        }

        public string[] getLanguageKeys()
        {
            return languageNames.ToArray();
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
