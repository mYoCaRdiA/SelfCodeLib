using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

/// <summary>
/// 事件控制器
/// </summary>
public class EventControl:SingleTonByNew<EventControl>
{
    private string curEventName = string.Empty;

    public delegate void Action(params object[] param);
    private Dictionary<string, Action> eventDic = new Dictionary<string, Action>();
   
    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    /// <param name="isStep">步骤事件才能设置为当前事件</param>
    public void AddEvent(string eventName,Action action,bool isStep = false)
    {
     
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, null);
        }

        if (!eventDic.ContainsValue(action))
        {
            eventDic[eventName] += action;
        }

        if (isStep)
        {
            curEventName = eventName;
        }
    }

    public void ChangeEvent(string eventKey,Action action)
    {
        if (!eventDic.ContainsKey(eventKey))
        {
            eventDic.Add(eventKey, null);
        }

        eventDic[eventKey] = action;
    }

    public void ExecuteEvent(string eventKey, bool isDelete = true, params object[] param)// )
    {
        if (eventDic.ContainsKey(eventKey))
        {
            Action action = eventDic[eventKey];
            if(action != null)
            {
                action(param);
            }
            else
            {
             Debug.LogError(eventKey + "为空");
            }
            if (isDelete)
            {
                DeleteEvent(eventKey);
            }
        }
        else
        {
            Debug.Log("未找到对应的事件" + eventKey);
        }
    }

    public void DeleteEvent(string key)
    {
        if (eventDic.ContainsKey(key))
        {
            //待验证不知道会不会造成内存泄漏
            eventDic.Remove(key);
        }
        else
        {
            Debug.Log("未找到对应的事件 " + key);
        }
    }

    public void DeleteCurEvent()
    {
        if(curEventName != string.Empty)
        {
            DeleteEvent(curEventName);
        }
    }

    public bool IsExist(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            return true;
        }

        return false;
    }
}
