using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Frame.View
{
    public abstract class BaseGraphic : MonoBehaviour
    {
        public bool m_IsInit { get; private set; }
        public void Init() {
            if (!m_IsInit)
            {
                OnInit();
                m_IsInit = true;
            }
        }
        public void Release() {
            if (m_IsInit)
            {
                OnRelease();
                m_IsInit = false;
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_IsInit)
            {
                OnRelease();
                m_IsInit = false;
            }
        }

        protected abstract void OnInit();
        protected abstract void OnRelease();
    }

    public struct GraphicAsset
    {
        public string m_Key { get; private set; }
        public string m_AssetName { get; private set; }
        public object m_Default { get; private set; }
        public GraphicAsset(string m_Key, string m_AssetName, object m_Default)
        {
            this.m_Key = m_Key ;
            this.m_AssetName = m_AssetName;
            this.m_Default = m_Default;
        }
    }
}
