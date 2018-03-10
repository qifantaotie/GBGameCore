using UnityEngine;
using System.Collections;

public interface ILoadListener
{

    /// <summary>
    /// 加载资源结束回调
    /// </summary>
    void LoadFinish(object asset);

    /// <summary>
    /// 加载资源失败回调
    /// </summary>
    void LoadFailure(string assetName);
}