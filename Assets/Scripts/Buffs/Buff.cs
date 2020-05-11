using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BuffType
{
    //Doesn't affect its turn
    Standard,

    //Affect its turn
    Special
}

public class Buff : MonoBehaviour
{
    public string buffName;
    public float duration;
    public BuffType type;
    public bool ActivateRightAway = true;

    [Header("Particle Settings")]
    public GameObject ParticleFX;
    public float yOffSet;

    [HideInInspector]
    public GameObject ParticleInstance;
    [HideInInspector]
    public Character Owner;

    protected CombatManager combatManager;
    protected AudioManager audioManager;

    protected BuffReciever rcvr;

    public UnityEvent OnActivate;

    public virtual void Activate(BuffReciever reciever) 
    {
        OnActivate.Invoke();

        rcvr = reciever;
        combatManager = SingletonManager.Get<CombatManager>();
        audioManager = SingletonManager.Get<AudioManager>();

        //Instantiate Particle FX
        if (ParticleFX) ParticleInstance = Instantiate(ParticleFX, rcvr.transform.position + (Vector3.up * yOffSet), ParticleFX.transform.rotation, rcvr.transform);
    }

    public virtual void Deactivate(BuffReciever reciever) 
    {
        reciever.RemoveBuff(this);   
    }

    public virtual void BuffEffect()
    {
        duration--;

        if (duration == 0)
        {
            Deactivate(rcvr);

            //Destroy Particle FX
            if (ParticleInstance) Destroy(ParticleInstance);
        }
    }
}