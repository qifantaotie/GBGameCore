using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmorSerialize : ScriptableObject
{
    public List<Armor> mArmors = new List<Armor>();

#if UNITY_EDITOR
    public void SetDatas(object[] obj)
    {
        mArmors.Clear();
        foreach (object o in obj)
        {
            mArmors.Add(o as Armor);
        }
    }
#endif
}
