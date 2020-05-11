using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CharacterType
{
    Red,Green,Blue
}

public class Character : MonoBehaviour, IHealth, IDamager
{
    public string CharacterName;
    public float Health;
    public float maxHealth;
    public float Mana;
    public float maxMana;
    public float MaxTurns;
    public float CurrentTurns;
    public float Damage;
    public float Shield;
    public bool IsAlive = true;
    public bool IsUndead = false;

    public ParticleSystem characterHighlight;

    public HealthEvent OnAddHealth;
    public HealthEvent OnDeductHealth;
    public DamageEvent OnDealDamage;
    public UnityEvent OnDestroy;

    public CharacterType Type;

    protected AudioManager audioManager;

    void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();
    }

    public void AddHealth(float amount)
    {
        Health += amount;

        Health = Mathf.Clamp(Health, 0, maxHealth);

        OnAddHealth.Invoke(amount);
    }

    public void DealDamage(IHealth target, float amount)
    {
        target.DeductHealth(amount);
        OnDealDamage.Invoke(target, amount);
    }

    public void DeductHealth(float amount)
    {
        //Zeroed the damage received XD
        if (IsUndead == true) amount = 0;

        if(CheckUndeadBuffExist() == true && Health - amount <= 0)
        {
            IsUndead = true;
        }

        //Reduce damage if applicable
        amount -= (amount * Shield);
        amount = Mathf.Clamp(amount, 0, amount);

        //RoundOff Damage
        amount = (float)Math.Round(amount, 1);

        Health -= amount;
        Health = Mathf.Clamp(Health, 0, maxHealth);

        //Bool Variable
        if (Health <= 0 && !IsUndead) IsAlive = false;

        OnDeductHealth.Invoke(amount);
        StartCoroutine(this.GetComponent<CustomCharacterController>().TakeDamageAnimation());
    }

    public void Destroy()
    {
        PlayDeathSound();
        OnDestroy.Invoke();
        StartCoroutine(this.GetComponent<CustomCharacterController>().DeathAnimation());
        Destroy(this.gameObject);
    }

    public void PlayDeathSound()
    {
        if(CharacterName == "Mage")
        {
            if(audioManager) audioManager.PlaySFX("Death_Woman");
        }
        else
        {
            int rng = UnityEngine.Random.Range(1,2);
            if(audioManager) audioManager.PlaySFX("Death_Man_"+rng);
        }
    }

    public bool CheckUndeadBuffExist()
    {
        BuffReciever br = GetComponent<BuffReciever>();

        Buff buff = br.buffs.Find(b => b.GetType() == typeof(UndyingBuff));

        if (buff == null) return false;
        else return true;
    }
}
