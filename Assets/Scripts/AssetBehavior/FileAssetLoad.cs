using System.Collections;
using System.Collections.Generic;
using System.IO;
using ResLibary;
using UnityEngine;

public class FileAssetLoad
{
    /// <summary>
    /// 文件夹路径
    /// </summary>
    public string dPath;
    public FileAssetLoad(string dpath)
    {
        dPath = dpath;
    }

    /// <summary>
    /// 开始读取
    /// </summary>
    public void startRead()
    {
        
        readDiredtory(dPath);
    }


    private void readDiredtory(string dirPath)
    {
        if (string.IsNullOrEmpty(dirPath))
            return;
        DirectoryInfo dir = new DirectoryInfo(dirPath);
        if (!dir.Exists || dir.Name == ".svn")
            return;
        FileInfo[] fs = dir.GetFiles();
        DirectoryInfo[] dirs = dir.GetDirectories();

        for (int i = 0; i < fs.Length; i++)
        {
            //string fn = fs[i].Name.Replace(fs[i].Extension, "");
            //string content = ResLibaryMgr.Instance.GetTextAsset(fn);
            //if (string.IsNullOrEmpty(content))
            FileLibary.Instance.UpdateLibary(fs[i].FullName, AssetExistStatusEnum.Scene);
        }

        for (int i = 0; i < dirs.Length; i++)
        {
            readDiredtory(dirs[i].FullName);
        }
    }

}
