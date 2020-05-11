using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSkill : RangeSkill
{
    public AOEProjectile projectilePrefab;

    public override void SkillEffect()
    {
        if(audioManager) audioManager.PlaySFX("Launch_Fireball");
        Bounds bound = new Bounds();
        AOEProjectile projectile = Instantiate(projectilePrefab, actor.transform.position, projectilePrefab.transform.rotation);
        projectile.maxSpeed = 1;
        projectile.acceleration = 1.5f;
        projectile.BaseValue = _Intensity.Levels[IntensityLevel - 1].IDatas.First();

        foreach (var item in enemies)
        {
            bound.Encapsulate(item.transform.position);
        }
        projectile.destination = bound.center + Vector3.up * 1;

        projectile.targets = enemies;
        projectile.OnHit.AddListener(SkillEffectEnd);
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
    }
}