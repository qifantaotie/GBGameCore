using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TableData : Singleton<TableData>
{

    public ArmorSerialize Armor{ get; set; }

    public  void LoadDataFinish()
    {
        Debug.Log("数据表加载完成");
        for (int i = 0; i < Armor.mArmors.Count; i++)
        {
            Debug.Log(Armor.mArmors[i].ArmorName);
        }
    }

    /// <summary>
    /// 注册数据表
    /// </summary>
    public void RegisterTableData()
    {
        TableDataMgr.Instance.AddTableName("ArmorSerialize");
    }

    /// <summary>
    /// 解析数据表
    /// </summary>
    /// <param name="so"></param>
    public void ResolveTableData(ScriptableObject so)
    {
        if (so is ArmorSerialize)
        {
            Armor = so as ArmorSerialize;
        }
    }

    
}
