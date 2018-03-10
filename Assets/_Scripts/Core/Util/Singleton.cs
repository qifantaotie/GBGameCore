using System;
using UnityEngine;

//public class SingletonParent : Singleton<SingletonParent>
//{

//}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (singletonIsDestroy)
            {
                Log.Error(typeof(T) + "已被销毁");
                return null;
            }
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Log.Error("已有单例" + _instance.gameObject.name);
                        }

                        if (_instance == null)
                        {
                           // _instance = (T)Activator.CreateInstance(typeof(T));
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(Singleton)"+typeof(T).ToString();

                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
            }
            return _instance;

        }
    }

    private static bool singletonIsDestroy = false;

    public void OnDestroy()
    {
        singletonIsDestroy = true;
    }

    public static void DestroySingleton()
    {
        if (Instance != null)
        {
            Debug.Log("销毁单例:" + _instance.gameObject.name);
            Destroy(_instance.gameObject);
            singletonIsDestroy = true;
            _instance = null;
        }
    }
}
