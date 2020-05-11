using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballExplodeAudio : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();

        if(audioManager) audioManager.PlaySFX("Hit_Fireball");
    }


}
