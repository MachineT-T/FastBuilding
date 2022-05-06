using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VR_ChuangKe.Share
{
    public class CodeColliderManager : MonoBehaviour
    {
        [SerializeField]
        public Transform ColParent;
        private Dictionary<string, GameObject> colDict;
        private Dictionary<GameObject, string> colNames;
        public static CodeColliderManager Instance { get { return _instance; } }
        private static CodeColliderManager _instance;
        private void Awake()
        {
            _instance = this;
            colDict = new Dictionary<string, GameObject>();
            colNames = new Dictionary<GameObject, string>();
            if (ColParent == null)
                ColParent = transform;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="cname"></param>
        public void delColGameObj(string cname)
        {
            GameObject go = null;
            colDict.TryGetValue(cname, out go);
            if (go != null)
                Destroy(go);
            colDict.Remove(cname);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="colGameObj"></param>
        public void delColGameObj(GameObject colGameObj)
        {
            if (colGameObj == null)
                return;
            if (colDict.ContainsKey(colGameObj.name))
            {
                colDict.Remove(colGameObj.name);
                Destroy(colGameObj);               
                return;
            }
            if (colDict.ContainsValue(colGameObj))
            {
                string[] names = getColNames();
                for (int i = 0; i < name.Length; i++)
                {
                    if (string.IsNullOrEmpty(names[i]) || !colDict.ContainsKey(names[i]))
                        continue;
                    if (colDict[names[i]] == colGameObj)
                    {
                        Destroy(colGameObj);
                        colDict.Remove(names[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        public GameObject createColGameObj(string resName)
        {
            GameObject go = ResLibaryMgr.Instance.GetObject<GameObject>(resName);
            if (go != null)
            {
                GameObject clone = Instantiate(go);
                string cname = System.Guid.NewGuid().ToString();
                clone.name = "Col:"+ cname;
                colDict[cname] = clone;
                clone.transform.SetParent(ColParent);
                EventListener.dispatchEvent("CreateCodeCol",clone);
                return clone;
            }
            return null;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="cname"></param>
        /// <returns></returns>
        public GameObject getColGameObj(string cname)
        {
            if (colDict.ContainsKey(cname))
            {
                return colDict[cname];
            }

            return null;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public string[] getColNames()
        {
            List<string> keys = new List<string>(colDict.Keys);
            return keys.ToArray();
        }
    }
}
