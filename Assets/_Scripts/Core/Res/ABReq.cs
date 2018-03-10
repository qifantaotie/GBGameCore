using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;

public class ABReq:ResOrABReqBase  {

    /// <summary>
    /// 加载的实例
    /// </summary>
    public  WWW ABWww { get; set; }

    /// <summary>
    /// 是否加载完成
    /// </summary>
    public override bool IsDone
    {
        get { return ABWww.isDone; }
    }

    /// <summary>
    /// 资源
    /// </summary>
    public override object Asset
    {
        get { return null; }
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    public override void Do()
    {
        ABWww = new WWW("file://" + mFullPath); 
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="isKeepInMemory"></param>
    /// <param name="isAsync"></param>
    public ABReq(string assetName,string abName,ILoadListener listener,  bool isKeepInMemory, bool isAsync = true)
    {
        if (File.Exists(AppMgr.Instance.externalResPath + "/" + abName))
        {
            base.mFullPath = AppMgr.Instance.externalResPath + "/" + abName;
        }
        else
        {
            base.mFullPath = AppMgr.Instance.interiorResPath + "/" + abName;
        }
        this.LoadListener = listener;
        base.AssetName = assetName;
        base.IsKeepInMemory = isKeepInMemory;
        base.IsAsync = isAsync;
    }

    /// <summary>
    /// 销毁WWW资源
    /// </summary>
    public void WWWDispose()
    {
        ABWww.Dispose();
    }

    /// <summary>
    /// AB资源加载完成回调
    /// </summary>
    public override ILoadListener LoadListener {  get; set; }
}
