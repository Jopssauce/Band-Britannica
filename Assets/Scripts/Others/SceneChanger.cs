using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string SceneName = "Map Scene";
    public LoadSceneMode SceneMode = LoadSceneMode.Additive;
    SceneController sc;
    CombatManager combat;

    private void Start()
    {
        sc = SceneController.instance;
    }

    public void ChangeScene()
    {
        combat = SingletonManager.Get<CombatManager>();
        if(combat.currentStage.missionNumber == "Final Mission" && combat.currentStage.completed) SceneName = "Ending Scene";
        
        //Remove All Combat Scene Related
        SingletonManager.RemoveAllOccasionalType();

        sc.LoadScene(SceneName);
    }
}
