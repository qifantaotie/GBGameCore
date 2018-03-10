using UnityEngine;
using System.Collections;

public class AppMgr : MonoBehaviour {

    public static AppMgr Instance { private set; get; }

    /// <summary>
    /// 内部资源路径
    /// </summary>
    public string interiorResPath = "";

    /// <summary>
    /// 外部资源路径
    /// </summary>
    public string externalResPath = "";

    /// <summary>
    /// 内部资源URL路径
    /// </summary>
    public string InteriorResPathUrl
    {
        get
        {
#if UNITY_ANDROID
            interiorResPath="jar://"+interiorResPath;
#elif UNITY_IPHONE
#else
            interiorResPath = "file://" + interiorResPath;
#endif
            return interiorResPath;
        }
        
    }


    public string ExternalResPathUrl
    {
        get
        {
#if UNITY_ANDROID
            externalResPath="jar://"+externalResPath;
#elif UNITY_IPHONE
#else
            externalResPath = "file://" + externalResPath;
#endif
            return externalResPath;
        }
    }

    void Awake()
    {
        Instance = this;
        interiorResPath = Application.streamingAssetsPath;
        externalResPath = Application.persistentDataPath;
    }
}
