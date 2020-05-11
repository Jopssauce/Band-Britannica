using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DamageEvent : UnityEvent<IHealth, float> { }

public interface IDamager
{
    void DealDamage(IHealth target, float amount);
}
