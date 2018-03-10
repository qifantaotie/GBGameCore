using UnityEngine;
using System.Collections;

/// <summary>
/// 缓存资源
/// </summary>
public class AssetPack
{

    private bool mIsKeepIsMemory = false;

    /// <summary>
    /// 是否常驻内存
    /// </summary>
    public bool IsKeepInMemory {get { return mIsKeepIsMemory; }set { mIsKeepIsMemory = value; } }

    /// <summary>
    /// 资源名称
    /// </summary>
    public string AssetName {private set; get; }

    /// <summary>
    /// 资源
    /// </summary>
    public object Asset { get; set; }

    /// <summary>
    /// 有多少层在使用这个资源
    /// </summary>
    public int Count { get; set; }


    public AssetPack(string _assetName, object _asset, bool _isKeepInMemory)
    {
        this.AssetName = _assetName;
        this.Asset = _asset;
        this.mIsKeepIsMemory = _isKeepInMemory;
    }
}
