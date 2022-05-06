using UnityEngine;


/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
namespace VR_ChuangKe.Share
{
    public abstract class MonoSingleton<T> : MonoSingleton where T : MonoSingleton
    {
        private static volatile T _instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }
                if (_instance == null)
                {

                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            T[] instance1 = FindObjectsOfType<T>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(T).Name);
                    singletonObj = go;
                    _instance = go.AddComponent<T>();
                    _instance.OnInit();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return _instance;
            }

        }


        private void Awake()
        {

            T t = gameObject.GetComponent<T>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);

                _instance = t;
                OnInit();
                _applicationIsQuitting = false;
            }
            else if (singletonObj != gameObject)
            {
                MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
                if (monos.Length > 1)
                {
                    Destroy(t);
                }
                else
                {
                    Destroy(gameObject);
                }
            }

        }


        public override void OnUpdate()
        {

        }

        public override void OnDispose()
        {
            _instance = null;
           
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                singletonObj = null;
                OnDispose();
                _applicationIsQuitting = true;
            }

        }
    }


    public abstract class MonoSingleton : MonoBehaviour, IBehaviorHandle
    {
        public abstract void OnInit();
        public abstract void OnUpdate();
        public abstract void OnDispose();
    }

}