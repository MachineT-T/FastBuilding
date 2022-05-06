using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Text;
using LitJson;
using System.Security.Cryptography;

public class UnityTool : VR_ChuangKe.Share.MonoSingleton<UnityTool>
{
    private class LogStateObj
    {
        public string time;
        public string condition;
        public string stackTrace;
        public LogType lType;

        public LogStateObj(string condition, string stackTrace, LogType lType)
        {
            this.time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff");
            this.lType = lType;
            this.condition = condition;
            this.stackTrace = stackTrace;
        }
    }
    private bool hasReglog = false;
    private List<LogStateObj> logList;
    private Tool.Coroutine.GlobalCoroutineMgr GlobalCoroutine;
    public override void OnInit()
    {
        logList = new List<LogStateObj>();
        GlobalCoroutine = gameObject.AddComponent<Tool.Coroutine.GlobalCoroutineMgr>();
        GlobalCoroutine.OnInit();
    }

    public void Registorlog()
    {
        if (!hasReglog)
        {
            hasReglog = true;
            Application.logMessageReceived += HandleLog;
        }
    }

    void HandleLog(string condition, string stackTrace, LogType lType)
    {
        if (lType == LogType.Exception || lType == LogType.Error)
            logList.Add(new LogStateObj(condition, stackTrace, lType));
    }

    public void SubmitDebugInfo(System.Action callback)
    {
        if (!hasReglog)
        {
            if (callback != null)
                callback();
            return;
        }
        GlobalCoroutine.UTStartCoroutine("SubmitDebugInfo", PostDebugInfo(callback));
    }
    IEnumerator PostDebugInfo(System.Action cb)
    {
        StringBuilder sb = new StringBuilder();
        LogStateObj[] ls = logList.ToArray();
        for (int i = 0; i < ls.Length; i++)
        {
            LogStateObj lso = ls[i];
            sb.Append(lso.time);
            sb.Append("----");
            sb.Append(lso.lType.ToString());
            sb.Append("----");
            sb.Append(lso.condition);
            sb.Append("\n");
            sb.Append(lso.stackTrace);
            if (i < ls.Length - 1)
                sb.Append("\n");
        }
        string url = "http://o220835a38.iask.in/log";
        WWWForm form = new WWWForm();
        form.AddField("identifier", "xkshaw");
        form.AddField("passw", "1qaz2wsx3edc");
        form.AddField("logData", ZipHelper.GZipCompressString((sb.ToString())));
        UnityWebRequest unityWeb = UnityWebRequest.Post(url, form);
        unityWeb.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        unityWeb.downloadHandler = new DownloadHandlerBuffer();
        unityWeb.timeout = 3000;
        yield return unityWeb.SendWebRequest();

        if (cb != null)
            cb();
    }
    public static System.Random random
    {
        get
        {
            if (_random == null)
                _random = new System.Random(UnityEngine.Random.Range(0, 1000000));
            return _random;
        }
    }
    private static System.Random _random;
    public static string CreateIntID(uint length)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            result = string.Format("{0}{1}", result, random.Next(0, 9).ToString());
        }
        return result;
    }

    public static string CreateStringID(uint length)
    {

        if (length >= 32)
        {
            int lf = (int)length % 32;
            int c = lf == 0 ? (int)length / 32 : (int)length / 32 + 1;
            string str = "";
            for (int i = 0; i < c; i++)
            {
                str = string.Format("{0}{1}", str, Guid.NewGuid().ToString("N"));
            }
            return str.Substring(0, (int)length);
        }
        else
        {
            string str = Guid.NewGuid().ToString("N");
            return str.Substring(0, (int)length);
        }
    }

    #region 协程

    public static void UTDelayExcuteAction(string id, float delay, System.Action callback)
    {
        UTStartCoroutine(id, Instance.DelayExcuteMethod(delay, callback));
    }
    public static void UTDelayExcuteAction(float delay, System.Action callback)
    {
        UTStartCoroutine(Instance.DelayExcuteMethod(delay, callback));
    }
    private IEnumerator DelayExcuteMethod(float delay, System.Action callback)
    {
        delay = delay < 0 ? 0 : delay;
        yield return new WaitForSeconds(delay);
        if (callback != null)
            callback();
    }

    public void m_UTStartCoroutine(string id, IEnumerator routine)
    {
        GlobalCoroutine.UTStartCoroutine(id, routine);
    }
    public IEnumerator m_BGStartCoroutine(string id, IEnumerator routine)
    {
        return GlobalCoroutine.GBStartCoroutine(id, routine);
    }

    public void m_UTStopCoroutine(string id)
    {
        GlobalCoroutine.UTStopCoroutine(id);
    }

    public static void UTStopCoroutine(string id)
    {
        if (UnityTool.Instance != null)
            UnityTool.Instance.m_UTStopCoroutine(id);
    }

    //public static void UTStopCoroutine(IEnumerator routine)
    //{
    //    UnityTool.Instance.StopCoroutine(routine);
    //}

    public static void UTStartCoroutine(IEnumerator routine)
    {
        string id = Time.time.ToString() + UnityEngine.Random.Range(1, 100).ToString();
        UTStartCoroutine(id, routine);
    }

    public static void UTStartCoroutine(string id, IEnumerator routine)
    {
        UnityTool.Instance.m_UTStartCoroutine(id, routine);
    }

    public static IEnumerator GBStartCoroutine(IEnumerator routine)
    {
        string id = Time.time.ToString() + UnityEngine.Random.Range(1, 100).ToString();
        yield return UnityTool.Instance.m_BGStartCoroutine(id, routine);
    }

    #endregion

    private static Dictionary<string, List<System.Action<string, AudioClip>>> loadAudioDic = new Dictionary<string, List<Action<string, AudioClip>>>();
    public static void LoadAudio(string path, System.Action<string, AudioClip> callback)
    {
        if (callback == null)
            return;
        string fp = path;
        fp = fp.Replace("/", "");
        fp = fp.Replace("//", "");
        fp = fp.Replace("\\", "");
        fp = fp.Replace("\\\\", "");
        if (!loadAudioDic.ContainsKey(fp))
            loadAudioDic[fp] = new List<Action<string, AudioClip>>();
        loadAudioDic[fp].Add(callback);
        UnityTool.UTStartCoroutine(LoadAudioIEnumerator(path));
    }

    private static IEnumerator LoadAudioIEnumerator(string path)
    {
        string fp = path;
        fp = fp.Replace("/", "");
        fp = fp.Replace("//", "");
        fp = fp.Replace("\\", "");
        fp = fp.Replace("\\\\", "");
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN))
        {
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();
            Action<string, AudioClip>[] actions = loadAudioDic[fp].ToArray();
            for (int i = 0; i < actions.Length; i++)
            {
                Action<string, AudioClip> callback = actions[i];
                if (!string.IsNullOrEmpty(uwr.error))
                {
                    if (callback != null)
                        callback(uwr.error, null);
                }
                else
                {
                    if (callback != null)
                        callback(null, DownloadHandlerAudioClip.GetContent(uwr));
                }
            }
            loadAudioDic.Remove(fp);

        }

    }

    #region 组件和对象查找

    public static void GetChildComp<T>(Transform Trans, ref List<T> list) where T : Component
    {
        for (int i = 0; i < Trans.childCount; i++)
        {
            T[] ts = Trans.GetChild(i).GetComponents<T>();
            if (ts.Length > 0)
            {
                for (int j = 0; j < ts.Length; j++)
                {
                    T t = ts[j];
                    if (!list.Contains(t))
                        list.Add(t);
                }
            }
            GetChildComp<T>(Trans.GetChild(i), ref list);
        }
    }

    public static void GetChildTrans(Transform Trans, string childName, ref List<Transform> list)
    {
        if (string.IsNullOrEmpty(childName))
            return;
        for (int i = 0; i < Trans.childCount; i++)
        {
            if (Trans.GetChild(i).name == childName)
            {
                if (!list.Contains(Trans.GetChild(i)))
                    list.Add(Trans.GetChild(i));
            }
            GetChildTrans(Trans.GetChild(i), childName, ref list);
        }
    }

    /// <summary>
    /// 查找子节点对象
    /// 内部使用“递归算法”
    /// </summary>
    /// <param name="goParent">父对象</param>
    /// <param name="chiildName">查找的子对象名称</param>
    /// <returns></returns>
    public static Transform FindTheChildNode(GameObject goParent, string chiildName)
    {
        Transform searchTrans = null;                   //查找结果

        searchTrans = goParent.transform.Find(chiildName);
        if (searchTrans == null)
        {
            foreach (Transform trans in goParent.transform)
            {
                searchTrans = FindTheChildNode(trans.gameObject, chiildName);
                if (searchTrans != null)
                {
                    return searchTrans;

                }
            }
        }
        return searchTrans;
    }

    /// <summary>
    /// 获取子节点（对象）脚本
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T GetTheChildNodeComponetScript<T>(GameObject goParent, string childName) where T : Component
    {
        //查找结果
        T t = null;
        Transform searchTrans = goParent.transform.Find(childName);
        if (searchTrans != null)
            t = searchTrans.gameObject.GetComponent<T>();
        if (t == null)
        {
            for (int i = 0; i < goParent.transform.childCount; i++)
            {
                Transform trans = goParent.transform.GetChild(i);
                if (trans == null)
                    continue;
                t = GetTheChildNodeComponetScript<T>(trans.gameObject, childName);
                if (t != null)
                {
                    return t;
                }
            }
        }
        return t;
    }

    /// <summary>
    /// 给子节点添加脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranform = null;                //查找特定节点结果

        //查找特定子节点
        searchTranform = FindTheChildNode(goParent, childName);
        //如果查找成功，则考虑如果已经有相同的脚本了，则先删除，否则直接添加。
        if (searchTranform != null)
        {
            //如果已经有相同的脚本了，则先删除
            T[] componentScriptsArray = searchTranform.GetComponents<T>();
            for (int i = 0; i < componentScriptsArray.Length; i++)
            {
                if (componentScriptsArray[i] != null)
                {
                    Destroy(componentScriptsArray[i]);
                }
            }
            return searchTranform.gameObject.AddComponent<T>();
        }
        else
        {
            return null;
        }
        //如果查找不成功，返回Null.
    }

    /// <summary>
    /// 给子节点添加父对象
    /// </summary>
    /// <param name="parents">父对象的方位</param>
    /// <param name="child">子对象的方法</param>
    public static void AddChildNodeToParentNode(Transform parents, Transform child)
    {
        child.SetParent(parents, false);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        child.localEulerAngles = Vector3.zero;
    }
    public static T GetParentComponentScript<T>(Transform go) where T : Component
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

    public static Transform FindTheParentNode(Transform go, string pName)
    {
        Transform t = null;
        if (go.parent != null)
        {
            if (go.root != go.parent)
            {
                if (go.parent.name != pName)
                    t = FindTheParentNode(go.parent, pName);
                else
                    t = go.parent;
            }
        }
        return t;
    }

    #endregion
    public static bool isjson(string content)
    {
        try
        {
            JsonData jd = JsonMapper.ToObject(content);
            return true;
        }
        catch
        {
            return false;
        }
    }
    #region 加密解密,压缩解压
    public static bool EnableEncryptor = false;
    private static string CryptorKey { get { return "22348578902223367977723456789011"; } }
    private static RijndaelManaged rDel;
    private static ICryptoTransform cEncryptor;
    private static ICryptoTransform cDecryptor;

    /// <summary>
    /// 加密  返回加密后的结果
    /// </summary>
    /// <param name="content">需要加密的数据内容</param>
    /// <returns></returns>
    public static string Encrypt(string content)
    {
        if (!EnableEncryptor)
            return content;
        return EncryptTool(content);
    }
    public static string EncryptTool(string content)
    {

        string toE = ZipHelper.GZipCompressString(content);
        if (rDel == null)
        {
            rDel = new RijndaelManaged();
            rDel.Key = UTF8Encoding.UTF8.GetBytes(CryptorKey);
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            cEncryptor = rDel.CreateEncryptor();
            cDecryptor = rDel.CreateDecryptor();
        }

        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toE);
        byte[] resultArray = cEncryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public static byte[] EncryptBytes(byte[] content)
    {
        if (!EnableEncryptor)
            return content;
        return EncryptBytesTool(content);
    }
    public static byte[] EncryptBytesTool(byte[] content)
    {
        byte[] toE = ZipHelper.Compress(content);
        if (rDel == null)
        {
            rDel = new RijndaelManaged();
            rDel.Key = UTF8Encoding.UTF8.GetBytes(CryptorKey);
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            cEncryptor = rDel.CreateEncryptor();
            cDecryptor = rDel.CreateDecryptor();
        }

        byte[] toEncryptArray = toE;
        byte[] resultArray = cEncryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return resultArray;
    }

    /// <summary>
    /// 解密  返回解密后的结果
    /// </summary>
    /// <param name="toD">加密的数据内容</param>
    /// <returns></returns>
    public static string Decrypt(string toD)
    {
        if (rDel == null)
        {
            rDel = new RijndaelManaged();
            rDel.Key = UTF8Encoding.UTF8.GetBytes(CryptorKey);
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            cEncryptor = rDel.CreateEncryptor();
            cDecryptor = rDel.CreateDecryptor();
        }
        try
        {
            byte[] toEncryptArray = Convert.FromBase64String(toD);
            byte[] resultArray = cDecryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            string content = ZipHelper.GZipDecompressString(UTF8Encoding.UTF8.GetString(resultArray));
            return content;
        }
        catch { return toD; }
    }

    public static byte[] DecryptBytes(byte[] toD)
    {
        if (rDel == null)
        {
            rDel = new RijndaelManaged();
            rDel.Key = UTF8Encoding.UTF8.GetBytes(CryptorKey);
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            cEncryptor = rDel.CreateEncryptor();
            cDecryptor = rDel.CreateDecryptor();
        }
        try
        {
            byte[] toEncryptArray = toD;
            byte[] resultArray = cDecryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            byte[] content = ZipHelper.Decompress(resultArray);
            return content;
        }
        catch { return toD; }
    }


    #endregion
}

