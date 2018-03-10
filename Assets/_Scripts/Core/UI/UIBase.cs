using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour {
    /// <summary>
    /// 需要查找的组件物体名称
    /// </summary>
    protected abstract List<string> mFindNames
    {
        get;
    }


    private List<Transform> mFindTransforms;

    /// <summary>
    /// 获取子物体中所有要查找的组件的游戏对象
    /// </summary>
    /// <param name="mFindTransforms"></param>
    private void GetTransformsInChilds(List<Transform> mFindTransforms)
    {
        ComUtil.GetTransformInChilds(mFindNames,mTransform,ref mFindTransforms);
    }


    /// <summary>
    /// 靠名称获取子物体中的指定组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="goName">组件游戏对象名称</param>
    /// <param name="component">找到的组件</param>
    protected void GetComponentInChildsByName<T>(string goName,out T component) where T:Component
    {
        component=new Component()as T;
        for (int i = 0; i < mFindTransforms.Count; i++)
        {
            if (mFindTransforms[i].gameObject.name==goName)
            {
                component = mFindTransforms[i].transform.GetComponent<T>();
            }
        }
        
    }

    /// <summary>
    /// 绑定按钮点击事件
    /// </summary>
    /// <param name="button"></param>
    /// <param name="callback"></param>
    protected void BindButtonEvent(Button button,UnityAction callback)
    {
        if (null!=button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }
    }

    /// <summary>
    /// 当前UI的名字
    /// </summary>
    public string UIName
    {
        set;
        get;
    }

    private Transform mTransform;
    public Transform CacheTransform
    {
        get
        {
            return mTransform ;
        }
    }

    private GameObject mGo;
    public GameObject CacheGameobject
    {
        get
        {
            return mGo;
        }
    }

    

    #region 创建界面

    /// <summary>
    /// 创建界面
    /// </summary>
    public  void Init()
    {
        mGo = this.gameObject;
        mTransform = this.transform;
        mFindTransforms=new List<Transform>();
        GetTransformsInChilds(mFindTransforms);
        OnInit();
    }
#endregion

    #region 显示界面

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="param1"></param>
    public  void Show(params object[] objs)
    {
        CacheGameobject.SetActive(true);

        OnShow(objs);
    }

    #endregion

    #region 隐藏这个界面

    /// <summary>
    /// 隐藏界面
    /// </summary>
    public void Hide(params object[] objs)
    {
        CacheGameobject.SetActive(false);
        OnHide(objs);
    }

    #endregion

    #region 删除这个界面

    /// <summary>
    /// 当前脚本被删除
    /// </summary>
    protected virtual void OnDestroy()
    {
        
    }
    #endregion

    #region 外部需要实现的东西


    /// <summary>
    /// 最早调用 =-- 初始化函数
    /// </summary>
    protected abstract void OnInit();

    /// <summary>
    /// 显示或者刷新界面
    /// </summary>
    /// <param name="param1"></param>
    protected abstract void OnShow(params object[] objs);

    /// <summary>
    /// 隐藏当前界面
    /// </summary>
    /// <param name="param1"></param>
    protected abstract void OnHide(params object[] objs);

#endregion

}
