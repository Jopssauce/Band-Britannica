using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkill : Skill
{
    public override void SkillEffectEnd()
    {
        CustomCharacterController characterController = actor.GetComponent<CustomCharacterController>();
        //To Give listener that damage is done
        OnActivated.Invoke();
        characterController.OnDestinationReach.Invoke();

        //CameraShake camShake = Camera.main.GetComponent<CameraShake>();
        //camShake.CameraShakeByDefault();
        base.SkillEffectEnd();
    }
}