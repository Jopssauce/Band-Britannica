using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Button start;
    AudioManager audioManager;

    void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayBGM("BGM_TITLE");
        }
    }


    void Update()
    {
        
    }
}
