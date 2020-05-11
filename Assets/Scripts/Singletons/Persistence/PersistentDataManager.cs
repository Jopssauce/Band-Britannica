using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager instance;
    public List<Character> characters;
    public List<StageInfo> stageStates;

    public void Awake()
    {
        instance = this;
        
        PersistentDataManager.instance.stageStates[0].completed = false;
        for (int i = 1; i < PersistentDataManager.instance.stageStates.Count; i++)
        {
            PersistentDataManager.instance.stageStates[i].completed = false;
            PersistentDataManager.instance.stageStates[i].unlocked = false;
        }
    }
}
