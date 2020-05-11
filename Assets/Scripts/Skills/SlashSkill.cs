using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class SlashSkill : MeleeSkill
{
    public GameObject slashEffect;
    public float animYOffset;

    [Header("Animation Variables")]
    public Ease ease;
    public float riseRange;
    public float riseDuration;
    public float targetOffset;
    public float moveToDuration;
    public float  windUp;
    public float windUpDuration;
    public float slam;
    public float slamDuration;
    public float returnDuration;

    public override void SkillEffect()
    {
        Vector3 origin = actor.transform.position;
        StartCoroutine(SkillRoutine(actor.GetComponent<CustomCharacterController>(), origin));
    }

    public override IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        yield return new WaitForSeconds(0.5f);
        
        //Rise Up
        Sequence s = DOTween.Sequence();
        s.Append(actor.transform.DOMoveY(riseRange, riseDuration));
        yield return new WaitForSeconds(riseDuration);
        
        //Move To Enemy
        Vector3 movePoint = new Vector3(enemy.transform.position.x - targetOffset, enemy.transform.position.y, enemy.transform.position.z);
        s.Append(actor.transform.DOMove(movePoint, moveToDuration));
        yield return new WaitForSeconds(moveToDuration);

        //Wind Up
        Vector3 rotate = new Vector3(windUp - actor.transform.rotation.x, 90, 0);
        s.Append(actor.transform.DORotate(rotate, windUpDuration));
        s.Append(actor.transform.DOPunchScale(new Vector3(2,2,2),windUpDuration, 5, 0.5f));
        yield return new WaitForSeconds(windUpDuration);

        //Slam
        rotate = new Vector3(slam + actor.transform.rotation.x, 90, 0);
        s.Append(actor.transform.DORotate(rotate, slamDuration));

        DealDamage(enemy, BaseDamage);
        OnActivated.Invoke();
        Quaternion rotation = Quaternion.Euler(-90,0, 0);
        GameObject effectInstance = Instantiate(slashEffect, actor.transform.position + (Vector3.up*animYOffset), slashEffect.transform.rotation);
        yield return new WaitForSeconds(slamDuration);
        
        s.Append(actor.transform.DORotate(new Vector3(0,90,0), 0.1f));
        movePoint = origin;
        s.Append(actor.transform.DOMove(movePoint, returnDuration).OnComplete(SkillEffectEnd));
        yield return new WaitForSeconds(returnDuration);
        actor.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
    }
}