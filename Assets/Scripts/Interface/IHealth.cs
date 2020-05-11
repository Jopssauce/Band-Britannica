using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthEvent : UnityEvent<float> { }

public interface IHealth
{
    void AddHealth(float amount);
    void DeductHealth(float amount);
}
