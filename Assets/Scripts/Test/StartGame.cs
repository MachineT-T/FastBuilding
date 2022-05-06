using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VR_ChuangKe.Share.Map;

namespace FastBuild.Test
{
    public class StartGame : MonoBehaviour
    {
        private bool hasInit = false;
        // Start is called before the first frame update
        void Start()
        {
            ResLibaryMgr.Instance.releaseAll();
            ResLibary.ResLibaryConfig.isPlaying = Application.isPlaying;
            ResLibaryMgr.Instance.releaseAll();
            Frame.View.UIFactory.LoadString = ResLibaryMgr.Instance.GetTextAsset;
            Frame.View.UIFactory.LoadPrefab = ResLibaryMgr.Instance.GetObject<GameObject>;
            Frame.View.UIFactory.ReleaseObj = ResLibaryMgr.Instance.releaseObj;

            ResLibary.FileLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
            ResLibary.AssetBundleLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
            ResLibary.AssetsLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
            LibaryAssetSetting assetSet = Resources.Load<LibaryAssetSetting>("AssetLibarySetting");
            LibaryResourceSetting resourceSet = Resources.Load<LibaryResourceSetting>("ResourceLibarySetting");
            LibaryStreamingAssetSetting streamingAssetsSet = Resources.Load<LibaryStreamingAssetSetting>("StreamingAssetLibarySetting");

            LibaryFileSetting fileSetting = new LibaryFileSetting();
            DirectoryInfo appDataDir = new DirectoryInfo(Application.dataPath);
            fileSetting.path = Path.Combine(appDataDir.Parent.FullName, "FileAssets/");
            ResLibaryMgr.Instance.InsertLibrary(new List<object> { assetSet, resourceSet, streamingAssetsSet, fileSetting });
            new GameObject().AddComponent<Loom>();
        }

        private void Update()
        {
            if (!hasInit && ResLibaryMgr.Instance.HasLoadAsset)
            {
                hasInit = true;
                Frame.View.UIFactory.LoadConfigure("UIPrefabConfig");
                TestMediator mediator = new TestMediator();
                mediator.Initialized();
                mediator.Enter();
                enabled = false;
            }
        }
    }
}