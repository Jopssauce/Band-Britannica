using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleAttackBuff : FunctionBuff
{
    public override void Activate(BuffReciever reciever)
    {
        base.Activate(reciever);

        //Add Listeners
        combatManager.EarlyBuffEffectStart.AddListener(FirstBuffEffect);
        combatManager.LateBuffEffectStart.AddListener(BuffEffect);
    }

    public void FirstBuffEffect()
    {
        float randNum = Random.value;

        if(randNum <= BaseValue)
        {
            combatManager.ProcessAllyAttack(Owner, Owner.GetComponent<SkillActor>().skillInstances.First());
            combatManager.ProcessAllyAttack(Owner, Owner.GetComponent<SkillActor>().skillInstances.First());
        }
    }

    public override void Deactivate(BuffReciever reciever)
    {
        //Remove Listeners
        combatManager.EarlyBuffEffectStart.RemoveListener(FirstBuffEffect);
        combatManager.LateBuffEffectStart.RemoveListener(BuffEffect);

        base.Deactivate(reciever);
    }
}