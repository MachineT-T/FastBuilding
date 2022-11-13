using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VR_ChuangKe.Share
{
    public class ChunkAssetObj
    {
        public string name { get; private set; }
        public string type { get; private set; }
        public string size { get; private set; }
        public ChunkAssetObj(string name, string type, string size)
        {
            this.name = name;
            this.type = type;
            this.size = size;
        }
    }
    public class CubeAssetObj
    {
        public string name { get; private set; }
        public string type { get; private set; }
        public CubeAssetObj(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
    public class CompAssetObj
    {
        public string name { get; private set; }
        public string type { get; private set; }
        public CompAssetObj(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
    public class AssetDConfig : DisplayConfig
    {
        private Dictionary<string, JsonData> _idDict;
        private Dictionary<string, Dictionary<string, HashSet<string>>> _itemDict;

        public AssetDConfig(string file) : base(file)
        {
            _idDict = new Dictionary<string, JsonData>();
            _itemDict = new Dictionary<string, Dictionary<string, HashSet<string>>>();
        }

        public AssetDConfig(string file, string sourcePath) : base(file, sourcePath)
        {
            _idDict = new Dictionary<string, JsonData>();
            _itemDict = new Dictionary<string, Dictionary<string, HashSet<string>>>();
        }

        protected override void readSingle(string content)
        {
            string dstr = UnityTool.Decrypt(content);
            if (isjson(dstr))
            {
                content = dstr;
            }
            base.readSingle(content);
            JsonData dpo = null;
            try { dpo = JsonMapper.ToObject(content); }
            catch { }
            if (dpo == null)
                return;

            string name = null;
            if (dpo.Keys.Contains("name") && dpo["name"] != null) { name = dpo["name"].ToString(); }
            if (name != null)
            {
                _idDict[name] = dpo;
                List<string> keys = new List<string>(dpo.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    if (string.IsNullOrEmpty(keys[i]) || keys[i] == "name" || keys[i] == "icon" || keys[i] == "display" || keys[i] == "states" || keys[i] == "anims")
                        continue;
                    if (!_itemDict.ContainsKey(keys[i]))
                        _itemDict[keys[i]] = new Dictionary<string, HashSet<string>>();
                    if (!_itemDict[keys[i]].ContainsKey(dpo[keys[i]].ToString()))
                        _itemDict[keys[i]][dpo[keys[i]].ToString()] = new HashSet<string>();
                    _itemDict[keys[i]][dpo[keys[i]].ToString()].Add(name);
                }
            }
        }

        /// <summary>
        /// 通过id获取Key值
        /// </summary>
        /// <param name="kid">id</param>
        /// <param name="kname">key名</param>
        /// <returns></returns>
        public string getKeyValue(string kid, string kname)
        {
            if (_idDict.ContainsKey(kname))
            {
                if (_idDict[kname].ContainsKey(kname))
                {
                    return _idDict[kname][kname].ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取或有key名列表
        /// </summary>
        /// <returns></returns>
        public string[] listKey()
        {
            return new List<string>(_itemDict.Keys).ToArray();
        }

        //public string[] listIds(string kname)
        //{
        //    if (string.IsNullOrEmpty(kname) || !_itemDict.ContainsKey(kname))
        //        return new string[0];
        //    return _itemDict[kname][kvalue].ToArray();
        //}

        /// <summary>
        /// 获取id列表
        /// </summary>
        /// <param name="kname">key名</param>
        /// <param name="kvalue">key值</param>
        /// <returns></returns>
        public string[] listIds(string kname, string kvalue)
        {
            if (string.IsNullOrEmpty(kname) || !_itemDict.ContainsKey(kname) || kvalue == null || !_itemDict[kname].ContainsKey(kvalue))
                return new string[0];
            return _itemDict[kname][kvalue].ToArray();
        }

        /// <summary>
        /// 获取key值列表
        /// </summary>
        /// <param name="kname"></param>
        /// <returns></returns>
        public string[] listValue(string kname)
        {
            if (string.IsNullOrEmpty(kname) || !_itemDict.ContainsKey(kname))
                return new string[0];
            List<string> list = new List<string>(_itemDict[kname].Keys);
            list.Sort();
            return list.ToArray();
        }


        //public string[] listSize()
        //{
        //    if (itemDict.Count == 0)
        //        return new string[0];
        //    return itemDict.Keys.ToArray();
        //}

        //public string[] listKey(string size)
        //{
        //    if (size == null || !itemDict.ContainsKey(size))
        //        return new string[0];
        //    return itemDict[size].ToArray();
        //}
    }
    public class AssetCubeConfig : DisplayConfig
    {
        private List<string> ids;
        private List<CubeAssetObj> values;
        private Dictionary<string, int> valueDic;
        public AssetCubeConfig(string file) : base(file)
        {
            ids = new List<string>();
            values = new List<CubeAssetObj>();
            valueDic = new Dictionary<string, int>();
        }

        public AssetCubeConfig(string file, string sourcePath) : base(file, sourcePath)
        {
            ids = new List<string>();
            values = new List<CubeAssetObj>();
            valueDic = new Dictionary<string, int>();
        }

        protected override void readSingle(string content)
        {
            string dstr = UnityTool.Decrypt(content);
            if (isjson(dstr))
            {
                content = dstr;
            }
            base.readSingle(content);
            JsonData dpo = null;
            try
            {
                dpo = JsonMapper.ToObject(content);
                string name = dpo["name"].ToString();
                string type = dpo["type"].ToString();
                if (!valueDic.ContainsKey(name))
                {
                    ids.Add(name);
                    values.Add(new CubeAssetObj(name, type));
                    valueDic.Add(name, ids.Count);
                }
            }
            catch { }
        }



        /// <summary>
        /// 获取或有key名列表
        /// </summary>
        /// <returns></returns>
        public string[] listKey()
        {
            return ids.ToArray();
        }

        public CubeAssetObj[] listValue()
        {
            return values.ToArray();
        }


    }
    public class AssetCompConfig : DisplayConfig
    {
        private List<string> ids;
        private List<CompAssetObj> values;
        private Dictionary<string, int> valueDic;
        public AssetCompConfig(string file) : base(file)
        {
            ids = new List<string>();
            values = new List<CompAssetObj>();
            valueDic = new Dictionary<string, int>();
        }

        public AssetCompConfig(string file, string sourcePath) : base(file, sourcePath)
        {
            ids = new List<string>();
            values = new List<CompAssetObj>();
            valueDic = new Dictionary<string, int>();
        }

        protected override void readSingle(string content)
        {
            string dstr = UnityTool.Decrypt(content);
            if (isjson(dstr))
            {
                content = dstr;
            }
            base.readSingle(content);
            JsonData dpo = null;
            try
            {
                dpo = JsonMapper.ToObject(content);
                string name = dpo["name"].ToString();
                string type = dpo["type"].ToString();
                if (!valueDic.ContainsKey(name))
                {
                    ids.Add(name);
                    values.Add(new CompAssetObj(name, type));
                    valueDic.Add(name, ids.Count);
                }
                if (dpo.Keys.Contains("states"))
                {
                    for (int i = 0; i < dpo["states"].Count; i++)
                    {
                        base.readSingle(dpo["states"][i].ToJson());
                    }
                }
            }
            catch { }
        }



        /// <summary>
        /// 获取或有key名列表
        /// </summary>
        /// <returns></returns>
        public string[] listKey()
        {
            return ids.ToArray();
        }

        public CompAssetObj[] listValue()
        {
            return values.ToArray();
        }


    }
    public class AssetChunkConfig : DisplayConfig
    {
        private List<string> ids;
        private List<ChunkAssetObj> values;
        private Dictionary<string, int> valueDic;
        public AssetChunkConfig(string file) : base(file)
        {
            ids = new List<string>();
            values = new List<ChunkAssetObj>();
            valueDic = new Dictionary<string, int>();
        }

        public AssetChunkConfig(string file, string sourcePath) : base(file, sourcePath)
        {
            ids = new List<string>();
            values = new List<ChunkAssetObj>();
            valueDic = new Dictionary<string, int>();
        }

        protected override void readSingle(string content)
        {
            string dstr = UnityTool.Decrypt(content);
            if (isjson(dstr))
            {
                content = dstr;
            }
            base.readSingle(content);
            try
            {
                JsonData dpo = JsonMapper.ToObject(content);
                string name = dpo["name"].ToString();
                string type = dpo["type"].ToString();
                string size = dpo["styp"].ToString();
                if (!valueDic.ContainsKey(name))
                {
                    ids.Add(name);
                    values.Add(new ChunkAssetObj(name, type, size));
                    valueDic.Add(name, ids.Count);
                }
            }
            catch (System.Exception e) { Debug.LogError(e.Message + ":" + e.StackTrace); }
        }



        /// <summary>
        /// 获取或有key名列表
        /// </summary>
        /// <returns></returns>
        public string[] listKey()
        {
            return ids.ToArray();
        }

        public ChunkAssetObj[] listValue()
        {
            return values.ToArray();
        }


    }

    public class AssetBoneConfig : DisplayConfig
    {
        public AssetBoneConfig(string file) : base(file)
        {

        }

        public AssetBoneConfig(string file, string sourcePath) : base(file, sourcePath)
        {

        }

        protected override void readSingle(string content)
        {
            string dstr = UnityTool.Decrypt(content);
            if (isjson(dstr))
            {
                content = dstr;
            }
            base.readSingle(content);
            JsonData dpo = null;
            try { dpo = JsonMapper.ToObject(content); }
            catch { }
            if (dpo == null)
                return;

            string name = null;
            if (dpo.Keys.Contains("name") && dpo["name"] != null) { name = dpo["name"].ToString(); }
            if (name != null && dpo.Keys.Contains("anims"))
            {
                for (int i = 0; i < dpo["anims"].Count; i++)
                {
                    base.readSingle(dpo["anims"][i].ToJson());
                }
            }
        }


    }

}