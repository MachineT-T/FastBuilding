
using System;
using System.Collections.Generic;
using System.Threading;

namespace Frame.Model
{
    public class ParamObject
    {
        private string typeName;
        private abstract class VartableBase
        {
            public abstract void Release();
        }
        private class Vartable<T> : VartableBase
        {
            public T Value;
            public override void Release()
            {
                Value = default(T);
            }
        }

        private static object lockobj = new object();
        private static Dictionary<string, List<ParamObject>> cacheDic = new Dictionary<string, List<ParamObject>>();
        public bool HasRecycle { get; private set; }
        private VartableBase data;
        private ParamObject()
        {
            HasRecycle = false;
        }

        public void SetObject<T>(T t)
        {
            if (HasRecycle)
            {
                throw new System.NotImplementedException("数据已经回收");
            }
            Type type = typeof(T);
            string typeName = type.FullName;
            if (this.typeName == typeName)
            {
                Vartable<T> vb = (Vartable<T>)data;
                vb.Value = t;
            }
            else
            {
                throw new System.NotImplementedException("类型不一致");
            }
        }

        public T Peek<T>()
        {
            if (HasRecycle)
            {
                throw new System.NotImplementedException("数据已经回收");
            }
            Vartable<T> vb = (Vartable<T>)data;
            T t = vb.Value;
            return t;
        }

        public void Release()
        {
            if (!HasRecycle)
            {
                data.Release();
                if (!cacheDic.ContainsKey(typeName))
                {
                    lock (lockobj)
                    {
                        cacheDic.Add(typeName, new List<ParamObject>());
                    }
                }
                List<ParamObject> paramObjects = cacheDic[typeName];
                if (!paramObjects.Contains(this))
                {
                    lock (lockobj)
                    {
                        paramObjects.Add(this);
                    }
                }
                HasRecycle = true;
            }
        }

        public static ParamObject Push<T>(T value)
        {
            Type type = typeof(T);
            string typeName = type.FullName;
            if (!cacheDic.ContainsKey(typeName))
            {
                lock (lockobj)
                {
                    cacheDic.Add(typeName, new List<ParamObject>());
                }
            }
            List<ParamObject> paramObjects = cacheDic[typeName];
            ParamObject paramObject = null;
            Vartable<T> vb;
            if (paramObjects.Count > 0)
            {
                paramObject = paramObjects[0];
                vb = (Vartable<T>)paramObject.data;
                lock (lockobj)
                {
                    paramObjects.RemoveAt(0);
                }
            }
            else
            {
                vb = new Vartable<T>();
                paramObject = new ParamObject();
                paramObject.data = vb;

            }
            paramObject.typeName = typeName;
            paramObject.HasRecycle = false;
            vb.Value = value;
            return paramObject;
        }
    }
    public class ParamArray
    {
        private string typeName;
        private abstract class VartableBase
        {
            public abstract void Release();
        }
        private class Vartable<T> : VartableBase
        {
            public T[] Value;
            public override void Release()
            {
                Value = null;
            }
        }

        private static object lockobj = new object();
        private static Dictionary<string, List<ParamArray>> cacheDic = new Dictionary<string, List<ParamArray>>();
        public bool HasRecycle { get; private set; }
        private VartableBase data;
        private ParamArray()
        {
            HasRecycle = false;
        }
        public void SetObject<T>(T[] t)
        {
            if (HasRecycle)
            {
                throw new System.NotImplementedException("数据已经回收");
            }
            Type type = typeof(T);
            string typeName = type.FullName;
            if (this.typeName == typeName)
            {
                Vartable<T> vb = (Vartable<T>)data;
                vb.Value = t;
            }
            else
            {
                throw new System.NotImplementedException("类型不一致");
            }
        }

        public T[] Peek<T>()
        {
            if (HasRecycle)
            {
                throw new System.NotImplementedException("数据已经回收");
            }
            Vartable<T> vb = (Vartable<T>)data;
            T[] t = vb.Value;
            return t;
        }
        public static ParamArray Push<T>(T[] value)
        {
            Type type = typeof(T);
            string typeName = type.FullName;
            if (!cacheDic.ContainsKey(typeName))
            {
                lock (lockobj)
                {
                    cacheDic.Add(typeName, new List<ParamArray>());
                }
            }
            List<ParamArray> paramObjects = cacheDic[typeName];
            ParamArray paramObject = null;
            Vartable<T> vb;
            if (paramObjects.Count > 0)
            {
                paramObject = paramObjects[0];
                vb = (Vartable<T>)paramObject.data;
                lock (lockobj)
                {
                    paramObjects.RemoveAt(0);
                }
            }
            else
            {
                vb = new Vartable<T>();
                paramObject = new ParamArray();
                paramObject.data = vb;

            }
            paramObject.typeName = typeName;
            paramObject.HasRecycle = false;
            vb.Value = value;
            return paramObject;
        }

        public void Release()
        {
            if (!HasRecycle)
            {
                data.Release();
                if (!cacheDic.ContainsKey(typeName))
                {
                    lock (lockobj)
                    {
                        cacheDic.Add(typeName, new List<ParamArray>());
                    }
                }
                List<ParamArray> paramObjects = cacheDic[typeName];
                if (!paramObjects.Contains(this))
                {
                    lock (lockobj)
                    {
                        paramObjects.Add(this);
                    }
                }
                HasRecycle = true;
            }
        }
    }

}

