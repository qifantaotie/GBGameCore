using UnityEngine;
using System.Collections;

public class EventTest : IEventListener
{
    
    
    public void HandleEvnet(string evnetName, params object[] objs)
    {
        if (evnetName == "Test")
        {
            Debug.Log("Test收到");
//Debug.Log(objs.Length);
            if (objs.Length==1)
            {
                Debug.Log(objs[0].ToString());
            }
            
        }
    }

    public int EventHandlePriority
    {
        get { return 0; }
    }

    
}