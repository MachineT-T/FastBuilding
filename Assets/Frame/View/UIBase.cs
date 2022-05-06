using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Frame.View
{
    public abstract class UIBase : MonoBehaviour
    {
        public bool isInit { get; private set; }
        public bool isOpen { get; private set; }
        public bool isFreeze { get; private set; }
        public string canvasName { get { return m_canvasName; } }
        public UIType uiFormType { get { return m_uiFormType; } }
        public string uiFormName { get { return m_uiFormName; } }
        protected abstract string m_canvasName { get; }
        protected abstract string m_uiFormName { get; }
        protected abstract UIType m_uiFormType { get; }
        public GameObject Container
        {
            get
            {
                if (m_Container == null)
                {
                    if (uiFormType.IsNewCanvas)
                    {
                        int ct = 0;
                        string str = "";
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            Transform t = transform.GetChild(i);
                            if (t.name != "Normal" || t.name != "Fixed" || t.name != "PopUp")
                            {
                                if (ct > 0 && ct < transform.childCount)
                                    str = string.Format("{0}#", str);
                                str = string.Format("{0}{1}", str, i.ToString());
                                ct++;
                            }
                        }
                        string[] arrct = str.Split('#');
                        if (ct == 0 || ct > 1)
                        {
                            GameObject go = new GameObject("Container");
                            go.transform.SetParent(transform);
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localRotation = Quaternion.identity;
                            go.transform.localScale = Vector3.one;
                            RectTransform rt = go.GetComponent<RectTransform>();
                            if (rt == null)
                                rt = go.AddComponent<RectTransform>();
                            rt.anchorMin = new Vector2(0, 0);
                            rt.anchorMax = new Vector2(1, 1);
                            rt.offsetMax = new Vector2(0, 0);
                            rt.offsetMin = new Vector2(0, 0);
                            if (ct > 1)
                            {
                                for (int i = 0; i < arrct.Length; i++)
                                {
                                    Transform t = transform.GetChild(int.Parse(arrct[i]));
                                    t.SetParent(rt);
                                }
                            }
                        }
                        else
                        {
                            Transform t = transform.GetChild(int.Parse(arrct[0]));
                            m_Container = t.gameObject;
                        }
                    }
                    else
                    {
                        m_Container = gameObject;
                    }
                }
                return m_Container;
            }
        }

        public List<string> excuteMsgs { get { return m_ExcuteMsgs; } }
        protected abstract List<string> m_ExcuteMsgs { get; }
        [SerializeField]
        private GameObject m_Container;

        public CanvasGroup canvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = Container.GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                        _canvasGroup = Container.AddComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        private CanvasGroup _canvasGroup;
        public string CurCanvas { get; set; }
        private List<BaseGraphic> baseGraphics;
        private IHandleUIManager handleUI;
        public void InitView(IHandleUIManager handleUI)
        {
            if (!isInit)
            {
                isInit = true;
                this.handleUI = handleUI;
                baseGraphics = new List<BaseGraphic>();
                Container.SetActive(false);
                BaseGraphic[] baseGraphic = gameObject.GetComponents<BaseGraphic>();
                if (baseGraphic != null)
                {
                    for (int i = 0; i < baseGraphic.Length; i++)
                    {
                        baseGraphics.Add(baseGraphic[i]);
                    }
                }
                GetChildComp<BaseGraphic>(Container.transform, ref baseGraphics);
                BaseGraphic[] graphics = baseGraphics.ToArray();
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].Init();
                }
                OnInit();

            }
        }

        public void Excute(string msg, object[] body)
        {
            if (isInit)
                OnExcute(msg, body);
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Display()
        {
            isOpen = true;
            isFreeze = false;
            Container.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            OnDisplay();

        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hiding()
        {
            isOpen = false;
            isFreeze = false;
            Container.SetActive(false);
            OnHide();

        }

        /// <summary>
        /// 冻结
        /// </summary>
        public void Freeze()
        {
            isFreeze = true;
            Container.SetActive(true);
            canvasGroup.interactable = false;
            OnFreese();

        }

        /// <summary>
        /// 再显示
        /// </summary>
        public void ReDisplay()
        {
            isFreeze = false;
            Container.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            OnReDisplay();
        }

        public void ReleaseView()
        {
            if (isInit) 
            {
                isFreeze = false;
                isOpen = false;
                isInit = false;
                Container.SetActive(false);
                BaseGraphic[] graphics = baseGraphics.ToArray();
                for (int i = 0; i < graphics.Length; i++)
                {
                    if (graphics[i] != null)
                        graphics[i].Release();
                }
                baseGraphics.Clear();
                OnRelease(true);
                handleUI.UnRegisterExcute(m_uiFormName, excuteMsgs.ToArray());
                handleUI.RemoveUIFormToCatch(m_uiFormName);
                UIFactory.ReleaseForm(m_uiFormName);
            }
            //Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (isInit)
            {
                isFreeze = false;
                isOpen = false;
                isInit = false;
                BaseGraphic[] graphics = baseGraphics.ToArray();
                for (int i = 0; i < graphics.Length; i++)
                {
                    if (graphics[i] != null)
                        graphics[i].Release();
                }
                baseGraphics.Clear();
                OnRelease(false);
                handleUI.UnRegisterExcute(m_uiFormName, excuteMsgs.ToArray());
                handleUI.RemoveUIFormToCatch(m_uiFormName);
                UIFactory.ReleaseForm(m_uiFormName);
            }
        }

        protected abstract void OnInit();

        protected abstract void OnExcute(string msg, object[] body);

        protected virtual void OnDisplay() { }

        protected virtual void OnHide() { }

        protected virtual void OnReDisplay() { }

        protected virtual void OnFreese() { }

        protected virtual void OnRelease(bool isRecycle) { }

        private void GetChildComp<T>(Transform Trans, ref List<T> list) where T : Component
        {
            for (int i = 0; i < Trans.childCount; i++)
            {
                T[] ts = Trans.GetChild(i).GetComponents<T>();
                if (ts.Length > 0)
                {
                    for (int j = 0; j < ts.Length; j++)
                    {
                        T t = ts[j];
                        if (!list.Contains(t))
                            list.Add(t);
                    }
                }
                GetChildComp<T>(Trans.GetChild(i), ref list);
            }
        }
    }
}