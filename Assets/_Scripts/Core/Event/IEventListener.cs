using UnityEngine;
using System.Collections;

public interface IEventListener
{
    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="evnetName">消息名称</param>
    /// <param name="objs">参数</param>
    /// <returns>是否中断消息</returns>
    void HandleEvnet(string evnetName,params object[] objs);

    /// <summary>
    /// 消息处理优先级
    /// </summary>
    int EventHandlePriority { get; }
}