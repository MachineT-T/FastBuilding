using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Frame.Ctrl
{
    public class MediatorManager
    {
        private static MediatorManager s_singleton = default(MediatorManager);
        private static object s_objectLock = new object();
        public static MediatorManager Instance
        {
            get
            {
                if (MediatorManager.s_singleton == null)
                {
                    object obj;
                    Monitor.Enter(obj = MediatorManager.s_objectLock);//加锁防止多线程创建单例
                    try
                    {
                        if (MediatorManager.s_singleton == null)
                        {
                            MediatorManager.s_singleton = ((default(MediatorManager) == null) ? Activator.CreateInstance<MediatorManager>() : default(MediatorManager));//创建单例的实例
                        }
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return MediatorManager.s_singleton;
            }
        }

        private List<BaseMediator> enterNodes;
        private List<BaseMediator> updateNodes;
       
        private Dictionary<string, List<BaseMediator>> excuteDic;
        private Dictionary<string, List<BaseMediator>> tryGetValueDic;
        public MediatorManager()
        {
            enterNodes = new List<BaseMediator>();
            updateNodes = new List<BaseMediator>();
            excuteDic = new Dictionary<string, List<BaseMediator>>();
            tryGetValueDic = new Dictionary<string, List<BaseMediator>>();
        }
        public void RegstorExcute(BaseMediator node)
        {
            if (node == null)
                return;
            string[] msgs = node.ListExcute.ToArray();
            for (int i = 0; i < msgs.Length; i++)
            {
                if (!excuteDic.ContainsKey(msgs[i]))
                    excuteDic[msgs[i]] = new List<BaseMediator>();
                if (!excuteDic[msgs[i]].Contains(node))
                    excuteDic[msgs[i]].Add(node);
            }
        }
        public void UnRegstorExcute(BaseMediator node)
        {
            if (node == null)
                return;
            string[] msgs = node.ListExcute.ToArray();
            for (int i = 0; i < msgs.Length; i++)
            {
                if (!excuteDic.ContainsKey(msgs[i]))
                    continue;
                if (excuteDic[msgs[i]].Contains(node))
                    excuteDic[msgs[i]].Remove(node);
            }
        }

        public void UnRegstorTryGetValue(BaseMediator node)
        {
            if (node == null)
                return;
            string[] msgs = node.ListTryGetValue.ToArray();
            for (int i = 0; i < msgs.Length; i++)
            {
                if (!tryGetValueDic.ContainsKey(msgs[i]))
                    continue;
                if (tryGetValueDic[msgs[i]].Contains(node))
                    tryGetValueDic[msgs[i]].Remove(node);
            }
        }
        public void RegstorTryGetValue(BaseMediator node)
        {
            if (node == null)
                return;
            string[] msgs = node.ListTryGetValue.ToArray();
            for (int i = 0; i < msgs.Length; i++)
            {
                if (!tryGetValueDic.ContainsKey(msgs[i]))
                    tryGetValueDic[msgs[i]] = new List<BaseMediator>();
                if (!tryGetValueDic[msgs[i]].Contains(node))
                    tryGetValueDic[msgs[i]].Add(node);
            }
        }

        public void RegstorEnter(BaseMediator node)
        {
            if (node != null && !enterNodes.Contains(node)) enterNodes.Add(node);
        }
        public void UnRegstorEnter(BaseMediator node)
        {
            if (node != null && enterNodes.Contains(node)) enterNodes.Remove(node);
        }
        public void RegstorUpdate(BaseMediator node)
        {
            if (node != null && !updateNodes.Contains(node)) updateNodes.Add(node);
        }
        public void UnRegstorUpdate(BaseMediator node)
        {
            if (node != null && updateNodes.Contains(node)) updateNodes.Remove(node);
        }
        public void Update()
        {
            BaseMediator[] nodes = updateNodes.ToArray();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].Update();
            }
        }

        public void Excute(string msg,params object[] data)
        {
            if (excuteDic.ContainsKey(msg))
            {
                BaseMediator[] nodes = excuteDic[msg].ToArray();
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Excute(msg, data);
                }
            }
        }

        public bool TryGetValue(string msg, string key, out object data)
        {
            if (tryGetValueDic.ContainsKey(msg))
            {
                BaseMediator[] nodes = tryGetValueDic[msg].ToArray();
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].TryGetValue(msg, key, out data))
                    {
                        return true;
                    }
                }
            }
            data = null;
            return false;
        }

        public bool TryGetValue<T>(string msg, string key, out T data)
        {
            if (tryGetValueDic.ContainsKey(msg))
            {
                BaseMediator[] nodes = tryGetValueDic[msg].ToArray();
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].TryGetValue<T>(msg, key, out data))
                    {
                        return true;
                    }
                }
            }
            data = default(T);
            return false;
        }

        public T GetValue<T>(string msg, string key)
        {
            if (tryGetValueDic.ContainsKey(msg))
            {
                BaseMediator[] nodes = tryGetValueDic[msg].ToArray();
                for (int i = 0; i < nodes.Length; i++)
                {
                    return nodes[i].GetValue<T>(msg, key);
                }
            }
            return default(T);
        }
    }
}
