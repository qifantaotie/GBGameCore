using UnityEngine;
using System.Collections;

public class EventTest1 : IEventListener {


    public void HandleEvnet(string evnetName, params object[] objs)
    {
        if (evnetName == "Test1")
        {
            Debug.Log("Test1收到");
        }
    }

    public int EventHandlePriority
    {
        get { return 1; }
    }
}
