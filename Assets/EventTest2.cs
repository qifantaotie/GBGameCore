using UnityEngine;
using System.Collections;

public class EventTest2 : IEventListener {


    public void HandleEvnet(string evnetName, params object[] objs)
    {
        if (evnetName=="Test2")
        {
            Debug.Log("Test2收到");
        }
    }

    public int EventHandlePriority
    {
        get { return 2; }
    }
}
