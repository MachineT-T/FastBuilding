using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Frame.View
{
    public class UIMaskMgr
    {
        private Dictionary<string, GameObject> dicMask;

        public UIMaskMgr()
        {
            dicMask = new Dictionary<string, GameObject>();
        }

        public void TurnOffUIMask(UIBase uIBase)
        {
            GameObject mask;
            string canvasName = uIBase.CurCanvas;
            mask = LoadUIMaskObj(canvasName, uIBase.Container.transform);
            if (mask == null)
                return;
            mask.transform.SetParent(uIBase.Container.transform);
            mask.transform.SetAsFirstSibling();
            mask.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置UI遮罩
        /// </summary>
        /// <param name="uIBase">面板</param>
        public void TurnOnUIMask(UIBase uIBase)
        {

            GameObject mask;
            string canvasName = uIBase.CurCanvas;
            mask = LoadUIMaskObj(canvasName, uIBase.Container.transform);
            if (mask == null)
                return;
            mask.gameObject.SetActive(true);
            CanvasGroup uimask = mask.GetComponent<CanvasGroup>();
            switch (uIBase.uiFormType.UIForm_LucencyType)
            {
                case UIFormLucenyType.Lucency:
                    uimask.alpha = 0;
                    uimask.blocksRaycasts = true;
                    break;
                case UIFormLucenyType.Translucence:
                    uimask.alpha = 0.5f;
                    uimask.blocksRaycasts = true;
                    break;
                case UIFormLucenyType.ImPenetrable:
                    uimask.alpha = 0.25f;
                    uimask.blocksRaycasts = true;
                    break;
                case UIFormLucenyType.Pentrate:
                    uimask.alpha = 0;
                    uimask.blocksRaycasts = false;
                    break;
            }
            mask.transform.SetParent(uIBase.Container.transform);
            mask.transform.SetAsFirstSibling();
            Canvas vas = UIFactory.GetParentComponentScript<Canvas>(uIBase.Container.transform);
            if (vas != null)
            {
                RectTransform mrt = mask.GetComponent<RectTransform>();
                RectTransform vrt = vas.gameObject.GetComponent<RectTransform>();
                mrt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vrt.rect.width);
                mrt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vrt.rect.height);
            }
        }

        private GameObject LoadUIMaskObj(string canvasName, Transform ParentObj)
        {
            if (dicMask.ContainsKey(canvasName))
            {
                return dicMask[canvasName];
            }
            else
            {
                GameObject clone = UIFactory.LoadUIMask();
                if (clone == null)
                    return null;
                clone.transform.SetParent(ParentObj);
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localEulerAngles = Vector3.zero;
                clone.transform.localScale = Vector3.one;
                dicMask[canvasName] = clone;
                return clone;
            }
        }

        public void OnUpdate()
        {

        }
        public void OnDispose()
        {

        }


    }
}
