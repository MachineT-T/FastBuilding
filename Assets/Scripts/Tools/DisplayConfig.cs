using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.IO;
using ResLibary;
using VR_ChuangKe.Share.Map;

namespace VR_ChuangKe.Share
{
    public class DisplayConfig
    {
        protected string fileName;
        protected string sourcePath;
        protected List<string> names;
        private Dictionary<string, string> _idConfigureDict;
        public DisplayConfig(string fileName)
        {
            this.fileName = fileName;
            this.names = new List<string>();
        }

        public DisplayConfig(string fileName, string sourcePath)
        {
            this.fileName = fileName;
            this.sourcePath = sourcePath;
            this.names = new List<string>();
        }

        public void startRead()
        {
            readFile();
            readDirectory();
        }

        protected virtual void readFile()
        {
            string fr = FileTool.LoadFileStr(fileName);
            string content = string.IsNullOrEmpty(fr)?ResLibaryMgr.Instance.GetTextAsset(fileName): fr;
            if (string.IsNullOrEmpty(content))
            {
               if(!isjson(fileName))
                    return;
                content = fileName;
            }
            try
            {
                string dstr = UnityTool.Decrypt(content);
                if (isjson(dstr))
                {
                    content = dstr;
                }
                JsonData jd = JsonMapper.ToObject(content);
                for (int i = 0; i < jd.Count; i++)
                {
                    readSingle(jd[i].ToJson());
                }
            }
            catch (Exception e)
            {
                //Debug.LogError(fileName + ":" + e.Message + ":" + e.StackTrace);
            }
        }

        protected virtual void readDirectory()
        {
            if (string.IsNullOrEmpty(sourcePath))
                return;
            DirectoryInfo sDir = new DirectoryInfo(sourcePath);
            if (!sDir.Exists)
            {
                //Debug.LogError(sourcePath + " is null");
                return;
            }

            FileInfo[] fis = sDir.GetFiles();
            for (int j = 0; j < fis.Length; j++)
            {
                string extension = fis[j].Extension.ToLower();
                if (extension == ".txt" || extension == ".json")
                {
                    string fn = fis[j].Name.Replace(fis[j].Extension, "");
                    int cIndex = fn.LastIndexOf("_config");
                    if (cIndex >= 0)
                    {
                        string str = FileTool.LoadFileStr(fis[j].FullName);
                        readSingle(str);
                        
                        string content = ResLibaryMgr.Instance.GetTextAsset(fn);
                        if (string.IsNullOrEmpty(content))
                            FileLibary.Instance.UpdateLibary(fis[j].FullName, AssetExistStatusEnum.Scene);
                    }
                }
            }

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

        protected virtual void readSingle(string content)
        {
            JsonData dpo = null;
            try
            {
                dpo = JsonMapper.ToObject(content);
            }
            catch 
            { 
                Debug.LogError(this.GetType().Name + ":" + content);
            }
            if (dpo == null)
                return;
            string name = null;
            string icon = null;
            bool fdisplay = dpo.Keys.Contains("display") && dpo["display"] != null;

            if (dpo.Keys.Contains("name") && dpo["name"] != null) { name = dpo["name"].ToString(); }
            if (dpo.Keys.Contains("icon") && dpo["icon"] != null) { icon = dpo["icon"].ToString(); }

            if (name != null)
            {
                if (!names.Contains(name))
                    names.Add(name);
                if (icon != null)
                    DisplayIconMgr.Instance.updateDisplayIcon(name, icon);
                if (fdisplay)
                    LanguageMgr.Instance.readSingle(content);
            }
        }

        public string[] getInforKeys()
        {
            return names.ToArray();
        }

    }
}