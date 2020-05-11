using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class EnemySlashSkill : Skill, IDamager
{
    public float damage = 5.0f;

    public GameObject slashEffect;
    public float animYOffset;

    [Header("Animation Variables")]
    public float riseRange;
    public float riseDuration;
    public float targetOffset;
    public float moveToDuration;
    public float  windUp;
    public float windUpDuration;
    public float slam;
    public float slamDuration;
    public float returnDuration;

    public void DealDamage(IHealth target, float amount)
    {
        if(audioManager) audioManager.PlaySFX("Hit");
        target.DeductHealth(amount);
    }

    public override void SkillEffect()
    {
        Vector3 origin = actor.transform.position;

        CustomCharacterController cc = actor.GetComponent<CustomCharacterController>();

        StartCoroutine(SkillRoutine(cc, origin));
        
        base.SkillEffect();
    }

    //TODO: Change this to something more flexible
    public IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        //Move towards
        yield return new WaitForSeconds(0.5f);

        Sequence s = DOTween.Sequence();
        s.Append(actor.transform.DOMoveY(riseRange, riseDuration));
        yield return new WaitForSeconds(riseDuration);
        
        //Move To Enemy
        Vector3 movePoint = new Vector3(enemy.transform.position.x - targetOffset, enemy.transform.position.y, enemy.transform.position.z);
        s.Append(actor.transform.DOMove(movePoint, moveToDuration));
        yield return new WaitForSeconds(moveToDuration);

        //Wind Up
        Vector3 rotate = new Vector3(windUp - actor.transform.rotation.x, -90, 0);
        s.Append(actor.transform.DORotate(rotate, windUpDuration));
        s.Append(actor.transform.DOPunchScale(new Vector3(2,2,2),windUpDuration, 5, 0.5f));
        yield return new WaitForSeconds(windUpDuration);

        //Slam
        rotate = new Vector3(slam + actor.transform.rotation.x, -90, 0);
        s.Append(actor.transform.DORotate(rotate, slamDuration));

        DealDamage(enemy, damage);
        OnActivated.Invoke();

        Quaternion rotation = Quaternion.Euler(-90,0, 180);
        GameObject effectInstance = Instantiate(slashEffect, actor.transform.position + (Vector3.up*animYOffset), rotation);
        yield return new WaitForSeconds(slamDuration);
        
        s.Append(actor.transform.DORotate(new Vector3(0,-90,0), 0.1f));
        movePoint = origin;
        s.Append(actor.transform.DOMove(movePoint, returnDuration).OnComplete(SkillEffectEnd));
        yield return new WaitForSeconds(returnDuration);
        actor.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
    }

    protected override void UpdateSkillDescription()
    {
        Descriptions[InsertIndexes.First()] = "<color=red>";

        int levelCount = _Intensity.Levels.Count;

        for (int a = 0; a < levelCount; a++)
        {
            List<float> IDatas = _Intensity.Levels[a].IDatas;
            int count = IDatas.Count;

            Descriptions[InsertIndexes.First()] += IDatas.First().ToString();

            if (a < levelCount - 1)
            {
                Descriptions[InsertIndexes.First()] += "/";
            }
        }

        Descriptions[InsertIndexes.First()] += " damage</color>";
    }
}