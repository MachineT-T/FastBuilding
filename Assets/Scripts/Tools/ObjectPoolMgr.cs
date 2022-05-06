using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectPoolMgr
{
    private static ObjectPoolMgr s_singleton = default(ObjectPoolMgr);
    private static object s_objectLock = new object();
    public static ObjectPoolMgr Instance
    {
        get
        {
            if (ObjectPoolMgr.s_singleton == null)
            {
                object obj;
                Monitor.Enter(obj = ObjectPoolMgr.s_objectLock);//加锁防止多线程创建单例
                try
                {
                    if (ObjectPoolMgr.s_singleton == null)
                    {
                        ObjectPoolMgr.s_singleton = ((default(ObjectPoolMgr) == null) ? Activator.CreateInstance<ObjectPoolMgr>() : default(ObjectPoolMgr));//创建单例的实例
                    }
                }
                finally
                {
                    Monitor.Exit(obj);
                }
            }
            return ObjectPoolMgr.s_singleton;
        }
    }

    public static int MaxPoolCount = 5000;
    private static int ObjectPoolCount = 0;
    private Dictionary<string, object> ObjectPoolDic;
    public ObjectPoolMgr()
    {
        ObjectPoolDic = new Dictionary<string, object>();
    }

    public T NewObject<T>(VR_ChuangKe.Share.Map.BCWAction<T>.GBCWObject LoadAction)
    {
        string typename = typeof(T).Name;
        if (!ObjectPoolDic.ContainsKey(typename))
            ObjectPoolDic.Add(typename, new ObjectPoolBehavior<T>());
        ObjectPoolBehavior<T> OPB = (ObjectPoolBehavior<T>)ObjectPoolDic[typename];
        return OPB.NewObject(LoadAction);
    }
    public T NewObject<T>(string _type, VR_ChuangKe.Share.Map.BCWAction<T>.GBCWObject LoadAction)
    {
        string typename = typeof(T).Name;
        if (!ObjectPoolDic.ContainsKey(typename))
            ObjectPoolDic.Add(typename, new ObjectPoolBehavior<T>());
        ObjectPoolBehavior<T> OPB = (ObjectPoolBehavior<T>)ObjectPoolDic[typename];
        return OPB.NewObject(_type, LoadAction);
    }

    public bool Store<T>(T obj)
    {
        string typename = typeof(T).Name;
        if (!ObjectPoolDic.ContainsKey(typename))
            ObjectPoolDic.Add(typename, new ObjectPoolBehavior<T>());
        ObjectPoolBehavior<T> OPB = (ObjectPoolBehavior<T>)ObjectPoolDic[typename];
        return OPB.Store(obj);

    }
    public bool Store<T>(string _type, T obj)
    {
        string typename = typeof(T).Name;
        if (!ObjectPoolDic.ContainsKey(typename))
            ObjectPoolDic.Add(typename, new ObjectPoolBehavior<T>());
        ObjectPoolBehavior<T> OPB = (ObjectPoolBehavior<T>)ObjectPoolDic[typename];
        return OPB.Store(_type, obj);

    }

    private class ObjectPoolBehavior<T>
    {
        private object lock_obj;
        private List<T> nomalPoolStack;
        private Dictionary<string, List<T>> assortedPoolStack;
        public ObjectPoolBehavior()
        {
            lock_obj = new object();
            nomalPoolStack = new List<T>();
            assortedPoolStack = new Dictionary<string, List<T>>();
        }

        public T NewObject(VR_ChuangKe.Share.Map.BCWAction<T>.GBCWObject LoadAction)
        {
            if (nomalPoolStack.Count > 0)
            {
                T t = nomalPoolStack[0];
                lock (lock_obj)
                    nomalPoolStack.RemoveAt(0);
                ObjectPoolMgr.ObjectPoolCount--;
                //if(((object)t) == null)
                //    t = LoadAction();
                return t;
            }
            else
            {
                if (LoadAction != null)
                {
                    T t = LoadAction();
                    return t;
                }
            }
            throw new NotImplementedException("对象是空的!");
        }
        public T NewObject(string _type, VR_ChuangKe.Share.Map.BCWAction<T>.GBCWObject LoadAction)
        {
            List<T> sp = null;
            assortedPoolStack.TryGetValue(_type, out sp);
            if (sp != null && sp.Count > 0)
            {
                ObjectPoolMgr.ObjectPoolCount--;
                T t = sp[0];
                lock (lock_obj)
                    sp.RemoveAt(0);
                //if (((object)t) == null)
                //    t = LoadAction();
                return t;
            }
            else
            {
                if (LoadAction != null)
                {
                    T t = LoadAction();
                    return t;
                }
            }
            throw new NotImplementedException("对象是空的!");
        }

        public bool Store(T obj)
        {
            if (!nomalPoolStack.Contains(obj))
            {
                if (ObjectPoolCount < 0 && ObjectPoolCount < MaxPoolCount)
                {
                    lock (lock_obj)
                        nomalPoolStack.Add(obj);
                    ObjectPoolMgr.ObjectPoolCount++;
                    return true;
                }
            }
            return false;
        }
        public bool Store(string _type, T obj)
        {
            if (!assortedPoolStack.ContainsKey(_type))
            {
                lock (lock_obj)
                    assortedPoolStack[_type] = new List<T>();
            }
            if (!assortedPoolStack[_type].Contains(obj))
            {

                if (ObjectPoolCount < 0 && ObjectPoolCount < MaxPoolCount)
                {
                    lock (lock_obj)
                        assortedPoolStack[_type].Add(obj);
                    ObjectPoolMgr.ObjectPoolCount++;
                    return true;
                }
            }
            return false;
        }
    }
}