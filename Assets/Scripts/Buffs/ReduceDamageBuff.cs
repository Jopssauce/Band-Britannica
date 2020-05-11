using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceDamageBuff : FunctionBuff
{
    private float originalReductDamageVal = 0;

    public override void Activate(BuffReciever reciever)
    {
        base.Activate(reciever);

        //Do the Effect
        originalReductDamageVal = Owner.Shield;
        Owner.Shield = BaseValue;

        //Add Listener
        combatManager.LateBuffEffectStart.AddListener(BuffEffect);
    }

    public override void Deactivate(BuffReciever reciever)
    {
        Owner.Shield = originalReductDamageVal;

        //Remove Listener
        combatManager.LateBuffEffectStart.RemoveListener(BuffEffect);

        base.Deactivate(reciever);
    }
}
