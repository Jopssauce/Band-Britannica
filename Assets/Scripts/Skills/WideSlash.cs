using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WideSlash : Skill, IDamager
{
    public float damage = 20.0f;
    public GameObject wideSlashEffect;
    public float animYOffset;

    [Header("Animation Variables")]
    public float riseRange;
    public float riseDuration;
    public float movePointYOffset;
    public float moveToDuration;
    public float  windUp;
    public float windUpDuration;
    public float heliSpeed;
    public float heliDuration;
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

    public IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        yield return new WaitForSeconds(0.5f);
        
        float facing = -90;
        //Rise Up
        Sequence s = DOTween.Sequence();

        s.Append(actor.transform.DOMoveY(riseRange, riseDuration));
        yield return new WaitForSeconds(riseDuration);

        //Move To Enemy
        Bounds bound = new Bounds();
        foreach (var item in enemies)
        {
            bound.Encapsulate(item.transform.position);
        }

        Vector3 movePoint = bound.center;
        movePoint.y += movePointYOffset;
        s.Append(actor.transform.DOMove(movePoint, moveToDuration));
        yield return new WaitForSeconds(moveToDuration);

        //Wind Up
        Vector3 rotate = new Vector3(windUp - actor.transform.rotation.x, facing, 0);
        s.Append(actor.transform.DORotate(rotate, windUpDuration));
        s.Append(actor.transform.DOPunchScale(new Vector3(2,2,2),windUpDuration, 5, 0.5f));
        yield return new WaitForSeconds(windUpDuration);

        //Slam
        rotate = new Vector3(0, 180, 0);
        s.Append(actor.transform.DOBlendableRotateBy(rotate, heliSpeed));
        rotate = new Vector3(0, 180, 0);
        s.Append(actor.transform.DOBlendableRotateBy(rotate, heliSpeed));
        rotate = new Vector3(0, 180, 0);
        s.Append(actor.transform.DOBlendableRotateBy(rotate, heliSpeed));
        rotate = new Vector3(0, 180, 0);
        s.Append(actor.transform.DOBlendableRotateBy(rotate, heliSpeed));

        foreach (var item in enemies)
        {
            DealDamage(item, damage);   
        }
        GameObject effectInstance = Instantiate(wideSlashEffect, actor.transform.position + (Vector3.up*animYOffset), wideSlashEffect.transform.rotation);
        OnActivated.Invoke();

        yield return new WaitForSeconds(heliDuration);
        
        s.Append(actor.transform.DORotate(new Vector3(0, facing,0), 0.1f));
        movePoint = origin;
        s.Append(actor.transform.DOMove(movePoint, returnDuration).OnComplete(SkillEffectEnd));
        yield return new WaitForSeconds(returnDuration);

        cc.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
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
