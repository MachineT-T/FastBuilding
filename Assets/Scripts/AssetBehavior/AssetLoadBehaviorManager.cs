using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using VR_ChuangKe.Share;
using VR_ChuangKe.Share.Map;

//namespace VR_ChuangKe.Share
//{
public class AssetLoadBehaviorManager : VR_ChuangKe.Share.AssetBehavior.Singleton<AssetLoadBehaviorManager>
{

    private AssetCubeConfig assetCube;
    private AssetCompConfig assetComp;
    private AssetChunkConfig assetChunk;
    private AssetDConfig assetCollider;
    public static string fileLoadPath { get { return Application.streamingAssetsPath + "/VR_ChuangKe_Share"; } }
    public AssetLoadBehaviorManager()
    {
        string _outputPath = fileLoadPath;
        _outputPath = _outputPath.Replace("//", "/");
        _outputPath = _outputPath.Replace("///", "/");
        _outputPath = _outputPath.Replace("\\", "/");
        _outputPath = _outputPath.Replace("\\\\", "/");
        assetCube = new AssetCubeConfig("", _outputPath + "/ConfigCube");
        assetComp = new AssetCompConfig("", _outputPath + "/ConfigComp");
        assetChunk = new AssetChunkConfig("", _outputPath + "/ConfigChunk");
        assetCollider = new AssetDConfig("", _outputPath + "/ConfigCollider");
        //fileAssetLoad.startRead();
    }


    /// <summary>
    /// 加载资源
    /// </summary>
    public void startLoadAsset()
    {
        assetCube.startRead();
        assetComp.startRead();
        assetChunk.startRead();
        assetCollider.startRead();
        new DisplayConfig("MaterialDisplay").startRead();
        new AssetBoneConfig("BoneConfig").startRead();
        new DisplayConfig("BoneType").startRead();
    }






    /// <summary>
    /// 获取地图列表
    /// </summary>
    /// <returns></returns>
    public ChunkAssetObj[] getMapList()
    {
        //return m_ChunkMgr.getInforKeys();
        return assetChunk.listValue();
    }


    /// <summary>
    /// 获取地图显示名称
    /// </summary>
    /// <param name="mapName"></param>
    /// <returns></returns>
    public string getMapDisplayName(string mapName)
    {
        return LanguageMgr.Instance.getTranslationValue(mapName);
    }

    /// <summary>
    /// 获取地图显示图标
    /// </summary>
    /// <param name="mapName"></param>
    /// <returns></returns>
    public Texture getMapDisplayIcon(string mapName)
    {
        return ResLibaryMgr.Instance.GetTexture2d(DisplayIconMgr.Instance.getIcon(mapName));
    }


    public string[] getCubeNames()
    {
        //return m_CubesMgr.getInforKeys();
        return assetCube.listKey();
    }

    /// <summary>
    /// 获取方块列表
    /// </summary>
    /// <returns></returns>
    public CubeAssetObj[] getCubeList()
    {
        //return m_CubesMgr.getInforKeys();
        return assetCube.listValue();
    }

    /// <summary>
    /// 获取方块的显示名称
    /// </summary>
    /// <param name="cubeName"></param>
    /// <returns></returns>
    public string getCubeDisplayName(string cubeName)
    {
        return LanguageMgr.Instance.getTranslationValue(cubeName);
    }


    /// <summary>
    /// 方块的显示图标
    /// </summary>
    /// <param name="cubeName"></param>
    /// <returns></returns>
    public Texture getCubeDisplayIcon(string cubeName)
    {
        return ResLibaryMgr.Instance.GetTexture2d(DisplayIconMgr.Instance.getIcon(cubeName));
    }

    /// <summary>
    /// 获取组件列表
    /// </summary>
    /// <returns></returns>
    public CompAssetObj[] getCompList()
    {
        return assetComp.listValue();

    }
    public string[] getCompNames()
    {
        //return m_CubesMgr.getInforKeys();
        return assetComp.listKey();
    }
    /// <summary>
    /// 组件的显示名称
    /// </summary>
    /// <param name="compName"></param>
    /// <returns></returns>
    public string getCompDisplayName(string compName)
    {
        return LanguageMgr.Instance.getTranslationValue(compName);
    }



    /// <summary>
    /// 组件的显示图标
    /// </summary>
    /// <param name="compName"></param>
    /// <returns></returns>
    public Texture getCompDisplayIcon(string compName)
    {
        return ResLibaryMgr.Instance.GetTexture2d(DisplayIconMgr.Instance.getIcon(compName));
    }




    /// <summary>
    /// 获取碰撞体列表
    /// </summary>
    /// <returns></returns>
    public string[] getColliderList()
    {
        //return m_ColliderMgr.getInforKeys();
        return assetCollider.listIds("resType", "Collider");
    }

    /// <summary>
    /// 获取碰撞体显示名称
    /// </summary>
    /// <param name="colliderName"></param>
    /// <returns></returns>
    public string getColliderDisplayName(string colliderName)
    {
        return LanguageMgr.Instance.getTranslationValue(colliderName);
    }



    /// <summary>
    /// 获取碰撞体显示图标
    /// </summary>
    /// <param name="colliderName"></param>
    /// <returns></returns>
    public Texture getColliderDisplayIcon(string colliderName)
    {
        return ResLibaryMgr.Instance.GetTexture2d(DisplayIconMgr.Instance.getIcon(colliderName));
    }

}
//}