using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SingleTargetProjectile : Projectile
{
    public Character target;
    [Header("Animation Variables")]
    public float moveToDuration;
    public bool spin;
    public float spinSpeed;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(ProjectileRoutine());
    }

    public override void OnProjectileHit()
    {
        DealDamage(target, Damage);

        base.OnProjectileHit();
    }

    public override void WhileProjectileMoving()
    {
        
        if(spin)
        {
            this.transform.Rotate(0f,0f,spinSpeed);
        }
    }

    public void DealDamage(IHealth target, float amount)
    {
        if(audioManager) audioManager.PlaySFX("Hit");
        target.DeductHealth(amount);
    }

    public IEnumerator ProjectileRoutine()
    {
        Sequence s = DOTween.Sequence();
        s.Append(this.transform.DOMove(destination, moveToDuration).SetEase(Ease.Linear).OnUpdate(WhileProjectileMoving));
        yield return new WaitForSeconds(moveToDuration);

        OnProjectileHit();
        DestroyProjectile();
    }
}
