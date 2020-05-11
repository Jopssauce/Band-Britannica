using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FleetFootedSkill : MeleeSkill
{
    public FunctionBuff _DoubleAttackBuff;

    [Header("Animation Variables")]
    public float riseRange;
    public float riseDuration;
    public float spinTime;
    public float spinDuration;

    public override void SkillEffect()
    {
        Vector3 origin = actor.transform.position;
        StartCoroutine(SkillRoutine(origin));
    }

    public IEnumerator SkillRoutine(Vector3 origin)
    {
        yield return new WaitForSeconds(0.5f);
        
        Sequence s = DOTween.Sequence();
        s.Append(actor.transform.DOMoveY(riseRange, riseDuration));

        yield return new WaitForSeconds(riseDuration);

        Vector3 rotate = new Vector3(0, 180, 0);
        s.Append(actor.transform.DOBlendableRotateBy(rotate, spinTime));
        s.Append(actor.transform.DOBlendableRotateBy(rotate, spinTime));
        s.Append(actor.transform.DOBlendableRotateBy(rotate, spinTime));
        s.Append(actor.transform.DOBlendableRotateBy(rotate, spinTime));
        if (audioManager) audioManager.PlaySFX("Cast_Ice_Shield");

        yield return new WaitForSeconds(spinDuration);

        actor.GetComponent<Character>().Mana -= cost;

        //Buff Settings
        _DoubleAttackBuff.BaseValue = _Intensity.Levels[IntensityLevel - 1].IDatas.First();

        BuffReciever bReceiver = actor.GetComponent<BuffReciever>();

        if (bReceiver.FindBuff(_DoubleAttackBuff) == false) bReceiver.AddBuff(_DoubleAttackBuff);
        else bReceiver.FindBuff(_DoubleAttackBuff).duration = _DoubleAttackBuff.duration;

        s.Append(actor.transform.DOMove(origin, riseDuration));

        yield return new WaitForSeconds(riseDuration);

        OnActivated.Invoke();
        base.SkillEffectEnd();
        actor.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
    }

    protected override void UpdateSkillDescription()
    {
        Descriptions[InsertIndexes.First()] = "<color=blue>";

        int levelCount = _Intensity.Levels.Count;

        for (int a = 0; a < levelCount; a++)
        {
            List<float> IDatas = _Intensity.Levels[a].IDatas;
            int count = IDatas.Count;

            for (int i = 0; i < count; i++)
            {
                var item = IDatas[i];

                Descriptions[InsertIndexes.First()] += (item * 100).ToString();
            }

            if (a < levelCount - 1)
            {
                Descriptions[InsertIndexes.First()] += "/";
            }
        }

        Descriptions[InsertIndexes.First()] += "% chance</color>";
    }
}
