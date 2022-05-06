using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Frame.Ctrl
{
    public delegate BaseMediator MediatorConstruct();
    public class MediatorFactory : MonoBehaviour
    {
        private static Dictionary<string, MediatorConstruct> constructDic
        {
            get
            {
                if (_constructDic == null)
                    _constructDic = new Dictionary<string, MediatorConstruct>();
                return _constructDic;
            }
        }
        private static Dictionary<string, MediatorConstruct> _constructDic;

        public static void RegistorConstruct(string constructName, MediatorConstruct construct)
        {
            if (!constructDic.ContainsKey(constructName))
            {
                constructDic[constructName] = construct ;
            }
        }

        public static BaseMediator CreateMediator(string constructName,params object[] data)
        {
            if (constructDic.ContainsKey(constructName))
            {
                BaseMediator node = constructDic[constructName]();
                node.Initialized(data);
                return node;
            }
            return null;
        }
    }
}
