using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject[] buttons;

    void Awake()
    {
        //SingletonManager.Register<UIController>(this, SingletonType.Persistent);
    }

    public void DisableAllColliders()
    {
        foreach (var item in buttons)
        {
            if(item.GetComponent<MapNodeClick>())
            {
                if(item.GetComponent<MapNodeClick>().info.unlocked) item.GetComponent<BoxCollider>().enabled = false;
            }
            else item.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void EnableAllColliders()
    {
        foreach (var item in buttons)
        {
            if(item.GetComponent<MapNodeClick>())
            {
                if(item.GetComponent<MapNodeClick>().info.unlocked) item.GetComponent<BoxCollider>().enabled = true;
            }
            else item.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void DisableAllObjects()
    {
        foreach (var item in buttons)
        {
            item.SetActive(false);
        }
    }

    public void EnableAllObjects()
    {
        foreach (var item in buttons)
        {
            item.SetActive(true);
        }
    }
}
