using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrowAxeSkill : RangeSkill
{
    public SingleTargetProjectile projectilePrefab;
    public float numberOfAxeToThrow;
    protected float throwCount;

    [Header("Animation Variables")]
    public float windUp;
    public float windUpDuration;
    public float slam;
    public float slamDuration;

    public override void SkillEffect()
    {
        throwCount = 0;
        StartCoroutine(SkillRoutine());
    }

    public IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        //Rise Up
        Sequence s = DOTween.Sequence();

        //Wind Up
        Vector3 rotate = new Vector3(windUp - actor.transform.rotation.x, -90, 0);
        s.Append(actor.transform.DORotate(rotate, windUpDuration));
        s.Append(actor.transform.DOPunchScale(new Vector3(2,2,2),windUpDuration, 5, 0.5f));
        yield return new WaitForSeconds(windUpDuration);

        //Slam
        rotate = new Vector3(slam + actor.transform.rotation.x, -90, 0);
        s.Append(actor.transform.DORotate(rotate, slamDuration));
        yield return new WaitForSeconds(slamDuration);

        int rng = Random.Range(0, enemies.Length);
        
        if(audioManager) audioManager.PlaySFX("Cast_Throw");
        SingleTargetProjectile projectileInstance = Instantiate(projectilePrefab, actor.transform.position + Vector3.up * 3, projectilePrefab.transform.rotation);
        projectileInstance.destination = enemies[rng].transform.position + Vector3.up * 1;
        projectileInstance.target = enemies[rng];

        throwCount++;
        if(throwCount == numberOfAxeToThrow) projectileInstance.OnHit.AddListener(SkillEffectEnd);
        
        s.Append(actor.transform.DORotate(new Vector3(0,-90,0), 0.1f));
        yield return new WaitForSeconds(1f);
        if(throwCount != numberOfAxeToThrow) StartCoroutine(SkillRoutine());
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