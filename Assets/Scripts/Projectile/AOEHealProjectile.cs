using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AOEHealProjectile : Projectile
{
    public Character[] targets;
    [Header("Animation Variables")]
    public float arcHeight;
    public float duration;

    protected override void Start()
    {
        StartCoroutine(ProjectileRoutine());
        base.Start();
    }

    public override void OnProjectileHit()
    {
        base.OnProjectileHit();
        foreach (var target in targets)
        {
            Heal(target, Damage);
        }
        
    }

    public void Heal(IHealth target, float amount)
    {
        target.AddHealth(amount);
    }

    public IEnumerator ProjectileRoutine()
    {
        Sequence s = DOTween.Sequence();
        s.Append(this.transform.DOJump(destination, arcHeight, 1, duration));
        yield return new WaitForSeconds(duration);

        if(audioManager) audioManager.PlaySFX("Hit_Throw_Potion");

        OnProjectileHit();
        DestroyProjectile();
    }
}
