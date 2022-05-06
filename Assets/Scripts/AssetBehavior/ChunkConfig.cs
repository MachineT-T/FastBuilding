using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VR_ChuangKe.Share
{
    public class ChunkConfig : DisplayConfig
    {
        private Dictionary<string, HashSet<string>> itemDict;

        public ChunkConfig(string file) : base(file)
        {
            itemDict = new Dictionary<string, HashSet<string>>();
        }

        public ChunkConfig(string file, string sourcePath) : base(file, sourcePath)
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
                
            }
            if (dpo == null)
                return;

            string name = null;
            string size = null;
            if (dpo.Keys.Contains("name") && dpo["name"] != null) { name = dpo["name"].ToString(); }
            if (dpo.Keys.Contains("size") && dpo["size"] != null) { size = dpo["size"].ToString(); }

            if (name != null)
            {
                if (size != null)
                {
                    if (!itemDict.ContainsKey(size))
                        itemDict[size] = new HashSet<string>();
                    itemDict[size].Add(name);
                }              
            }
        }

        public string[] listSize()
        {
            if (itemDict.Count == 0)
                return new string[0];
            return itemDict.Keys.ToArray();
        }

        public string[] listKey(string size)
        {
            if (size == null || !itemDict.ContainsKey(size))
                return new string[0];
            return itemDict[size].ToArray();
        }
    }
}