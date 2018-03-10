using System;
using UnityEngine;
using System.Collections;

public class ResReq : ResOrABReqBase
{
    /// <summary>
    /// 异步加载的Resource资源返回值
    /// </summary>
    private ResourceRequest mResourceRequest;

    /// <summary>
    /// 当前资源实发已经加载完成
    /// </summary>
    public override bool IsDone
    {
        get
        {
            if (IsAsync)
            {
                return mResourceRequest != null && mResourceRequest.isDone;
            }
            else
            {
                return mIsDone;
            }
        }
    }

    /// <summary>
    /// 加载到的资源
    /// </summary>
    public override object Asset
    {
        get {
            if (IsAsync)
            {
                return mResourceRequest.asset;
            }
            else
            {
                return mAsset;
            }
        }
    }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loadListener">加载完成的回调</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="type">资源类型</param>
        /// <param name="isKeepInMemory">是否常驻内存</param>
        public ResReq(string assetName,ILoadListener loadListener, Type type,
            bool isKeepInMemory, bool isAsync = true)
        {
            base.AssetName = assetName;
            //this.LoadListener.Add(loadListener);
            base.AssetType= type;
            base.IsKeepInMemory = isKeepInMemory;
            base.IsAsync = isAsync;
            this.LoadListener = loadListener;
            mFullPath = ResMgr.Instance.GetResFullName(assetName);
        }


    /// <summary>
    /// 加载资源
    /// </summary>
    public override void Do()
    {
        if (!IsAsync)
        {
            mIsDone = true;
            mAsset = Resources.Load(mFullPath, AssetType);
            return;
        }
#if UNITY_5
       mResourceRequest= Resources.LoadAsync(mFullPath, AssetType);
#else
            mIsDone = true;
            mAsset = Resources.Load(mFullPath, AssetType);
#endif
    }

    /// <summary>
    /// Resource资源加载完成回调
    /// </summary>
    public override ILoadListener LoadListener { get; set; }
}
