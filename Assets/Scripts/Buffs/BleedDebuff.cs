using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedDebuff : FunctionBuff
{
    public override void Activate(BuffReciever reciever)
    {
        base.Activate(reciever);

        //Add Listener
        combatManager.LateBuffEffectStart.AddListener(BuffEffect);
    }

    public override void BuffEffect()
    {
        if(audioManager) audioManager.PlaySFX("Bleed");

        //Effect
        Owner.DeductHealth(Amount);

        base.BuffEffect();
    }

    public override void Deactivate(BuffReciever reciever)
    {
        //Remove Listner
        combatManager.LateBuffEffectStart.RemoveListener(BuffEffect);

        base.Deactivate(reciever);
    }
}
