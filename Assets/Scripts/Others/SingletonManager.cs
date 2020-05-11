using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SingletonType
{
    Persistent,
    Occasional
}

[System.Serializable]
public struct SingletonInfo
{
    public MonoBehaviour Instance;
    public SingletonType Type;

    public SingletonInfo(MonoBehaviour instance, SingletonType type)
    {
        Instance = instance;
        Type = type;
    }
}

public class SingletonManager : MonoBehaviour
{
    private static SingletonManager ThisInstance;

    //Variables
    private static List<SingletonInfo> InstancesList = new List<SingletonInfo>();

    //Own Functions
    private void Awake()
    {
        if (ThisInstance == null)
        {
            ThisInstance = this;

            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    } 

    //Usable Functions
    /// <summary>
    /// Register a Manager as singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="manager"></param>
    public static void Register<T> (T manager, SingletonType type) where T : MonoBehaviour
    {
        SingletonInfo singletonInfo = new SingletonInfo(manager, type);

        if (InstancesList.Count == 0)
        {
            InstancesList.Add(singletonInfo);
        }
        else
        {
            if(InstancesList.Contains(singletonInfo) == false)
            {
                //Debug.Log("Added " + manager + " as Singleton.");
                InstancesList.Add(singletonInfo);
            }
            else Debug.Log(manager + " Manager Already Existed. Ignored.");
        }
    }
    /// <summary>
    /// Get the singleton instance registered by type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Get<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < InstancesList.Count; i++)
        {
            if (InstancesList[i].Instance.GetType() == typeof(T))
            {
                return (T)InstancesList[i].Instance;
            }
        }

        return null;
    }
    /// <summary>
    /// Unregister a singleton registered
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Remove<T>()
    {
        for (int i = 0; i < InstancesList.Count; i++)
        {
            if(InstancesList[i].Instance.GetType() == typeof(T))
            {
                InstancesList.Remove(InstancesList[i]);
                break;
            }
        }
    }
    /// <summary>
    /// Unregister all singleton registered in SingletonType.Occasional
    /// </summary>
    public static void RemoveAllOccasionalType()
    {
        int count = InstancesList.Count;

        List<SingletonInfo> toBeRemove = new List<SingletonInfo>();

        for (int i = 0; i < count; i++)
        {
            if(InstancesList[i].Type == SingletonType.Occasional)
            {
                toBeRemove.Add(InstancesList[i]);
            }
        }

        for (int i = 0; i < toBeRemove.Count; i++)
        {
            InstancesList.Remove(toBeRemove[i]);
        }
    }
}
