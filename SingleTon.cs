using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例脚本
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleTon<T> :MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<T>(true);
                if(instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
                else
                {
                    if (!instance.gameObject.activeSelf)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        instance = obj.AddComponent<T>(); 
                    }
                }
            }

            return instance;
        }
    }
}

public class SingleTonByNew<T> where T : new()
{
    private static T instance;
    public static T Instance 
    {
        get
        {
            if(instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }

}

