using System.Collections.Generic;
using UnityEngine;

public class EventMgr : Singleton<EventMgr>
{
    private EventRegister mEventRegister;

    /// <summary>
    /// 消息处理列表
    /// </summary>
    private List<EventPack> mEventHandleList=new List<EventPack>();

    /// <summary>
    /// 消息信息字典
    /// </summary>
    private Dictionary<string,EventInfoPack>  mEventInfoPackDic=new Dictionary<string, EventInfoPack>();

    /// <summary>
    /// 消息父子关系字典，键为父消息，值为子消息
    /// </summary>
    private Dictionary<string,List<string>> mEventRelationDic=new Dictionary<string, List<string>>();

    void Awake()
    {
        mEventRegister=new EventRegister();
        mEventRegister.RegisterEvent();
    }

    void Update()
    {
        lock (mEventHandleList)
        {
            while (mEventHandleList.Count!=0)
            {
                EventPack eventPack = mEventHandleList[0];
                mEventHandleList.RemoveAt(0);
                if (eventPack.isSendToChild)
                {
                    TriggerEvent(eventPack.eventName,eventPack.objs);
                }
                else
                {
                    TriggerEventOnlySelf(eventPack.eventName,eventPack.objs);
                }
                
            }
        }
    }

    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="eventName">消息名称</param>
    /// <param name="listeners">回调列表</param>
    /// <param name="eventPriority">消息优先级</param>
    /// <param name="fatherEventName">父消息</param>
    public void RegisterEvent(string eventName, List<IEventListener> listeners, int eventPriority, string fatherEventName=null)
    {if (fatherEventName == null)
            {
                fatherEventName = "EventCentre";
            }
        lock (mEventInfoPackDic)
        {
            
            if (listeners != null)
            {
                ListenerSort(listeners);
                if (!mEventInfoPackDic.ContainsKey(eventName))
                {
                    mEventInfoPackDic.Add(eventName, new EventInfoPack(eventName, listeners, eventPriority, fatherEventName));
                }
                else
                {
                    Log.Debug("此消息已注册过，请勿重名");
                }
            }
        }
        lock (mEventRelationDic)
        {
            GetRelation(eventName, fatherEventName);
            
        }
        
    }

    /// <summary>
    /// 按消息监听器优先级排序
    /// </summary>
    /// <param name="listeners"></param>
    private void ListenerSort(List<IEventListener> listeners)
    {
        for (int i = 0; i < listeners.Count - 1; i++)
        {
            for (int j = 1; j < listeners.Count - i; j++)
            {
                if (listeners[j - 1].EventHandlePriority < listeners[j].EventHandlePriority)
                {
                    IEventListener temp = listeners[j - 1];
                    listeners[j - 1] = listeners[j];
                    listeners[j] = temp;
                }
            }
        }
    }


    /// <summary>
    /// 得到父子消息关系
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="fatherEventName"></param>
    private void GetRelation(string eventName, string fatherEventName)
    {
        
            if (mEventRelationDic.ContainsKey(fatherEventName))
            {
                if (!mEventRelationDic[fatherEventName].Contains(eventName))
                {
                    int temp = mEventRelationDic[fatherEventName].Count;
                    for (int i = 0; i < mEventRelationDic[fatherEventName].Count; i++)
                    {
                        if (mEventInfoPackDic[eventName].eventPriority > mEventInfoPackDic[mEventRelationDic[fatherEventName][i]].eventPriority)
                        {
                            mEventRelationDic[fatherEventName].Insert(i, eventName);
                            break;
                        }
                    }
                    if (mEventRelationDic[fatherEventName].Count==temp)
                    {
                        mEventRelationDic[fatherEventName].Add(eventName);
                    }
                }
            }
            else
            {
                mEventRelationDic.Add(fatherEventName, new List<string>() {eventName});
            }
       
    }


    /// <summary>
    /// 发消息
    /// </summary>
    /// <param name="eventName">消息名称</param>
    /// <param name="isSendToChild">是否发送到子消息</param>
    /// <param name="objs">参数</param>
    public void SendEvent(string eventName,bool isSendToChild,params object[] objs)
    {
        lock (mEventHandleList)
        {
            if (mEventInfoPackDic.ContainsKey(eventName))
            {
                EventPack eventPack = new EventPack(eventName, isSendToChild, objs);
                if (mEventHandleList.Count == 0)
                {
                    mEventHandleList.Add(eventPack);
                }
                else
                {
                    for (int i = 0; i < mEventHandleList.Count; i++)
                    {
                        if (mEventInfoPackDic[eventName].eventPriority == mEventInfoPackDic[mEventHandleList[i].eventName].eventPriority)
                        {
                            mEventHandleList.Insert(i+1, eventPack);
                            break;
                        }
                        if (mEventInfoPackDic[eventName].eventPriority > mEventInfoPackDic[mEventHandleList[i].eventName].eventPriority)
                        {
                            mEventHandleList.Insert(i,eventPack);
                            break;
                        }
                    }
                }
            }
            else
            {
                Log.Debug("消息未注册");
            }
            
        }
        
    }


    /// <summary>
    /// 此消息及其子消息触发
    /// </summary>
    /// <param name="message"></param>
    /// <param name="objs"></param>
    private void TriggerEvent(string message, params object[] objs)
    {
        
        TriggerEventOnlySelf(message,objs);
        TriggerChildEvent(message,objs);
    }

    /// <summary>
    /// 此消息子消息触发
    /// </summary>
    /// <param name="message"></param>
    /// <param name="objs"></param>
    private void TriggerChildEvent(string message, params object[] objs)
    {
        if (mEventRelationDic.ContainsKey(message))
        {
            for (int i = 0; i < mEventRelationDic[message].Count; i++)
            {
                TriggerEventOnlySelf(mEventRelationDic[message][i],objs);
                TriggerChildEvent(mEventRelationDic[message][i], objs);
            }
        }
    }

    /// <summary>
    /// 仅触发此消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="objs"></param>
    private void TriggerEventOnlySelf(string message, params object[] objs)
    {
        if (!mEventInfoPackDic.ContainsKey(message))
        {
            return;
        }
        for (int i = 0; i < mEventInfoPackDic[message].listeners.Count; i++)
        {
            mEventInfoPackDic[message].listeners[i].HandleEvnet(message,objs);
        }
    }

    
    #region 消息包
    /// <summary>
    /// 消息信息包
    /// </summary>
    private class EventInfoPack
    {
        public  string eventName;
        public List<IEventListener> listeners;
        public int eventPriority;
        public string fatherEventName;

        public EventInfoPack(string _eventName, List<IEventListener> _listeners, int _eventPriority, string _fatherEventName)
        {
            this.eventName = _eventName;
            this.eventPriority = _eventPriority;
            this.fatherEventName = _fatherEventName;
            this.listeners = _listeners;
        }
    }

    /// <summary>
    /// 发送消息参数信息
    /// </summary>
    private class EventPack
    {
        public string eventName;
        public object[] objs;
        public bool isSendToChild;

        public EventPack(string _eventName,bool _isSendToChild, params object[] _objs)
        {
            eventName = _eventName;
            isSendToChild = _isSendToChild;
            objs = _objs;
        }
    }
    #endregion
    
}
