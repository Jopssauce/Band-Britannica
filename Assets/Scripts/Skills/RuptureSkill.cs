using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RuptureSkill : MeleeSkill
{
    public FunctionBuff _BleedDebuff;

    [Header("Animation Variables")]
    public float targetOffset;
    public float moveToDuration;
    public float  windUp;
    public float windUpDuration;
    public float slam;
    public float slamDuration;
    public float returnDuration;

    public float InitAmount = 10;
    public float InitDur = 2;

    public override void SkillEffect()
    {
        Vector3 origin = actor.transform.position;
        StartCoroutine(SkillRoutine(actor.GetComponent<CustomCharacterController>(), origin));
    }

    public override IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        yield return new WaitForSeconds(0.5f);
        
        Sequence s = DOTween.Sequence();
        
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

        DealDamage(enemy, Damage);

        //Add Bleed buff to enemy
        BuffReciever bReceiver = enemy.GetComponent<BuffReciever>();

        if (bReceiver.FindBuff(_BleedDebuff) == false) bReceiver.AddBuff(_BleedDebuff);
        else bReceiver.FindBuff(_BleedDebuff).duration = _BleedDebuff.duration;

        OnActivated.Invoke();
        
        if(audioManager) audioManager.PlaySFX("Hit");

        yield return new WaitForSeconds(slamDuration);
        
        s.Append(actor.transform.DORotate(new Vector3(0,90,0), 0.1f));
        movePoint = origin;
        s.Append(actor.transform.DOMove(movePoint, returnDuration).OnComplete(SkillEffectEnd));
        yield return new WaitForSeconds(returnDuration);
        cc.OnDestinationReach.Invoke();
    }

    protected override void UpdateSkillDescription()
    {
        Descriptions[InsertIndexes.First()] = "<color=blue>";

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

        Descriptions[InsertIndexes[1]] = "<color=blue>" + InitAmount + " damage </color>";

        Descriptions[InsertIndexes.Last()] = "<color=blue>(" + InitDur + " turns)</color>";
    }
}
