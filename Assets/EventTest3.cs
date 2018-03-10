using UnityEngine;
using System.Collections;

public class EventTest3 : IEventListener
{
    public void HandleEvnet(string evnetName, params object[] objs)
    {
        if (evnetName == "Test3")
        {
            Debug.Log("Test3收到");
        }
        if (evnetName=="Test")
        {
            Debug.Log("Test3收到");
        }
    }

    public int EventHandlePriority
    {
        get { return 1; }
    }
}
