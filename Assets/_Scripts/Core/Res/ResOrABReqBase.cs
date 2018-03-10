using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 得到资源信息基类
/// </summary>
public abstract class ResOrABReqBase
{

    /// <summary>
    /// 资源全路径
    /// </summary>
    protected string mFullPath;

    /// <summary>
    /// 资源名称
    /// </summary>
    public string AssetName { get; set; }

    /// <summary>
    /// 是否异步
    /// </summary>
    private bool mIsAsync = true;

    /// <summary>
    /// 是否异步
    /// </summary>
    public bool IsAsync
    {
        get { return mIsAsync; }
        set { mIsAsync = value; }
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public Type AssetType { get; set; }

    /// <summary>
    /// 是否常驻内存
    /// </summary>
    private bool mIsKeepInMemory = false;

    /// <summary>
    /// 是否常驻内存
    /// </summary>
    public bool IsKeepInMemory
    {
        get { return mIsKeepInMemory; }
        set
        {
            mIsKeepInMemory = value;
        }
    }

    /// <summary>
    /// 是否已经加载完成
    /// </summary>
    protected  bool mIsDone;

    /// <summary>
    /// 加载到的资源
    /// </summary>
    protected object mAsset;

    /// <summary>
    /// 是否加载完成
    /// </summary>
    public abstract bool IsDone { get; }

    /// <summary>
    /// 加载到的资源
    /// </summary>
    public abstract object Asset { get; }

    /// <summary>
    /// 资源加载完成回调
    /// </summary>
    public abstract ILoadListener LoadListener { get; set; }
    
    /// <summary>
    /// 开始加载资源
    /// </summary>
    public abstract void Do();
}
