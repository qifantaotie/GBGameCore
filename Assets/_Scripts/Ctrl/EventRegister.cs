using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EventRegister {

    public void RegisterEvent()
    {
        EventMgr.Instance.RegisterEvent("Test",new List<IEventListener>(){new EventTest(),new EventTest3()},0 );
        EventMgr.Instance.RegisterEvent("Test1",new List<IEventListener>(){new EventTest1()},5,"Test" );
        EventMgr.Instance.RegisterEvent("Test2",new List<IEventListener>(){new EventTest2()},2,"Test" );
        EventMgr.Instance.RegisterEvent("Test3",new List<IEventListener>(){new EventTest3()},3,"Test" );
    }
}
