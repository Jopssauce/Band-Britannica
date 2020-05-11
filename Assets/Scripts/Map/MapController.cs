using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    AudioManager audioManager;

    void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayBGM("BGM_MAP");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
