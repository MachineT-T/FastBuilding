using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VR_ChuangKe.Share.Map;

public class FastBuildingStart : MonoBehaviour
{
    public Slider slider;
    public GameObject canvas;
    public int state;
    public float prograss;
    private Transform ObjectPoolParent;
    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        //事件相关注册        
        VR_ChuangKe.Share.Map.MapDefine.DECRYPT_CONFIGURATION = UnityTool.Decrypt;
        VR_ChuangKe.Share.Map.MapDefine.ENCRYPT_CONFIGURATION = UnityTool.Encrypt;
        VR_ChuangKe.Share.Map.MapDefine.registerCallback = EventListener.registerEvent;
        VR_ChuangKe.Share.Map.MapDefine.deleteCallback = (v1, v2) => { EventListener.deleteEvent(v1, v2); };
        VR_ChuangKe.Share.Map.MapDefine.dispatchCallback = EventListener.dispatchEvent;

        //对象池初始化
        VR_ChuangKe.Share.Map.MapDefine.MAX_CREATE_GAMEOBJECT_TASK_NUM = () => { return 20; };
        VR_ChuangKe.Share.Map.MapDefine.ENABLE_OBJECT_POOL = () => { return true; };  //是否开启对象池
        ObjectPoolParent = new GameObject("ObjectPoolParent").transform;
        DontDestroyOnLoad(ObjectPoolParent);
        ObjectPoolParent.gameObject.SetActive(false);
        VR_ChuangKe.Share.Map.MapDefine.OBJECT_POOL_PARENT = () => { return ObjectPoolParent; };//对象池
        RegistorObjectPool<GameObject>();
        RegistorObjectPool<VR_ChuangKe.Share.Map.Comp.BoneBehavior>();
        RegistorObjectPool<VR_ChuangKe.Share.Map.Comp.ModelBehavior>();
        RegistorObjectPool<VR_ChuangKe.Share.Map.CubeRender>();
        RegistorObjectPool<VR_ChuangKe.Share.Map.Chunk.ChunkRender>();
        RegistorObjectPool<Chunk>();
        RegistorObjectPool<CompBehavior>();
        RegistorObjectPool<AnimBehavior>();
        RegistorObjectPool<VR_ChuangKe.Share.Map.Comp.IShowModel>();
        RegistorObjectPool<VR_ChuangKe.Share.Map.Chunk.Ctrl.IChunkAction>();

        //资源初始化
        ResLibaryMgr.Instance.releaseAll();
        ResLibary.ResLibaryConfig.isPlaying = Application.isPlaying;
        ResLibaryMgr.Instance.releaseAll();
        ResLibary.FileLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
        ResLibary.AssetBundleLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
        ResLibary.AssetsLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
        LibaryAssetSetting assetSet = Resources.Load<LibaryAssetSetting>("AssetLibarySetting");
        LibaryResourceSetting libaryResourceSetting = Resources.Load<LibaryResourceSetting>("ResourceLibarySetting");//Resource下的资源
        LibaryStreamingAssetSetting streamingAssetsSet = Resources.Load<LibaryStreamingAssetSetting>("StreamingAssetLibarySetting");
        LibaryFileSetting libaryFileSetting = new LibaryFileSetting() { path = Application.streamingAssetsPath + "/VR_ChuangKe_Share" };
        ResLibaryMgr.Instance.InsertLibrary(new List<object> { assetSet, libaryResourceSetting, streamingAssetsSet, libaryFileSetting });
    }

    // Update is called once per frame
    void Update()
    {
        //资源初始化完成
        if (state == 0 && ResLibaryMgr.Instance.HasLoadAsset)
        {
            state = 1;
        }
        else if (state == 1)
        {
            state = 2;
            //初始化UI框架
            Frame.View.UIFactory.LoadString = ResLibaryMgr.Instance.GetTextAsset;
            Frame.View.UIFactory.LoadPrefab = ResLibaryMgr.Instance.GetObject<GameObject>;
            Frame.View.UIFactory.ReleaseObj = ResLibaryMgr.Instance.releaseObj;
            Frame.View.UIFactory.LoadConfigure("UIPrefabConfig");
            AssetLoadBehaviorManager.Instance.startLoadAsset();
            new GameObject().AddComponent<Loom>();//挂载Loom
            GameObject map = new GameObject("map");

            Destroy(canvas);
            prograss = 1;
            state = 3;

            //正式进入快速搭建流程
            Instantiate(Resources.Load<GameObject>("UIPrefabs/CubeSelectView"));
        }
        //刷新进度
        if (state != 3)
        {
            prograss = ResLibaryMgr.Instance.InitPrograss;
            slider.value = prograss;
        }

        //流程管理类刷新
        if (state == 3) { Frame.Ctrl.MediatorManager.Instance.Update(); }
    }

    //注册对象池
    private void RegistorObjectPool<T>()
    {
        VR_ChuangKe.Share.Map.MapDefine.RegistorObjectPoolAction<T>((p) =>
        {
            return ObjectPoolMgr.Instance.NewObject(() => { return p == null ? default(T) : p(); });
        });

        VR_ChuangKe.Share.Map.MapDefine.RegistorObjectPoolAction<T>((v, p) =>
        {
            return ObjectPoolMgr.Instance.NewObject(v, () => { return p == null ? default(T) : p(); });
        });

        VR_ChuangKe.Share.Map.MapDefine.RegistorObjectPoolAction<T>((p) =>
        {
            return ObjectPoolMgr.Instance.Store(p);
        });

        VR_ChuangKe.Share.Map.MapDefine.RegistorObjectPoolAction<T>((v, p) =>
        {
            return ObjectPoolMgr.Instance.Store(v, p);
        });

    }
}
