
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Runtime.InteropServices;

public class FileTool
{
    public static void CopyFile(string fromPath, string toPath)
    {
        if (File.Exists(fromPath))
        {
            string str = Path.GetDirectoryName(toPath);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            File.Copy(fromPath, toPath, true);
        }
    }
    public static void WriteLocal(string text, int offset, string url, bool ReWrite = true, System.Action<string> callback = null)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
        WriteLocal(buffer, offset, url, ReWrite, callback);
    }
    public static void WriteLocal(byte[] buffer, int offset, string url, bool ReWrite = true, System.Action<string> callback = null)
    {
        string err = null;
        try
        {
            string str = Path.GetDirectoryName(url);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            bool found = File.Exists(url);
            if (found && ReWrite)
            {
                using (FileStream fs = new FileStream(url, FileMode.Truncate, FileAccess.Write))
                {
                    fs.Flush();
                    //关闭流
                    fs.Close();
                }
            }
            using (FileStream fs = new FileStream(url, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Write))
            {
                if (!ReWrite && found)
                {
                    fs.Seek(0, SeekOrigin.End);
                }
                fs.Write(buffer, offset, buffer.Length);

                fs.Flush();
                //关闭流
                fs.Close();
            }
        }
        catch (Exception e) { err = e.Message + ":" + e.StackTrace; Debug.LogError(err); }
        if (callback != null)
            callback(err);
        //try
        //{
        //    //文件流信息
        //    FileStream fs;
        //    //FileInfo t = new FileInfo(url);
        //    if (!File.Exists(url))
        //    {
        //        string str = Path.GetDirectoryName(url);
        //        if (!Directory.Exists(str))
        //            Directory.CreateDirectory(str);
        //        fs = new FileStream(url, FileMode.OpenOrCreate);
        //    }
        //    else
        //    {
        //        if (!ReWrite)
        //        {
        //            fs = new FileStream(url, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Write);
        //            fs.Seek(0, SeekOrigin.End);
        //        }
        //        else
        //        {
        //            fs = new FileStream(url, FileMode.Truncate, FileAccess.Write);
        //            fs.Close();
        //            fs = new FileStream(url, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Write);
        //        }
        //    }

        //    // fs = new FileStream(url, FileMode.Append,FileAccess.Write);
        //    // fs.Position = fs.Length;
        //    //以行的形式写入信息
        //    fs.Write(buffer, offset, buffer.Length);

        //    fs.Flush();
        //    //关闭流
        //    fs.Close();
        //    //销毁流
        //    fs.Dispose();
        //    if (callback != null)
        //        callback(null);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e.Message + ":" + e.StackTrace);
        //    if (callback != null)
        //        callback(e.Message + ":" + e.StackTrace);
        //}
    }

    public static string LocalPathCorrect(string url)
    {
        string path = url;
        path = path.Replace("//", "/");
        path = path.Replace("\\", "/");
        path = path.Replace("\\\\", "/");
        path = path.Replace('\\', '/');
        // path = path.Replace('/', '/');
        return path;
    }

    public static string ConversionStr(byte[] data)
    {
        string str = null;
        MemoryStream memory = new MemoryStream(data);
        StreamReader sr = new StreamReader(memory);
        str = sr.ReadToEnd();
        memory.Close();
        memory.Dispose();
        return str;
    }

    public static string LoadFileStr(string path)
    {
        string str = "";
        if (!File.Exists(path))
            return null;
        using (StreamReader sr = new StreamReader(path))
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        return str;
    }
    public static byte[] LoadFilebytes(string path)
    {
        if (!File.Exists(path))
            return null;
        return File.ReadAllBytes(path);
        //FileStream fs = File.OpenRead(path);
        //byte[] bytes = new byte[fs.Length];
        //fs.Read(bytes, 0, bytes.Length);

        //// 设置当前流的位置为流的开始   
        //fs.Seek(0, SeekOrigin.Begin);
        //fs.Dispose();
        //fs.Close();

        //return bytes;
    }

    public static Texture2D readLocalTexture2d(string filePath)
    {
        return readLocalTexture2d(filePath,1024,1024);       
    }
    public static Texture2D readLocalTexture2d(string filePath,int width,int height)
    {
        if (!File.Exists(filePath))
            return null;
        Texture2D t2d = null;
        try
        {
            t2d = new Texture2D(width, height);

            //读取文件
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int byteLength = (int)fs.Length;
                byte[] imgBytes = new byte[byteLength];
                fs.Read(imgBytes, 0, byteLength);
                t2d.LoadImage(imgBytes);
                fs.Close();
            }
            //转化为Texture2D                  
            t2d.Apply();
        }
        catch (Exception e) { Debug.LogError(e.Message + ":" + e.StackTrace); }
        return t2d;
    }

    public static void DeleteFolder(string dir)
    {
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);
            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if (d1.GetFiles().Length != 0)
                {
                    DeleteFolder(d1.FullName);////递归删除子文件夹
                }
                Directory.Delete(d);
            }
        }
    }

    public static void CopyDirectory(string sourcePath, string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);
            if (fsi is System.IO.FileInfo)
                File.Copy(fsi.FullName, destName);
            else
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
    }
}
#region 打开Windows文件对话框
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class LocalDialog
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    //链接指定系统函数       打开文件对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOFN([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    //链接指定系统函数        另存为对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    public static bool GetSFN([In, Out] OpenFileName ofn)
    {
        return GetSaveFileName(ofn);
    }
}

#endregion




