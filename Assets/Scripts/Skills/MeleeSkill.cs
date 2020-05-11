using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeSkill : Skill, IDamager
{
    public float BaseDamage = 0.0f;

    public virtual void DealDamage(IHealth enemy ,float amount)
    {
        if(audioManager) audioManager.PlaySFX("Hit");
        enemy.DeductHealth(amount);
    }

    public override void SkillEffect()
    {
        Vector3 origin = actor.transform.position;
        actor.GetComponent<Character>().Mana -= this.cost;

        CustomCharacterController cc = actor.GetComponent<CustomCharacterController>();

        StartCoroutine(SkillRoutine(cc, origin));
        
        base.SkillEffect();
    }

    public virtual IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        //Move towards
        cc.destination = (enemy.transform.position);
        cc.stopRange = 1;
        while (true)
        {
            if (Vector3.Distance(cc.transform.position, enemy.transform.position) <= cc.stopRange)
            {
                DealDamage(enemy, IntensityLevel);
                OnActivated.Invoke();
                break;
            }
            yield return null;
        }
        CameraShake camShake = Camera.main.GetComponent<CameraShake>();
        camShake.CameraShakeByDefault();

        //Move Back
        cc.destination = (origin);
        cc.OnDestinationReach.AddListener(SkillEffectEnd);
    }

    public virtual float Damage
    {
        get
        {
            return _Intensity.Levels[IntensityLevel - 1].IDatas.First();
        }
    }
}