using System.Collections;
using System.Collections.Generic;
using Frame.View;
using UnityEngine;
using UnityEngine.UI;
namespace FastBuild.Test
{
    public class TestCanvas : Frame.View.UIWindows
    {
        protected override string m_canvasName { get { return ""; } }
        protected override List<string> m_ExcuteMsgs
        {
            get
            {
                if (_emsg == null)
                {
                    _emsg = new List<string>();
                    _emsg.Add("REC_TestView_msg");
                }
                return _emsg;
            }
        }
        private List<string> _emsg;
        protected override string m_uiFormName { get { return "TestCanvas"; } }
        protected override Frame.View.UIType m_uiFormType
        {
            get
            {
                if (_uiFormType == null)
                {
                    _uiFormType = new Frame.View.UIType();
                    _uiFormType.IsNewCanvas = true;
                    _uiFormType.UIForms_ShowMode = Frame.View.UIFormShowMode.Normal;
                    _uiFormType.UIForms_Type = Frame.View.UIFormType.Normal;
                    _uiFormType.UIForm_LucencyType = Frame.View.UIFormLucenyType.Pentrate;
                }
                return _uiFormType;
            }
        }
        private Frame.View.UIType _uiFormType;
        private Text txt;
        protected override void OnExcute(string msg, object[] body)
        {
            if (msg == "REC_TestView_msg")
            {
                txt.text = "收到消息:" + (string)body[0];
            }
        }

        protected override void OnInit()
        {
            txt = transform.Find("Text").GetComponent<Text>();
            txt.text = "";
            transform.Find("canvasName").GetComponent<Text>().text = m_uiFormName;
            EventListener.registerEvent("EventListenerTest", EventListenerTest);
        }

        protected override void OnRelease(bool isRecycle)
        {
            EventListener.deleteEvent("EventListenerTest", EventListenerTest);
        }

        private void EventListenerTest(object data)
        {
            transform.Find("EventListener").GetComponent<Text>().text = "EventListener:" + (string)data;
        }
    }
}
