using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SwitchScene : MonoBehaviour
{
    public string sceneToTransition;
    SceneController sc;
    private void Start()
    {
        sc = SceneController.instance;
    }

    public void LoadStage(string stage)
    {
        sc.LoadStage(stage);
    }

    public void LoadScene(string stage)
    {
        sc.LoadScene(stage);
    }
}