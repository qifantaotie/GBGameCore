using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIMgr : Singleton<UIMgr>
{
    //public static UIMgr Instance
    //{
    //    get;
    //    private set;
    //}
    

    /// <summary>
    /// 所有UI界面的缓存
    /// </summary>
    private Dictionary<string, UIBase> mUIDic = new Dictionary<string, UIBase>();

    /// <summary>
    /// UI包队列
    /// </summary>
    private Queue<UIPack> mUiPackQueue=new Queue<UIPack>();
    
    /// <summary>
    /// UI 根节点
    /// </summary>
    private Transform UIROOT = null;

    private void AddUI(UIBase ui)
    {
        mUIDic.Add(ui.UIName,ui);
    }

    void Awake()
    {
        UIROOT = this.transform.Find("UIRoot");
        //Instance = this;
    }
    UIPack tempUiPack = null;
    void Update()
    {
        lock (mUiPackQueue)
        {
            if (mUiPackQueue.Count>0)
        {
            
            tempUiPack = mUiPackQueue.Dequeue();
            if (tempUiPack!=null)
            {
                switch (tempUiPack.UiCommand)
                {

                    case UIPack.UICommand.Show:
                        Show(tempUiPack);
                        break;
                    case UIPack.UICommand.CreateAndShow:
                        Create(tempUiPack);
                        break;
                    case UIPack.UICommand.Create:
                        Create(tempUiPack);
                        break;
                    case UIPack.UICommand.Hide:
                        Hide(tempUiPack);
                        break;
                    case UIPack.UICommand.Destroy:
                        Destroy(tempUiPack);
                        break;
                }
            }
        }
        }
        
    }

    #region 创建UI
    /// <summary>
    /// 创建UI实体
    /// </summary>
    /// <param name="uiPack"></param>
    private void Create(UIPack uiPack)
    {
        UIBase ui = null;
        mUIDic.TryGetValue(uiPack.UIName, out ui);
        if (ui == null)
        {
            ResMgr.Instance.Load<GameObject>(uiPack.UIName, new LoadUIFinish(uiPack));
        }
    }

    /// <summary>
    /// 加载UI完成
    /// </summary>
    private class LoadUIFinish : ILoadListener
    {
        private UIPack mUiPack;

        public LoadUIFinish(UIPack uiPack)
        {
            mUiPack = uiPack;
        }


        public void LoadFinish(object asset)
        {
            if (mUiPack == null)
            {
                return;
            }
            GameObject go = Instantiate<GameObject>(asset as GameObject);
            go.SetActive(false);
            if (mUiPack.ScriptType!=null)
            {
                UIBase ui = go.AddComponent(mUiPack.ScriptType) as UIBase;
                ui.UIName = mUiPack.UIName;
                go.gameObject.name = ui.UIName;
                UIMgr.Instance.AddUI(ui);
                ui.Init();
                ui.CacheTransform.SetParent(UIMgr.Instance.UIROOT, false);
                if (mUiPack.UiCommand == UIPack.UICommand.CreateAndShow)
                {
                    Instance.Show(mUiPack);
                }
            }
            
        }

        public void LoadFailure(string assetName)
        {
            Log.Error("资源加载失败，资源名：" + assetName);
        }
    }

    #endregion

    #region 显示UI
    /// <summary>
    /// 显示UI指令
    /// </summary>
    /// <param name="uiName">UI名称</param>
    /// <param name="scriptType">脚本类型</param>
    /// <param name="objs">参数</param>
    public void ShowUI(string uiName, Type scriptType, params object[] objs)
    {
        UIBase ui = null;
        mUIDic.TryGetValue(uiName, out ui);
        if (ui == null)
        {
            mUiPackQueue.Enqueue(new UIPack(UIPack.UICommand.CreateAndShow, uiName, scriptType, objs));

        }
        else
        {
            mUiPackQueue.Enqueue(new UIPack(UIPack.UICommand.Show,uiName,scriptType,objs));
        }
        
    }
    /// <summary>
    /// 显示UI实体
    /// </summary>
    /// <param name="uiPack">UI包</param>
    private void Show(UIPack uiPack)
    {
        UIBase ui = null;
        mUIDic.TryGetValue(uiPack.UIName, out ui);
        if (ui!=null)
        {
            ui.Show(uiPack.objs);
        }
    }
    #endregion

    #region 隐藏UI
    /// <summary>
    /// 隐藏UI
    /// </summary>
    /// <param name="uiName">UI名称</param>
    /// <param name="objs">参数</param>
    public void HideUI(string uiName, params object[] objs)
    {
        mUiPackQueue.Enqueue(new UIPack(UIPack.UICommand.Hide, uiName,null,objs));
    }

    /// <summary>
    /// 隐藏UI实体
    /// </summary>
    /// <param name="uiPack"></param>
    private void Hide(UIPack uiPack)
    {
        UIBase ui = null;
        mUIDic.TryGetValue(uiPack.UIName, out ui);
        if (ui!=null)
        {
            ui.Hide(uiPack.objs);
        }
    }
    #endregion

    #region 移除UI
    /// <summary>
    /// 删除UI
    /// </summary>
    /// <param name="uiName">UI名称</param>
    /// <param name="objs">参数</param>
    public void DestroyUI(string uiName,params object[] objs)
    {
        mUiPackQueue.Enqueue(new UIPack(UIPack.UICommand.Destroy, uiName,null,objs));
    }
    /// <summary>
    /// 删除UI
    /// </summary>
    /// <param name="uiPack"></param>
    private void Destroy(UIPack uiPack)
    {
        UIBase ui = null;
        mUIDic.TryGetValue(uiPack.UIName, out ui);
        if (ui!=null)
        {
            mUIDic.Remove(ui.UIName);
            Destroy(ui.CacheGameobject);
        }
    }
    #endregion

    /// <summary>
    /// UI信息
    /// </summary>
    public class UIPack
    {
        /// <summary>
        /// UI名称
        /// </summary>
        public string UIName { get; set; }

        /// <summary>
        /// 需要添加的脚本
        /// </summary>
        public Type ScriptType { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public object[] objs { get; set; }

        /// <summary>
        /// UI操作
        /// </summary>
        public UICommand UiCommand{ get; set; }
        /// <summary>
        /// UI操作命令
        /// </summary>
        public enum UICommand
        {
            Create,
            Show,
            CreateAndShow,
            Hide,
            Destroy
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uiCommand">UI操作命令</param>
        /// <param name="uiName">UI名称</param>
        /// <param name="type">脚本类型</param>
        /// <param name="objs">参数</param>
        public UIPack(UICommand uiCommand,string uiName,Type type,params object[] objs)
        {
            this.UiCommand = uiCommand;
            this.UIName = uiName;
            this.ScriptType = type;
            this.objs = objs;
        }
         
    }
}
