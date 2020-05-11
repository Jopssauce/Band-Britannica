using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntBuff : Buff
{
    public override void Activate(BuffReciever reciever)
    {
        base.Activate(reciever);

        //Add Listener
        combatManager.LateBuffEffectStart.AddListener(BuffEffect);
    }

    public override void Deactivate(BuffReciever reciever)
    {
        //Remove Listener
        combatManager.LateBuffEffectStart.RemoveListener(BuffEffect);

        base.Deactivate(reciever);
    }
}
