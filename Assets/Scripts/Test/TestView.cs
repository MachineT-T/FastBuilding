using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VR_ChuangKe.Share.Map.Comp;

namespace FastBuild.Test
{
    public class TestView : Frame.View.UIWindows
    {
        protected override string m_canvasName { get { return "TestCanvas"; } }
        protected override List<string> m_ExcuteMsgs
        {
            get
            {
                if (_emsg == null)
                {
                    _emsg = new List<string>();
                    _emsg.Add("RefrashComp");
                    _emsg.Add("SkinChange");
                    _emsg.Add("SizeChange");
                }
                return _emsg;
            }
        }
        private List<string> _emsg;
        protected override string m_uiFormName { get { return "TestView"; } }
        protected override Frame.View.UIType m_uiFormType
        {
            get
            {
                if (_uiFormType == null)
                {
                    _uiFormType = new Frame.View.UIType();
                    _uiFormType.IsNewCanvas = false;
                    _uiFormType.Depth = 2;
                    _uiFormType.UIForms_ShowMode = Frame.View.UIFormShowMode.Normal;
                    _uiFormType.UIForms_Type = Frame.View.UIFormType.Normal;
                    _uiFormType.UIForm_LucencyType = Frame.View.UIFormLucenyType.Pentrate;
                }
                return _uiFormType;
            }
        }
        private Frame.View.UIType _uiFormType;
        private InputField sizeInput;
        private Dropdown skin;
        private Dropdown bone;
        private bool isWorking;
        protected override void OnInit()
        {
            Container.transform.Find("viewName").GetComponent<Text>().text = m_uiFormName;
            Container.transform.Find("btn_rec").GetComponent<Button>().onClick.AddListener(()=> 
            {
                Frame.View.UIManager.Instance.Excute("REC_TestView_msg",Random.Range(1,10000).ToString());
            });
            Container.transform.Find("btn_EventListener").GetComponent<Button>().onClick.AddListener(() =>
            {
                EventListener.dispatchEvent("EventListenerTest", Random.Range(1, 10000).ToString());
            });
            Container.transform.Find("img_asset").GetComponent<RawImage>().texture = ResLibaryMgr.Instance.GetTexture2d("assetsImg");
            Container.transform.Find("img_resources").GetComponent<RawImage>().texture = ResLibaryMgr.Instance.GetTexture2d("resourcesImg");
            Container.transform.Find("img_streamingAssets").GetComponent<RawImage>().texture = ResLibaryMgr.Instance.GetTexture2d("streamingAssetsImg");
            Container.transform.Find("img_fileAssets").GetComponent<RawImage>().texture = ResLibaryMgr.Instance.GetTexture2d("fileAssetsImg");
            skin = Container.transform.Find("skinDrop").GetComponent<Dropdown>();
            bone = Container.transform.Find("boneDrop").GetComponent<Dropdown>();
            sizeInput = Container.transform.Find("InputField").GetComponent<InputField>();
            sizeInput.onEndEdit.AddListener(arg=>
            {
                isWorking = true;
                sizeInput.interactable = false;
                Frame.Ctrl.MediatorManager.Instance.Excute("SizeChange", arg);
            });
        }

        protected override void OnExcute(string msg, object[] body)
        {
            if (msg == "RefrashComp")
            {
                skin.onValueChanged.RemoveAllListeners();
                skin.options.Clear();
                List<string> skins = (List<string>)body[0];
                for (int i = 0; i < skins.Count; i++)
                {
                    skin.options.Add(new Dropdown.OptionData(skins[i]));
                }
                skin.value = 0;
                skin.onValueChanged.AddListener(OnSkinDropValueChange);
            }
            else if (msg == "SkinChange")
            {
                bone.options.Clear();
                SkeletonData sd = (SkeletonData)body[0]; ;
                for (int j = 0; j < sd.BoneNodeList.Count; j++)
                {
                    bone.options.Add(new Dropdown.OptionData(sd.BoneNodeList[j].mNodeName));
                }
            }
            else if (msg == "SizeChange")
            {
                isWorking = false;
                sizeInput.interactable = true;
            }
        }

        private void OnSkinDropValueChange(int arg)
        {
            Frame.Ctrl.MediatorManager.Instance.Excute("SkinChange", skin.options[arg].text);
        }

        protected override void OnRelease(bool isRecycle)
        {
            ResLibaryMgr.Instance.releaseObj(LibaryTypeEnum.LibaryType_Texture2D, "assetsImg");
            ResLibaryMgr.Instance.releaseObj(LibaryTypeEnum.LibaryType_Texture2D, "resourcesImg");
            ResLibaryMgr.Instance.releaseObj(LibaryTypeEnum.LibaryType_Texture2D, "streamingAssetsImg");
            ResLibaryMgr.Instance.releaseObj(LibaryTypeEnum.LibaryType_Texture2D, "fileAssetsImg");

        }
    }
}
