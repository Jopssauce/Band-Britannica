using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class IceShieldSkill : Skill
{
    public FunctionBuff _ReduceDamageBuff;

    [Header("Animation Variables")]
    public float riseRange;
    public float riseDuration;
    public float spinTime;
    public float spinDuration;

    public float InitDur = 1;

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
        s.Append(actor.transform.DOBlendableRotateBy(rotate, spinTime).OnComplete(Effect));
        if(audioManager) audioManager.PlaySFX("Cast_Ice_Shield");

        yield return new WaitForSeconds(spinDuration);
        
        s.Append(actor.transform.DOMove(origin, riseDuration));

        OnActivated.Invoke();

        base.SkillEffectEnd();
        actor.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
    }

    public void Effect()
    {
        actor.GetComponent<Character>().Mana -= cost;

        _ReduceDamageBuff.BaseValue = _Intensity.Levels[IntensityLevel - 1].IDatas.First();

        foreach (var item in SingletonManager.Get<FieldManager>().SpawnedAllies)
        {
            BuffReciever bReceiver = item.GetComponent<BuffReciever>();

            if(bReceiver.FindBuff(_ReduceDamageBuff) == false) bReceiver.AddBuff(_ReduceDamageBuff);
        }
    }

    protected override void UpdateSkillDescription()
    {
        Descriptions[InsertIndexes.First()] = "<color=blue>";

        int levelCount = _Intensity.Levels.Count;

        for (int a = 0; a < levelCount; a++)
        {
            List<float> IDatas = _Intensity.Levels[a].IDatas;
            int count = IDatas.Count;

            Descriptions[InsertIndexes.First()] += (IDatas.First() * 100).ToString();

            if (a < levelCount - 1)
            {
                Descriptions[InsertIndexes.First()] += "/";
            }
        }

        Descriptions[InsertIndexes.First()] += "%</color>";

        Descriptions[InsertIndexes.Last()] = "<color=blue>("+ InitDur +" turn)</color>";
    }
}
