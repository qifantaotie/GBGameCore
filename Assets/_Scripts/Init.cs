using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour {
    void Awake()
    {
        gameObject.AddComponent<AppMgr>();
        gameObject.AddComponent<ResMgr>();
        gameObject.AddComponent<EventMgr>();
        gameObject.AddComponent<TableDataMgr>();
        gameObject.AddComponent<UIMgr>();
    }
}
