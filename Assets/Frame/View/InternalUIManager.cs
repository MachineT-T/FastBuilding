using UnityEngine;
using System.Collections.Generic;
using System;

namespace Frame.View
{
    public interface IHandleUIManager
    {
        void InitView();
        void Excute(string msg, object[] body);
        void ReleaseForm(string uIFormName);
        void ReleaseView();
        void AddUIFormToCatch(UIBase uIBase);
        void RemoveUIFormToCatch(string uIFormName);
        void CloseAllUIForm();

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="uIFormName"></param>
        void LoadUIFormSync(string uIFormName);

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="uIFormName"></param>
        /// <param name="callback"></param>
        void LoadUIFormAsyn(string uIFormName, Action<string> callback);
        void ShowUIForm(string uIFormName);
        void ShowUIForm(string uIFormName, string msg, object[] body);
        void CloseUIForm(string uIFormName);
        void RegisterExcute(string uiforname, string[] exctes);
        void UnRegisterExcute(string uiforname, string[] exctes);
        //void OnExitScene(string scene);
        //void OnEnterScene(string scene);
    }

    internal class InternalUIManager : IHandleUIManager
    {
        private class ExcuteQuote
        {
            public int quote;
            public List<string> forms;

            public ExcuteQuote() { quote = 0; forms = new List<string>(); }
        }
        private UIMaskMgr uIMaskMgr;

        private Dictionary<string, List<UIBase>> _stackUIForm;
        private Dictionary<string, List<UIBase>> _dicCurUIForm;
        private Dictionary<string, UIBase> _dicAllUIForm;
        private Dictionary<string, ExcuteQuote> _excuteDic;
        private Dictionary<UIFormType, Dictionary<string, Dictionary<int, GameObject>>> _depthDic;
        void IHandleUIManager.InitView()
        {
            _stackUIForm = new Dictionary<string, List<UIBase>>();
            _dicCurUIForm = new Dictionary<string, List<UIBase>>();
            _dicAllUIForm = new Dictionary<string, UIBase>();
            _excuteDic = new Dictionary<string, ExcuteQuote>();
            _depthDic = new Dictionary<UIFormType, Dictionary<string, Dictionary<int, GameObject>>>();
            _depthDic[UIFormType.Normal] = new Dictionary<string, Dictionary<int, GameObject>>();
            _depthDic[UIFormType.Fixed] = new Dictionary<string, Dictionary<int, GameObject>>();
            _depthDic[UIFormType.PopUp] = new Dictionary<string, Dictionary<int, GameObject>>();
            uIMaskMgr = new UIMaskMgr();
        }

        void IHandleUIManager.ReleaseForm(string uIFormName)
        {
            uIFormName = uIFormName == null ? "" : uIFormName;
            ((IHandleUIManager)this).CloseUIForm(uIFormName);
            UIBase uIBase = null;
            _dicAllUIForm.TryGetValue(uIFormName, out uIBase);
            if (uIBase != null)
            {
                uIBase.ReleaseView();
            }
        }


        void IHandleUIManager.LoadUIFormSync(string uIFormName)
        {
            UIBase _UIBase = LoadUIBase(uIFormName);
            if (_UIBase == null)
            {
                Debug.LogError("UIBase is null");

            }
        }
        void IHandleUIManager.LoadUIFormAsyn(string uIFormName, Action<string> callback)
        {
            UIBase _UIBase = LoadUIBase(uIFormName);
            if (_UIBase == null)
            {
                Debug.LogError("UIBase is null");
                if (callback != null)
                {
                    callback("UIBase is null");
                }
                return;
            }
            if (callback != null)
            {
                callback(null);
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="uIFormName"></param>
        void IHandleUIManager.ShowUIForm(string uIFormName)
        {
            UIBase _UIBase = LoadUIBase(uIFormName);
            if (_UIBase == null)
            {
                Debug.LogError("UIBase is null");
                return;
            }

            switch (_UIBase.uiFormType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    if (_dicCurUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                    {
                        return;
                    }
                    _dicCurUIForm[_UIBase.CurCanvas].Add(_UIBase);
                    break;
                case UIFormShowMode.ReverseChange:
                    if (_stackUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                    {
                        return;
                    }
                    else
                    {
                        if (_stackUIForm[_UIBase.CurCanvas].Count > 0)
                        {
                            UIBase topUIForm = _stackUIForm[_UIBase.CurCanvas][_stackUIForm[_UIBase.CurCanvas].Count - 1];
                            //栈顶元素作冻结处理
                            topUIForm.Freeze();
                        }
                        _stackUIForm[_UIBase.CurCanvas].Add(_UIBase);
                    }
                    break;
                case UIFormShowMode.HideOther:
                    {
                        if (_dicCurUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                        {
                            return;
                        }
                        foreach (var stack in _stackUIForm[_UIBase.CurCanvas])
                        {
                            if (stack.canvasName == _UIBase.canvasName)
                            {
                                stack.Hiding();
                            }
                        }
                        foreach (var dic in _dicCurUIForm[_UIBase.CurCanvas])
                        {
                            if (dic.canvasName == _UIBase.canvasName)
                            {
                                dic.Hiding();
                            }
                        }
                        _dicCurUIForm[_UIBase.CurCanvas].Add(_UIBase);

                    }
                    break;
            }
            if ((_dicCurUIForm[_UIBase.CurCanvas].Count == 1 && _stackUIForm[_UIBase.CurCanvas].Count == 0) ||
               (_dicCurUIForm[_UIBase.CurCanvas].Count == 0 && _stackUIForm[_UIBase.CurCanvas].Count == 1))
            {
                uIMaskMgr.TurnOffUIMask(_UIBase);
            }
            else
            {
                uIMaskMgr.TurnOnUIMask(_UIBase);
            }
            //SetSblingIndex(_UIBase);
            _UIBase.Container.transform.SetAsLastSibling();
            _UIBase.Display();
        }
        void IHandleUIManager.ShowUIForm(string uIFormName, string msg, object[] body)
        {
            UIBase _UIBase = LoadUIBase(uIFormName);
            if (_UIBase == null)
            {
                Debug.LogError("UIBase is null");
                ((IHandleUIManager)this).Excute(msg, body);
                return;
            }

            switch (_UIBase.uiFormType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    if (_dicCurUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                    {
                        ((IHandleUIManager)this).Excute(msg, body);
                        return;
                    }
                    _dicCurUIForm[_UIBase.CurCanvas].Add(_UIBase);
                    break;
                case UIFormShowMode.ReverseChange:
                    if (_stackUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                    {
                        ((IHandleUIManager)this).Excute(msg, body);
                        return;
                    }
                    else
                    {
                        if (_stackUIForm[_UIBase.CurCanvas].Count > 0)
                        {
                            UIBase topUIForm = _stackUIForm[_UIBase.CurCanvas][_stackUIForm[_UIBase.CurCanvas].Count - 1];
                            //栈顶元素作冻结处理
                            topUIForm.Freeze();
                        }
                        _stackUIForm[_UIBase.CurCanvas].Add(_UIBase);
                    }
                    break;
                case UIFormShowMode.HideOther:
                    {
                        if (_dicCurUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                        {
                            ((IHandleUIManager)this).Excute(msg, body);
                            return;
                        }
                        foreach (var stack in _stackUIForm[_UIBase.CurCanvas])
                        {
                            if (stack.canvasName == _UIBase.canvasName)
                            {
                                stack.Hiding();
                            }
                        }
                        foreach (var dic in _dicCurUIForm[_UIBase.CurCanvas])
                        {
                            if (dic.canvasName == _UIBase.canvasName)
                            {
                                dic.Hiding();
                            }
                        }
                        _dicCurUIForm[_UIBase.CurCanvas].Add(_UIBase);

                    }
                    break;
            }
            if ((_dicCurUIForm[_UIBase.CurCanvas].Count == 1 && _stackUIForm[_UIBase.CurCanvas].Count == 0) ||
               (_dicCurUIForm[_UIBase.CurCanvas].Count == 0 && _stackUIForm[_UIBase.CurCanvas].Count == 1))
            {
                uIMaskMgr.TurnOffUIMask(_UIBase);
            }
            else
            {
                uIMaskMgr.TurnOnUIMask(_UIBase);
            }
            //SetSblingIndex(_UIBase);
            _UIBase.Container.transform.SetAsLastSibling();
            _UIBase.Display();
            ((IHandleUIManager)this).Excute(msg, body);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="uIFormName"></param>
        void IHandleUIManager.CloseUIForm(string uIFormName)
        {
            if (string.IsNullOrEmpty(uIFormName) || !_dicAllUIForm.ContainsKey(uIFormName))
                return;
            UIBase _UIBase = null;
            //_dicAllUIForm.TryGetValue(uIFormName,out _UIBase);
            UIBase jS_UIBase = _dicAllUIForm[uIFormName];
            if (jS_UIBase == null)
                return;
            if (!jS_UIBase.isOpen)
                return;
            switch (jS_UIBase.uiFormType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    {
                        _UIBase = jS_UIBase;
                        _UIBase.Container.transform.SetAsFirstSibling();
                        _UIBase.Hiding();
                        if (!_dicCurUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                            return;
                        _dicCurUIForm[_UIBase.CurCanvas].Remove(_UIBase);

                    }
                    break;
                case UIFormShowMode.ReverseChange:
                    {
                        if (_stackUIForm[jS_UIBase.CurCanvas].Count >= 2)
                        {
                            //出栈处理
                            _UIBase = _stackUIForm[jS_UIBase.CurCanvas][_stackUIForm[jS_UIBase.CurCanvas].Count - 1];
                            _stackUIForm[jS_UIBase.CurCanvas].RemoveAt(_stackUIForm[jS_UIBase.CurCanvas].Count - 1);
                            //做隐藏处理
                            _UIBase.Hiding();
                            //出栈后，下一个窗体做“重新显示”处理。
                            UIBase nextUIForms = _stackUIForm[jS_UIBase.CurCanvas][_stackUIForm[jS_UIBase.CurCanvas].Count - 1];
                            nextUIForms.ReDisplay();
                        }
                        else if (_stackUIForm[jS_UIBase.CurCanvas].Count == 1)
                        {
                            //出栈处理
                            _UIBase = _stackUIForm[jS_UIBase.CurCanvas][_stackUIForm[jS_UIBase.CurCanvas].Count - 1];
                            _stackUIForm[jS_UIBase.CurCanvas].RemoveAt(_stackUIForm[jS_UIBase.CurCanvas].Count - 1);
                            //做隐藏处理
                            _UIBase.Container.transform.SetAsFirstSibling();
                            _UIBase.Hiding();
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                case UIFormShowMode.HideOther:
                    _UIBase = jS_UIBase;
                    _UIBase.Hiding();
                    _UIBase.transform.SetAsFirstSibling();
                    if (!_dicCurUIForm[_UIBase.CurCanvas].Contains(_UIBase))
                        return;
                    _dicCurUIForm[_UIBase.CurCanvas].Remove(_UIBase);
                    foreach (var stack in _stackUIForm[_UIBase.CurCanvas])
                    {
                        stack.ReDisplay();
                    }
                    foreach (var dic in _dicCurUIForm[_UIBase.CurCanvas])
                    {
                        dic.ReDisplay();
                    }
                    break;
            }
            UIBase _preUIBase = null;
            if (_stackUIForm[_UIBase.CurCanvas].Count > 0)
            {
                _preUIBase = _stackUIForm[_UIBase.CurCanvas][_stackUIForm[_UIBase.CurCanvas].Count - 1];
            }
            else
            {
                if (_dicCurUIForm[_UIBase.CurCanvas].Count > 0)
                    _preUIBase = _dicCurUIForm[_UIBase.CurCanvas][_dicCurUIForm[_UIBase.CurCanvas].Count - 1];
            }
            if (_preUIBase != null)
            {
                uIMaskMgr.TurnOnUIMask(_preUIBase);
                _preUIBase.Container.transform.SetAsLastSibling();
                //SetSblingIndex(_preUIBase);
            }
            if ((_dicCurUIForm[_UIBase.CurCanvas].Count == 1 && _stackUIForm[_UIBase.CurCanvas].Count == 0) ||
               (_dicCurUIForm[_UIBase.CurCanvas].Count == 0 && _stackUIForm[_UIBase.CurCanvas].Count == 1))
            {
                uIMaskMgr.TurnOffUIMask(_UIBase);
            }
        }

        void IHandleUIManager.Excute(string msg, object[] body)
        {
            if (_excuteDic.ContainsKey(msg))
            {
                string[] forms = _excuteDic[msg].forms.ToArray();
                for (int i = 0; i < forms.Length; i++)
                {
                    string uiFormName = forms[i];
                    if (_dicAllUIForm.ContainsKey(uiFormName))
                    {
                        UIBase jS_UIBase = _dicAllUIForm[uiFormName];
                        jS_UIBase.Excute(msg, body);
                    }
                }
            }
        }

        void IHandleUIManager.AddUIFormToCatch(UIBase uIBase)
        {
            if (!_dicAllUIForm.ContainsKey(uIBase.uiFormName))
            {
                _dicAllUIForm[uIBase.uiFormName] = uIBase;
                UIFormSetParent(uIBase);
            }
        }

        void IHandleUIManager.RemoveUIFormToCatch(string uIFormName)
        {
            UIBase uiBase = null;
            _dicAllUIForm.TryGetValue(uIFormName, out uiBase);

            _dicAllUIForm.Remove(uIFormName);

            if (uiBase == null)
            {
                return;
            }
            if (_dicCurUIForm[uiBase.CurCanvas].Contains(uiBase))
            {
                _dicCurUIForm[uiBase.CurCanvas].Remove(uiBase);
            }
            if (uiBase.uiFormType.UIForms_ShowMode == UIFormShowMode.ReverseChange)
            {
                _stackUIForm[uiBase.CurCanvas].Remove(uiBase);
            }
            uiBase.ReleaseView();
        }

        void IHandleUIManager.CloseAllUIForm()
        {
            foreach (var stacks in _stackUIForm.Values)
            {
                foreach (var stack in stacks)
                {
                    if (stack != null)
                    {
                        stack.Hiding();
                    }
                }
                stacks.Clear();
            }
            foreach (var _dicCur in _dicCurUIForm.Values)
            {
                foreach (var dic in _dicCur)
                {
                    if (dic != null)
                    {
                        dic.Hiding();
                    }
                }
                _dicCur.Clear();
            }
        }

        //private void SetSblingIndex(UIBase uibase)
        //{

        //    int psubIndex = uibase.Container.transform.GetSiblingIndex();
        //    int msubIndex = 0;

        //    if (_dicCurUIForm.ContainsKey(uibase.CurCanvas))
        //    {
        //        UIBase[] cuis = _dicCurUIForm[uibase.CurCanvas].ToArray();
        //        for (int i = 0; i < cuis.Length; i++)
        //        {
        //            if (cuis[i].uiFormType.UIForms_Type == uibase.uiFormType.UIForms_Type &&
        //                cuis[i].uiFormType.Depth <= uibase.uiFormType.Depth)
        //            {
        //                if (msubIndex < cuis[i].Container.transform.GetSiblingIndex())
        //                {
        //                    msubIndex = cuis[i].Container.transform.GetSiblingIndex();
        //                }
        //            }
        //        }
        //    }
        //    if (_stackUIForm.ContainsKey(uibase.CurCanvas))
        //    {
        //        UIBase[] suis = _stackUIForm[uibase.CurCanvas].ToArray();
        //        for (int i = 0; i < suis.Length; i++)
        //        {
        //            if (suis[i].uiFormType.UIForms_Type == uibase.uiFormType.UIForms_Type &&
        //                suis[i].uiFormType.Depth <= uibase.uiFormType.Depth)
        //            {
        //                if (msubIndex < suis[i].Container.transform.GetSiblingIndex())
        //                {
        //                    msubIndex = suis[i].Container.transform.GetSiblingIndex();
        //                }
        //            }
        //        }
        //    }
        //    if (psubIndex < msubIndex)
        //    {
        //        uibase.Container.transform.SetSiblingIndex(msubIndex);
        //    }
        //    else if (psubIndex > msubIndex)
        //    {
        //        uibase.Container.transform.SetSiblingIndex(msubIndex + 1);
        //    }
        //}

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="uIFormName"></param>
        /// <returns></returns>
        private UIBase LoadUIBase(string uIFormName)
        {
            if (string.IsNullOrEmpty(uIFormName))
            {
                Debug.LogError("名称是空的");
                return null;
            }
            UIBase uiBase = null;
            _dicAllUIForm.TryGetValue(uIFormName, out uiBase);
            if (uiBase == null)
            {
                UIBase ruiBase = LoadResUIBase(uIFormName);
                if (ruiBase != null)
                {
                    UIFormSetParent(ruiBase);
                    UIFactory.OnLoadExcuteKey(ruiBase);
                    _dicAllUIForm[uIFormName] = ruiBase;
                    ((IHandleUIManager)this).RegisterExcute(uIFormName, ruiBase.excuteMsgs.ToArray());
                    ruiBase.InitView((IHandleUIManager)this);
                }
                uiBase = ruiBase;
            }
            else
            {
                UIFormSetParent(uiBase);
            }

            return uiBase;
        }

        private UIBase LoadResUIBase(string uIFormName)
        {
            GameObject go = UIFactory.LoadUIPrefab(uIFormName);
            if (go == null)
            {
                Debug.LogError("预设是空的:" + uIFormName);
                return null;
            }
            UIBase uiBase = go.GetComponent<UIBase>();
            if (uiBase == null)
            {
                Debug.LogError("预设是空的" + uIFormName);
                //uiBase.InitView();
                //_dicAllUIForm[uIFormName] = uiBase;
            }
            //else
            //{
            //    Debug.LogError("预设是空的");

            //}
            return uiBase;
        }

        private Transform LoadCanvasRoot()
        {
            GameObject croot = GameObject.Find("Canvas");
            if (croot == null)
            {
                GameObject _croot = Resources.Load<GameObject>("Canvas");
                if (_croot != null)
                {
                    croot = GameObject.Instantiate(_croot);
                    return croot.transform;
                }
                else
                {
                    Debug.Log("Canvas is null!");
                }
            }
            return croot.transform;
        }
        void IHandleUIManager.RegisterExcute(string uiFormName, string[] msgs)
        {
            for (int i = 0; i < msgs.Length; i++)
            {
                if (!_excuteDic.ContainsKey(msgs[i]))
                    _excuteDic[msgs[i]] = new ExcuteQuote();
                ExcuteQuote eq = _excuteDic[msgs[i]];
                if (!eq.forms.Contains(msgs[i]))
                {
                    eq.quote++;
                    eq.forms.Add(uiFormName);
                }

            }
        }
        void IHandleUIManager.UnRegisterExcute(string uiFormName, string[] msgs)
        {
            for (int i = 0; i < msgs.Length; i++)
            {

                if (_excuteDic.ContainsKey(msgs[i]))
                {
                    ExcuteQuote eq = _excuteDic[msgs[i]];
                    if (eq.forms.Contains(msgs[i]))
                    {
                        eq.quote--;
                        eq.forms.Remove(uiFormName);
                    }
                }

            }
        }

        private void UIFormSetParent(UIBase uiBase)
        {
            GameObject go = uiBase.Container;
            Transform CanvasT = null;

            if (uiBase.uiFormType.IsNewCanvas)
            {
                CanvasT = uiBase.transform;
                uiBase.CurCanvas = uiBase.uiFormName;
            }
            else
            {
                string canvasName = string.IsNullOrEmpty(uiBase.canvasName) ? UIFactory.DEFAULT_CANVAS : uiBase.canvasName;
                UIBase pui = LoadUIBase(canvasName);
                if (pui == null)
                {
                    Debug.LogError(canvasName + ":父对象不存在:" + uiBase.uiFormName);
                    return;
                }
                uiBase.CurCanvas = canvasName;
                CanvasT = pui.transform;
            }

            if (!_stackUIForm.ContainsKey(uiBase.CurCanvas))
                _stackUIForm[uiBase.CurCanvas] = new List<UIBase>();
            if (!_dicCurUIForm.ContainsKey(uiBase.CurCanvas))
                _dicCurUIForm[uiBase.CurCanvas] = new List<UIBase>();


            UIFormSetParent(uiBase, CanvasT);
        }

        private void UIFormSetParent(UIBase uiBase, Transform pCan)
        {
            GameObject go = uiBase.Container;
            Transform CanvasT = pCan;
            CheckUIFormTypeTrans(CanvasT);
            go.transform.SetParent(GetParent(uiBase, CanvasT));
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
        }

        private void CheckUIFormTypeTrans(Transform CanvasT)
        {
            Transform NorTran;
            Transform FixTran;
            Transform PopTran;
            NorTran = CanvasT.Find("Normal");
            FixTran = CanvasT.Find("Fixed");
            PopTran = CanvasT.Find("PopUp");

            bool IsSetIndex = false;

            if (NorTran == null)
            {
                GameObject clone = new GameObject("Normal");
                RectTransform rt = clone.GetComponent<RectTransform>();
                if (rt == null)
                    rt = clone.AddComponent<RectTransform>();
                rt.SetParent(CanvasT);
                rt.localPosition = Vector3.zero;
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMax = new Vector2(0, 0);
                rt.offsetMin = new Vector2(0, 0);
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                IsSetIndex = true;
                NorTran = rt;
            }
            if (FixTran == null)
            {
                GameObject clone = new GameObject("Fixed");
                RectTransform rt = clone.GetComponent<RectTransform>();
                if (rt == null)
                    rt = clone.AddComponent<RectTransform>();
                rt.SetParent(CanvasT);
                rt.localPosition = Vector3.zero;
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMax = new Vector2(0, 0);
                rt.offsetMin = new Vector2(0, 0);
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                IsSetIndex = true;
                FixTran = rt;
            }

            if (PopTran == null)
            {
                GameObject clone = new GameObject("PopUp");
                RectTransform rt = clone.GetComponent<RectTransform>();
                if (rt == null)
                    rt = clone.AddComponent<RectTransform>();
                rt.SetParent(CanvasT);
                rt.localPosition = Vector3.zero;
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMax = new Vector2(0, 0);
                rt.offsetMin = new Vector2(0, 0);
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                IsSetIndex = true;
                PopTran = rt;
            }
            if (IsSetIndex)
            {
                NorTran.SetAsLastSibling();
                FixTran.SetAsLastSibling();
                PopTran.SetAsLastSibling();
            }
        }

        private Transform GetParent(UIBase uiBase, Transform CanvasT)
        {
            if (!_depthDic[uiBase.uiFormType.UIForms_Type].ContainsKey(uiBase.CurCanvas))
                _depthDic[uiBase.uiFormType.UIForms_Type][uiBase.CurCanvas] = new Dictionary<int, GameObject>();
            GameObject pto = null;
            _depthDic[uiBase.uiFormType.UIForms_Type][uiBase.CurCanvas].TryGetValue(uiBase.uiFormType.Depth, out pto);
            if (pto == null)
            {
                pto = new GameObject(uiBase.uiFormType.Depth.ToString());
                _depthDic[uiBase.uiFormType.UIForms_Type][uiBase.CurCanvas][uiBase.uiFormType.Depth] = pto;
                Transform pt = null ;
                switch (uiBase.uiFormType.UIForms_Type)
                {
                    case UIFormType.Normal:
                        pt = CanvasT.Find("Normal"); 
                        break;
                    case UIFormType.Fixed:
                        pt = CanvasT.Find("Fixed");
                        break;
                    case UIFormType.PopUp:
                        pt = CanvasT.Find("PopUp");
                        break;

                }
                RectTransform rt = pto.GetComponent<RectTransform>();
                if (rt == null)
                    rt = pto.AddComponent<RectTransform>();
                rt.SetParent(pt);
                rt.localPosition = Vector3.zero;
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMax = new Vector2(0, 0);
                rt.offsetMin = new Vector2(0, 0);
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                List<int> depths = new List<int>(_depthDic[uiBase.uiFormType.UIForms_Type][uiBase.CurCanvas].Keys);
                depths.Sort((x, y) => x.CompareTo(y));
                for (int i = 0; i < depths.Count; i++)
                {
                    _depthDic[uiBase.uiFormType.UIForms_Type][uiBase.CurCanvas][depths[i]].transform.SetAsLastSibling();
                }
            }
            return _depthDic[uiBase.uiFormType.UIForms_Type][uiBase.CurCanvas][uiBase.uiFormType.Depth].transform;
        }

        void IHandleUIManager.ReleaseView()
        {
            ((IHandleUIManager)this).CloseAllUIForm();
            List<string> templist = new List<string>(_dicAllUIForm.Keys);
            foreach (var uiform in templist)
            {
                var uibase = _dicAllUIForm[uiform];
                if (uibase != null)
                    uibase.ReleaseView();
            }
        }

        private T GetParentComponentScript<T>(Transform go) where T : Component
        {
            T t = null;
            if (go.parent != null)
            {
                t = go.parent.GetComponent<T>();
                if (go.root != go.parent && t == null)
                {
                    t = GetParentComponentScript<T>(go.parent);
                }
            }
            return t;
        }

    }
}