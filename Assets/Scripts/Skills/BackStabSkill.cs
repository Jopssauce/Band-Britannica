using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackStabSkill : MeleeSkill
{
    public GameObject appearParticle;
    public GameObject disappearParticle;
    public float animYOffset;
    public float MinimumDamage;

    [Header("Animation Variables")]
    public float riseRange;
    public float riseDuration;
    public float targetOffset;
    public float  windUp;
    public float windUpDuration;
    public float slam;
    public float slamDuration;
    public float retunrDuration;

    public override void DealDamage(IHealth enemy ,float amount)
    {
        //Base on Missing HP
        float newAmount = (this.enemy.maxHealth - this.enemy.Health) * amount;

        //Clamp to Minimum Damage
        newAmount = Mathf.Clamp(newAmount, MinimumDamage, newAmount);
        
        enemy.DeductHealth(newAmount);
        if(audioManager) audioManager.PlaySFX("Hit_BackStab");
    }

    public override void SkillEffect()
    {
        Vector3 origin = actor.transform.position;
        StartCoroutine(SkillRoutine(actor.GetComponent<CustomCharacterController>(), origin));
    }

    public override IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        yield return new WaitForSeconds(0.5f);
        
        string sound;

        int rng = Random.Range(0,100);
        if (rng == 0) sound = "Spoof_BackStab";
        else sound = "Disappear_BackStab";

        Sequence s = DOTween.Sequence();
        s.Append(actor.transform.DOMoveY(riseRange, riseDuration).OnComplete(DisappearParticle));
        yield return new WaitForSeconds(riseDuration);
        if(audioManager) audioManager.PlaySFX(sound);

        actor.transform.position = new Vector3(enemy.transform.position.x + targetOffset, enemy.transform.position.y, enemy.transform.position.z);
        actor.transform.rotation = Quaternion.Euler(0,-90,0);

        AppearParticle();
        
        Vector3 rotate = new Vector3(windUp - actor.transform.rotation.x, -90, 0);
        s.Append(actor.transform.DORotate(rotate, windUpDuration));
        s.Append(actor.transform.DOPunchScale(new Vector3(2,2,2),windUpDuration, 5, 0.5f));
        yield return new WaitForSeconds(windUpDuration);
        
        rotate = new Vector3(slam + actor.transform.rotation.x, -90, 0);
        s.Append(actor.transform.DORotate(rotate, slamDuration));

        DealDamage(enemy, Damage);
        OnActivated.Invoke();

        yield return new WaitForSeconds(slamDuration);
        yield return new WaitForSeconds(0.2f);

        s.Append(actor.transform.DORotate(new Vector3(0,-90,0), retunrDuration).OnComplete(DisappearParticle));
        yield return new WaitForSeconds(retunrDuration);
        if(audioManager) audioManager.PlaySFX(sound);
        
        SkillEffectEnd();
        yield return new WaitForSeconds(0.2f);
        actor.transform.position = origin;
        actor.transform.rotation = Quaternion.Euler(0,90,0);
        yield return new WaitForSeconds(0.2f);
        actor.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
        AppearParticle();
    }
    
    void AppearParticle()
    {
        GameObject effectInstance = Instantiate(appearParticle, actor.transform.position + (Vector3.up*animYOffset), appearParticle.transform.rotation);
    }

    void DisappearParticle()
    {
        GameObject effectInstance = Instantiate(disappearParticle, actor.transform.position + (Vector3.up*animYOffset), disappearParticle.transform.rotation);
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

        Descriptions[InsertIndexes.First()] += "% of the target's Missing HP as damage</color>";
    }
}