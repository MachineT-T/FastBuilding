using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frame.Ctrl
{
    public abstract class BaseMediator
    {
        public bool IsInitialized { get { return m_IsInitialized; } }
        private bool m_IsInitialized;

        public bool IsEnered { get { return m_IsEnered; } }
        private bool m_IsEnered;

        public bool IsWorking { get { return m_IsWorking; } }
        private bool m_IsWorking;
        /// <summary>
        /// 是否启用更新
        /// </summary>
        public virtual bool IsUpdate { get { return false; } }

        /// <summary>
        /// 节点类型
        /// </summary>
        public virtual string NodeType { get { return ""; } }

        public abstract List<string> ListExcute { get; }
        public abstract List<string> ListTryGetValue { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Initialized(params object[] data)
        {
            if (!m_IsInitialized)
            {
                m_IsInitialized = true;
               
                MediatorManager.Instance.RegstorExcute(this);
                MediatorManager.Instance.RegstorTryGetValue(this);
                OnInitialized(data);
            }
        }

        /// <summary>
        /// 进入
        /// </summary>
        public void Enter(params object[] data)
        {
            if (!m_IsEnered && m_IsInitialized)
            {
                m_IsEnered = true;
                m_IsWorking = true;
                MediatorManager.Instance.RegstorEnter(this);
                if (IsUpdate) MediatorManager.Instance.RegstorUpdate(this);
                OnEnter(data);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update() { if (m_IsWorking && m_IsEnered) OnUpdate(); }

        /// <summary>
        /// 冻结
        /// </summary>
        public void Freeze()
        {
            if (m_IsEnered)
            {
                m_IsWorking = false;
                if (IsUpdate) MediatorManager.Instance.UnRegstorUpdate(this);
                OnFreeze();
            }
        }

        /// <summary>
        /// 取消冻结
        /// </summary>
        public void UnFreeze()
        {
            if (m_IsEnered)
            {
                m_IsWorking = true;
                if (IsUpdate) MediatorManager.Instance.RegstorUpdate(this);
                OnUnFreeze();
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void End()
        {
            if (m_IsEnered)
            {
                m_IsEnered = false;
                m_IsWorking = false;
                if (IsUpdate) MediatorManager.Instance.UnRegstorUpdate(this);
                MediatorManager.Instance.UnRegstorEnter(this);
                OnEnd();
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            if (m_IsInitialized)
            {
                m_IsEnered = false;
                m_IsInitialized = false;
                m_IsWorking = false;
                if (IsUpdate) MediatorManager.Instance.UnRegstorUpdate(this);
                MediatorManager.Instance.UnRegstorExcute(this);
                MediatorManager.Instance.UnRegstorTryGetValue(this);
                MediatorManager.Instance.UnRegstorEnter(this);
                OnRelease();
            }
        }


        public void Excute(string msg, object[] body) { if (m_IsInitialized) OnExcute(msg, body); }
        public bool TryGetValue(string msg, string key, out object body)
        {
            if (m_IsInitialized)
                return OnTryGetValue(msg, key, out body);
            body = null;
            return false;
        }
        public T GetValue<T>(string msg, string key)
        {
            if (m_IsInitialized)
                return OnGetValue<T>(msg, key);
            return default(T);
        }
        public bool TryGetValue<T>(string msg, string key, out T body)
        {
            if (m_IsInitialized)
                return OnTryGetValue<T>(msg, key, out body);
            body = default(T); 
            return false;
        }
        protected abstract void OnInitialized(object[] data);
        protected abstract void OnEnter(object[] data);
        protected virtual void OnUpdate() { }
        protected virtual void OnFreeze() { }
        protected virtual void OnUnFreeze() { }
        protected abstract void OnEnd();
        protected abstract void OnRelease();
        protected abstract void OnExcute(string msg, object[] body);

        protected virtual T OnGetValue<T>(string msg, string key)
        {
            return default(T);
        }
        protected virtual bool OnTryGetValue(string msg, string key, out object body)
        {
            body = null;
            return false;
        }
        protected virtual bool OnTryGetValue<T>(string msg, string key, out T body)
        {
            body = default(T);
            return false;
        }
    }
}