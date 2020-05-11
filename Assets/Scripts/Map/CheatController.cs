using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatController : MonoBehaviour
{
    public List<MapNodeClick> stages;
    protected int counter = 0;
    protected UIController uiController;

    void Start()
    {
        uiController = this.GetComponent<UIController>();
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F5))
        {
            if(counter == (stages.Count-1)) return;

            stages[counter].info.completed = true;
            stages[counter].CheckToSpawnParticles();

            stages[counter+1].info.unlocked = true;
            stages[counter+1].CheckToSpawnParticles();

            uiController.EnableAllColliders();
            
            counter++;
        }
    }
}
