using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ResMgr :Singleton<ResMgr>
{
    

    /// <summary>
    /// 最大加载数量（CPU线程数量）
    /// </summary>
    private int mMaxLoadCount = 0;


    /// <summary>
    /// 所有Resources的资源相对目录
    /// </summary>
    private Dictionary<string, string> mResTxtDic = new Dictionary<string, string>();

    /// <summary>
    /// 资源对应的ab名称
    /// </summary>
    private Dictionary<string, ABInfo> mABInfoDic = new Dictionary<string, ABInfo>();

    /// <summary>
    /// 已经加载的资源缓存
    /// </summary>
    private Dictionary<string, AssetPack> mAssetPackDic = new Dictionary<string, AssetPack>();

    /// <summary>
    /// 正在加载的资源
    /// </summary>
    private List<ResOrABReqBase> mLoadIn = new List<ResOrABReqBase>();

    /// <summary>
    /// 等待加载的资源
    /// </summary>
    private Queue<ResOrABReqBase> mLoadWait = new Queue<ResOrABReqBase>();


    #region 初始化


    void Awake()
    {
        mMaxLoadCount = SystemInfo.processorCount;
        mMaxLoadCount = mMaxLoadCount < 1 ? 1 : mMaxLoadCount;
        mMaxLoadCount = mMaxLoadCount > 8 ? 8 : mMaxLoadCount;

        LoadResTxt();
        LoadVer();
    }

    /// <summary>
    /// 加载Resources资源目录
    /// </summary>
    void LoadResTxt()
    {
        TextAsset txt = Resources.Load<TextAsset>("res");
        TextReader reader = new StringReader(txt.text);
        string lineTxt = reader.ReadLine();
        while (!string.IsNullOrEmpty(lineTxt))
        {
            mResTxtDic.Add(lineTxt.Split('=')[0], lineTxt.Split('=')[1]);
            lineTxt = reader.ReadLine();
        }
        reader.Close();
    }

    /// <summary>
    /// 加载AssetBundle资源目录
    /// </summary>
    void LoadVer()
    {
        TextAsset txt = Resources.Load<TextAsset>("Ver");
        TextReader reader = new StringReader(txt.text);
        string lineTxt = reader.ReadLine();
        while (!string.IsNullOrEmpty(lineTxt))
        {
            string assetName = lineTxt.Split('|')[0];
            string fullName = assetName + lineTxt.Split('|')[1];
            string abName = lineTxt.Split('|')[2];
            mABInfoDic.Add(assetName, new ABInfo(assetName, fullName, abName));
            lineTxt = reader.ReadLine();
        }
        reader.Close();
    }
    #endregion

    void Update()
    {
        if (mLoadIn.Count != 0)
        {
            for (int i = 0; i < mLoadIn.Count; i++)
            {
                if (mLoadIn[i].IsDone)
                {
                    LoadFinish(mLoadIn[i]);
                    mLoadIn.RemoveAt(i);
                    i--;
                }
            }
        }
        while (mLoadIn.Count < mMaxLoadCount && mLoadWait.Count != 0)
        {
            ResOrABReqBase req = mLoadWait.Dequeue();
            mLoadIn.Add(req);
            req.Do();
        }
    }
    #region 公用方法
    /// <summary>
    /// 获取资源对应的AB信息
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <returns></returns>
    private ABInfo GetABInfo(string assetName)
    {
        if (mABInfoDic.ContainsKey(assetName))
        {
            return mABInfoDic[assetName];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取Rescource文件夹内资源全名
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public string GetResFullName(string assetName)
    {
        return mResTxtDic.ContainsKey(assetName) && !string.IsNullOrEmpty(ResMgr.Instance.mResTxtDic[assetName]) ? ResMgr.Instance.mResTxtDic[assetName] + "/" + assetName : assetName;
    }
    #endregion

    #region 资源加载缓存卸载

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="listener">资源加载完成后的回调</param>
    /// <param name="isKeepInMemory">是否常驻内存</param>
    /// <param name="isAsync">是否异步</param>
    public void Load(string assetName, ILoadListener listener, bool isKeepInMemory = false, bool isAsync = true)
    {
        Load<GameObject>(assetName,listener,isKeepInMemory,isAsync);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="assetName">资源名称</param>
    /// <param name="listener">资源加载完成后的回调</param>
    /// <param name="isKeepInMemory">是否常驻内存</param>
    /// <param name="isAsync">是否异步</param>
    public void Load<T>(string assetName, ILoadListener listener, bool isKeepInMemory=false, bool isAsync=true)
    {
        //如果资源缓存里面已经存在这个资源那么直接返回
        if (mAssetPackDic.ContainsKey(assetName)&&mAssetPackDic[assetName]!=null)
        {
            if (listener!=null)
            {
                listener.LoadFinish(mAssetPackDic[assetName].Asset);
            }
            return;
        }
        //如果是Resources里的资源，调用LoadForRes
        if (mResTxtDic.ContainsKey(assetName))
        {
            LoadForRes<T>(assetName, listener,isKeepInMemory, isAsync);
        }else if (mABInfoDic.ContainsKey(assetName))
        {
            LoadForAB(assetName, listener, isKeepInMemory, isAsync);
        }
        else
        {
            if (listener!=null)
            {
                listener.LoadFailure(assetName);
            }
            Log.Error("你要加载的资源不存在 资源名称：" + assetName);
        }
    }
    
    /// <summary>
    /// 加载Resource资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="assetName">资源名称</param>
    /// <param name="listener">回调</param>
    /// <param name="isKeepInMemory">是否常驻内存</param>
    /// <param name="isAsync">是否异步</param>
    private void LoadForRes<T>(string assetName, ILoadListener listener, bool isKeepInMemory, bool isAsync)
    {
        if (!mResTxtDic.ContainsKey(assetName))
        {
            if (listener!=null)
            {
                listener.LoadFailure(assetName);
            }
            Log.Error("这个资源不存在Resources里面 资源名称：" + assetName);
            return;
        }
        for (int i = 0; i < mLoadIn.Count; i++)
        {
            if (mLoadIn[i].AssetName==assetName)
            {
                Log.Error("正在加载这个资源 资源名称：" + assetName);
                return;
            }
        }
        for (int i = 0; i < mLoadWait.Count; i++)
        {
            if (mLoadWait.ToArray()[i].AssetName == assetName)
            {
                Log.Error("这个资源已经等待加载了 资源名称：" + assetName);
                return;
            }
        }
        ResReq req=new ResReq(assetName,listener,typeof(T),isKeepInMemory,isAsync);
        mLoadWait.Enqueue(req);
    }

    /// <summary>
    /// 加载AB资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="listener">回调</param>
    /// <param name="isKeepInMemory">是否常驻内存</param>
    /// <param name="isAsync">是否异步</param>
    private void LoadForAB(string assetName, ILoadListener listener, bool isKeepInMemory, bool isAsync)
    {
        if (!mABInfoDic.ContainsKey(assetName))
        {
            if (listener != null)
            {
                listener.LoadFailure(assetName);
            }
            Debug.LogError("这个资源不存在AB里面 资源名称：" + assetName);
            return;
        }
        for (int i = 0; i < mLoadIn.Count; i++)
        {
            if (mLoadIn[i].AssetName == assetName)
            {
                Log.Error("正在加载这个资源 资源名称：" + assetName);
                return;
            }
        }
        for (int i = 0; i < mLoadWait.Count; i++)
        {
            if (mLoadWait.ToArray()[i].AssetName == assetName)
            {
                Log.Error("这个资源已经等待加载了 资源名称：" + assetName);
                return;
            }
        }
        ABInfo abInfo = GetABInfo(assetName);
        ABReq req=new ABReq(abInfo.AssetName,abInfo.ABName,listener,isKeepInMemory,isAsync);
        mLoadWait.Enqueue(req);
    }
    #endregion


    #region 加载完成回调

    /// <summary>
    /// 加载完成回调
    /// </summary>
    /// <param name="req"></param>
    void LoadFinish(ResOrABReqBase req)
    {
        if (req is ABReq)
        {
            LoadABFinish(req as ABReq);
        }
        else
        {
            LoadResFinish(req as ResReq);
        }
    }

    /// <summary>
    /// Resource资源加载完成回调
    /// </summary>
    /// <param name="req"></param>
    private void LoadResFinish(ResReq req)
    {
        if (req.Asset==null)
        {
            if (req.LoadListener!=null)
            {
                req.LoadListener.LoadFailure(req.AssetName);
            }
        }
        else
        {
            AssetPack asset=new AssetPack(req.AssetName ,req.Asset,req.IsKeepInMemory);
            if (mAssetPackDic.ContainsKey(req.AssetName))
            {
                mAssetPackDic[req.AssetName]=asset;
            }
            else
            {
                mAssetPackDic.Add(req.AssetName,asset);
            }
            req.LoadListener.LoadFinish(asset.Asset);
            AddAssetToStackTop(asset.AssetName);
        }
    }

    /// <summary>
    /// AB加载完成回调
    /// </summary>
    /// <param name="req"></param>
    private void LoadABFinish(ABReq req)
    {
        
        AssetBundle assetBundle = req.ABWww.assetBundle;
       // req.WWWDispose();
        if (assetBundle==null)
        {
            if (req.LoadListener!=null)
            {
                req.LoadListener.LoadFailure(req.AssetName);
            }
        }
        else
        {
            if (req.IsAsync)
            {
                StartCoroutine(LoadABAsync(assetBundle, req.AssetName, req.LoadListener, req.IsKeepInMemory));
            }
            else
            {
                ABInfo abInfo = GetABInfo(req.AssetName);
                object obj = assetBundle.LoadAsset(abInfo.FullName);
                AssetPack asset=new AssetPack(abInfo.AssetName,obj,req.IsKeepInMemory);
                if (mAssetPackDic.ContainsKey(asset.AssetName))
                {
                    mAssetPackDic[asset.AssetName] = asset;
                }
                else
                {
                    mAssetPackDic.Add(asset.AssetName,asset);
                }
                req.LoadListener.LoadFinish(obj);
                AddAssetToStackTop(asset.AssetName);
                assetBundle.Unload(false);
            }
        }
        
    }

    /// <summary>
    /// AB异步加载
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <param name="p1"></param>
    /// <param name="loadListener"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    IEnumerator LoadABAsync(AssetBundle assetBundle, string assetName, ILoadListener loadListener, bool isKeepInMemory)
    {
        
        AssetBundleRequest abreq = assetBundle.LoadAssetAsync(GetABInfo(assetName).FullName);
        yield return abreq;
        yield return 1;
        AssetPack asset = new AssetPack(assetName, abreq.asset, isKeepInMemory);
        if (mAssetPackDic.ContainsKey(asset.AssetName))
        {
            mAssetPackDic[asset.AssetName]= asset;
        }
        else
        {
            mAssetPackDic.Add(asset.AssetName,asset);
        }
        
        loadListener.LoadFinish(abreq.asset);
        AddAssetToStackTop(asset.AssetName);
        assetBundle.Unload(false);
    }
    #endregion

    #region 资源释放以及监听

    /// <summary>
    /// 资源加载堆栈
    /// </summary>
    public Stack<List<string>> mAssetStack = new Stack<List<string>>();

    /// <summary>
    /// 把资源压入顶层栈内
    /// </summary>
    /// <param name="assetName">资源名称</param>
    public void AddAssetToStackTop(string assetName)
    {
        if (mAssetStack.Count == 0)
        {
            mAssetStack.Push(new List<string>() { assetName });
        }

        List<string> list = mAssetStack.Peek();
        list.Add(assetName);
    }

    /// <summary>
    /// 开始让资源入栈
    /// </summary>
    public void PushAssetStack()
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, AssetPack> info in mAssetPackDic)
        {
            info.Value.Count++;
            list.Add(info.Key);
        }

        mAssetStack.Push(list);
    }

    /// <summary>
    /// 释放栈内资源
    /// </summary>
    public void PopAssetStack()
    {
        if (mAssetStack.Count == 0) return;

        List<string> list = mAssetStack.Pop();
        List<string> removeList = new List<string>();
        AssetPack info = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (mAssetPackDic.TryGetValue(list[i], out info))
            {
                info.Count--;
                if (info.Count < 1 && !info.IsKeepInMemory)
                {
                    removeList.Add(list[i]);
                }
            }
        }
        for (int i = 0; i < removeList.Count; i++)
        {
            if (mAssetPackDic.ContainsKey(removeList[i]))
                mAssetPackDic.Remove(removeList[i]);
        }

        GC();
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void GC()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }


    #endregion
}
