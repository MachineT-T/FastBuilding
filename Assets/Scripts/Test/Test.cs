using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VR_ChuangKe.Share;
using VR_ChuangKe.Share.Map;
using VR_ChuangKe.Share.Map.Chunk.Model;

public class Test : MonoBehaviour
{

    public int xSize;
    public int zSize;

    [Header("地图块")]
    public string Ground;
    public string Mountain;
    public string Lake;
    public string Forest;
    public string Desert;

    [Header("柏林噪声参数")]
    public float frequency;//柏林噪声频率
    public float scale;//柏林噪声振幅

    public Button create;

    private Chunk ck;

    private bool hasInit = false;
    private Transform ObjectPoolParent ;
    // Start is called before the first frame update
    void Start()
    {
        #region 初始化以及准备工作

        //资源初始化
        ResLibaryMgr.Instance.releaseAll();
        ResLibary.ResLibaryConfig.isPlaying = Application.isPlaying;
        ResLibaryMgr.Instance.releaseAll();
        ResLibary.FileLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
        ResLibary.AssetBundleLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
        ResLibary.AssetsLibary.UpdateAssetCallback = ResLibaryMgr.InsertAssetToLibary;
        LibaryAssetSetting assetSet = Resources.Load<LibaryAssetSetting>("AssetLibarySetting");
        LibaryStreamingAssetSetting streamingAssetsSet = Resources.Load<LibaryStreamingAssetSetting>("StreamingAssetLibarySetting");
        ResLibaryMgr.Instance.InsertLibrary(new List<object> { assetSet, streamingAssetsSet });


        //事件相关注册        
        VR_ChuangKe.Share.Map.MapDefine.DECRYPT_CONFIGURATION = UnityTool.Decrypt;
        VR_ChuangKe.Share.Map.MapDefine.ENCRYPT_CONFIGURATION = UnityTool.Encrypt;
        VR_ChuangKe.Share.Map.MapDefine.registerCallback = EventListener.registerEvent;
        VR_ChuangKe.Share.Map.MapDefine.deleteCallback = (v1, v2) => { EventListener.deleteEvent(v1, v2); };
        VR_ChuangKe.Share.Map.MapDefine.dispatchCallback = EventListener.dispatchEvent;

        //对象池初始化
        VR_ChuangKe.Share.Map.MapDefine.MAX_CREATE_GAMEOBJECT_TASK_NUM = () => { return 20; };      
        VR_ChuangKe.Share.Map.MapDefine.ENABLE_OBJECT_POOL = () => { return false; };  //是否开启对象池
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

        
        new GameObject().AddComponent<Loom>();//挂载Loom
        GameObject map = new GameObject("map");
        map.AddComponent<CompManager>();//挂载对象管理类
        map.AddComponent<ChunkManager>();//挂载地形管理类

        #endregion
        create.interactable = false;
    }

    private void Update()
    {
        //资源加载完成
        if (!hasInit && ResLibaryMgr.Instance.HasLoadAsset)
        {
            AssetLoadBehaviorManager.Instance.startLoadAsset();
            hasInit = true;
            create.interactable = true;
            enabled = false;
        }
    }

    //案例测试
    public void Init()
    {
        create.interactable = false;
        ChunkElement chunkElement = new ChunkElement();
        chunkElement.SizeX = xSize;
        chunkElement.SizeZ = zSize;
        //柏林噪声伪随机误差
        float xRandom = UnityEngine.Random.Range(0f, 100f);
        float zRandom = UnityEngine.Random.Range(0f, 100f);
        Debug.Log(System.DateTime.Now.ToString());
        int nodeCount = xSize * zSize;
        int nodeIndex = 0;
        long id32 = BitConverter.ToInt64(System.Guid.NewGuid().ToByteArray(), 0);
        System.Action<int> _callback = (arg) =>
        {
            int sind = arg;
            int eind = Mathf.Min(sind + 10000, nodeCount);
            //异步多线程
            Loom.QueueOnAsyncThread(id32,() =>
            {
                for (int i = sind; i < eind; i++)
                {
                    nodeIndex++;

                    int x = nodeIndex % zSize;
                    int z = nodeIndex / zSize;
                    //生成地图二维坐标
                    ChunkCube cc = new ChunkCube();
                    cc.mNodePos = new BWPoint(x, 0, z);
                    //柏林噪声生成地图高度信息
                    float xFloat = x;
                    float zFloat = z;
                    float xSizeFloat = xSize;
                    float zSizeFloat = zSize;
                    float gridHeight = Mathf.PerlinNoise(xFloat / xSizeFloat * frequency + xRandom, zFloat / zSizeFloat * frequency + zRandom) * scale;
                    float gridType = Mathf.PerlinNoise(xFloat / xSizeFloat * frequency + xRandom / 3, zFloat / zSizeFloat * frequency + zRandom / 3) * scale;
                    //地图单位格高度判断并更改类型
                    //生成 高山 平原 湖泊
                    if (gridHeight > 1.5f && gridHeight < 3.6f)
                    {
                        cc.mat_ID = Ground;
                        //生成 森林 沙漠
                        if (gridType >= 3.2f)
                        {
                            cc.mat_ID = Forest;
                            cc.mNodePos.y = gridHeight;
                        }
                        else if (gridType <= 1.5f)
                        {
                            cc.mat_ID = Desert;
                            cc.mNodePos.y = gridHeight;
                        }
                    }
                    else if (gridHeight >= 3.6f)
                    {
                        cc.mat_ID = Mountain;
                        cc.mNodePos.y = gridHeight + 1;
                    }
                    else if (gridHeight <= 1.5f)
                    {
                        cc.mat_ID = Lake;
                        cc.mNodePos.y = 1.4;
                    }
                    chunkElement.cubeNodeList.Add(cc);
                }
                if (nodeIndex == nodeCount)
                {
                    //主线程
                    Loom.QueueOnMainThread(id32,(p) =>
                    {
                        if (ck != null)
                        {
                            Destroy(ck.gameObject);
                        }
                        string chunkID = ChunkCacheAction.createChunkID();

                        ck = ChunkManager.Instance.createChunk(chunkID, chunkElement, (err) =>
                        {
                            create.interactable = true;
                            Debug.Log(System.DateTime.Now.ToString());
                        });
                        if (p != null) p();
                    });
                }
            });
        };

        if (nodeCount / MapDefine.CHUNK_LOAD_PER_TASK_COUNT > 0)
        {
            int taskCount = nodeCount % 10000 == 0 ? nodeCount / 10000 : (nodeCount / 10000) + 1;
            for (int i = 0; i < taskCount; i++)
            {
                _callback(i * 10000);
            }
        }
        else
        {
            _callback(0);
        }

      
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

