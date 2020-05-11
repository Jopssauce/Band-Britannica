using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public GameObject panelToDisplay;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();
    }
    public void EnablePanel()
    {
        panelToDisplay.SetActive(true);
    }
    public void DisablePanel()
    {
        panelToDisplay.SetActive(false);
    }
    public void RestartScene()
    {
        if(audioManager) audioManager.PlaySFX("Button_Click");
        SingletonManager.RemoveAllOccasionalType();
        string scene = SceneManager.GetActiveScene().name;
        SceneController.instance.ReloadStage(scene);
    }
}
