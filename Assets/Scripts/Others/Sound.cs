using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;

    public AudioClip Clip;

    [Range(0.0f,1.0f)]
    public float Volume;

    public bool Loop;

    [HideInInspector]
    public AudioSource Source;
}
