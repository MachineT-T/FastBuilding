using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VR_ChuangKe.Share
{
    public class CompConfig : DisplayConfig
    {

        private Dictionary<string, HashSet<string>> itemDict;

        public CompConfig(string file) : base(file)
        {
            itemDict = new Dictionary<string, HashSet<string>>();
        }

        public CompConfig(string file, string sourcePath) : base(file, sourcePath)
        {
            itemDict = new Dictionary<string, HashSet<string>>();
        }

        protected override void readSingle(string content)
        {
            base.readSingle(content);
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
            string type = null;
            if (dpo.Keys.Contains("name") && dpo["name"] != null) { name = dpo["name"].ToString(); }
            if (dpo.Keys.Contains("type") && dpo["type"] != null) { type = dpo["type"].ToString(); }
            if (name != null)
            {
                if (!string.IsNullOrEmpty(type))
                {
                    if (!itemDict.ContainsKey(type))
                        itemDict[type] = new HashSet<string>();
                    itemDict[type].Add(name);
                    
                }
            }


        }

        public string[] listType()
        {
            if (itemDict.Count == 0)
                return new string[0];
            return itemDict.Keys.ToArray();
        }

        public string[] listKey(string type)
        {
            if (type == null || !itemDict.ContainsKey(type))
                return new string[0];
            return itemDict[type].ToArray();
        }
    }
}