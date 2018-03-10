using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TableDataMgr : Singleton<TableDataMgr>
{

    /// <summary>
    /// 未加载的数据表包集合
    /// </summary>
    private List<TableDataPack> mTableDataPackList=new List<TableDataPack>();

    void Awake()
    {

        TableData.Instance.RegisterTableData();
        LoadData(TableData.Instance.LoadDataFinish);
    }


    /// <summary>
    /// 添加想要读取的表名
    /// </summary>
    /// <param name="tableName">表名</param>
    public void AddTableName(string tableName)
    {
        mTableDataPackList.Add(new TableDataPack(tableName));
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="loadDataFinish"></param>
    private void LoadData(Action loadDataFinish)
    {
        if (mTableDataPackList.Count==0)
        {
            loadDataFinish();
        }
        else
        {
            for (int i = 0; i < mTableDataPackList.Count; i++)
            {
                Debug.Log(mTableDataPackList[i].TableName);
                ResMgr.Instance.Load<ScriptableObject>(mTableDataPackList[i].TableName, mTableDataPackList[i], false, false);
            }
        }
    }

    /// <summary>
    /// 数据表数据包
    /// </summary>
    private class TableDataPack:ILoadListener
    {
        /// <summary>
        /// 数据表的名称或者数据表路径
        /// </summary>
        public string TableName { get; set; }


        public TableDataPack(string tableName)
        {
            TableName = tableName;
        }


        public void LoadFinish(object asset)
        {
            ScriptableObject so = asset as ScriptableObject;
            if (so != null)
            {
                TableData.Instance.ResolveTableData(so);
                Instance.mTableDataPackList.Remove(this);
                if (Instance.mTableDataPackList.Count == 0)
                {
                    TableData.Instance.LoadDataFinish();
                }
            }
        }

        public void LoadFailure(string assetName)
        {
            Log.Error("加载数据表失败,表名"+assetName);
        }
    }
}
