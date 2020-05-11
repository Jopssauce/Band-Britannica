using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCrySkill : Skill
{
    public Buff buffToAdd;

    public GameObject particleEffect;
    public float animYOffset;

    [Header("Animation Variables")]
    public float scaleSize;
    public float duration;

    public override void SkillEffect()
    {
        actor.GetComponent<Character>().Mana -= this.cost;

        buffToAdd.duration = _Intensity.Levels[IntensityLevel - 1].IDatas.First();

        if(!actor.GetComponent<BuffReciever>().FindBuff(buffToAdd))
        {
            actor.GetComponent<BuffReciever>().AddBuff(buffToAdd);
        }
        else
        {
            actor.GetComponent<BuffReciever>().FindBuff(buffToAdd).duration = buffToAdd.duration;
        }
        
        base.SkillEffect();
        
        Vector3 baseScale = actor.transform.lossyScale;
        StartCoroutine(SkillRoutine(baseScale));
    }

    public IEnumerator SkillRoutine(Vector3 baseScale)
    {
        Vector3 pos = actor.transform.position;

        yield return new WaitForSeconds(0.5f);
        Sequence s = DOTween.Sequence();
        Vector3 scale = new Vector3(scaleSize,scaleSize,scaleSize);

        s.Append(actor.transform.DOBlendableScaleBy(scale, duration));
        yield return new WaitForSeconds(duration);

        GameObject effectInstance = Instantiate(particleEffect, pos + (Vector3.up * animYOffset), particleEffect.transform.rotation);
        //effectInstance.transform.SetParent(actor.transform);
        if (audioManager) audioManager.PlaySFX("Cast_War_Cry");
        yield return new WaitForSeconds(2f);

        s.Append(actor.transform.DOScale(baseScale, duration));
        yield return new WaitForSeconds(duration);

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

            Descriptions[InsertIndexes.First()] += IDatas.First().ToString();

            if (a < levelCount - 1)
            {
                Descriptions[InsertIndexes.First()] += "/";
            }
        }

        Descriptions[InsertIndexes.First()] += " turns</color>";
    }
}
