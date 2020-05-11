using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPotionSkill : RangeSkill
{
    public AOEHealProjectile projectilePrefab;

    public override void SkillEffect()
    {
        Bounds bound = new Bounds();
        if(audioManager) audioManager.PlaySFX("Cast_Throw");

        AOEHealProjectile projectile = Instantiate(projectilePrefab, actor.transform.position, projectilePrefab.transform.rotation);
        projectile.BaseValue = _Intensity.Levels[IntensityLevel - 1].IDatas.First();

        foreach (var item in allies)
        {
            bound.Encapsulate(item.transform.position);
        }
        projectile.destination = bound.center;

        projectile.targets = allies;
        projectile.OnHit.AddListener(SkillEffectEnd);
    }

    protected override void UpdateSkillDescription()
    {
        Descriptions[InsertIndexes.First()] = "<color=blue>";

        int levelCount = _Intensity.Levels.Count;

        Debug.Log(levelCount);

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

        Descriptions[InsertIndexes.First()] += " Health</color>";
    }
}
