using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AOEProjectile : Projectile, IDamager
{
    public GameObject particleOnHit;
    public Character[] targets;

    [Header("Animation Variables")]
    public float riseRange;
    public float riseDuration;
    public float moveToDuration;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(ProjectileRoutine());
    }
    public override void OnProjectileHit()
    {
        foreach (var target in targets)
        {
            DealDamage(target, Damage);
        }
        if (this.particleOnHit != null)
        {
            Instantiate(this.particleOnHit, this.transform.position, this.particleOnHit.transform.rotation);
        }
        base.OnProjectileHit();
    }

    public void DealDamage(IHealth target, float amount)
    {
        target.DeductHealth(amount);
    }

    public IEnumerator ProjectileRoutine()
    {
        Sequence s = DOTween.Sequence();
        s.Append(this.transform.DOMoveY(riseRange, riseDuration));
        yield return new WaitForSeconds(riseDuration);

        s.Append(this.transform.DOMove(destination, moveToDuration).OnUpdate(WhileProjectileMoving));
        yield return new WaitForSeconds(moveToDuration);

        OnProjectileHit();
        DestroyProjectile();
    }
}
