using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndyingBuff : Buff
{
    public float MinPercent = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        //Add Listener
        Owner.OnDeductHealth.AddListener(CheckConditionMeet);
    }

    public override void Activate(BuffReciever reciever)
    {
        base.Activate(reciever);

        //Actions
        Owner.IsUndead = true;

        //Add Listener
        combatManager.OnCurrentRoundEnd.AddListener(BuffEffect);
    }

    public override void Deactivate(BuffReciever reciever)
    {
        //Reverts
        Owner.IsUndead = false;

        //Remove Listener
        combatManager.OnCurrentRoundEnd.RemoveListener(BuffEffect);

        base.Deactivate(reciever);
    }

    void CheckConditionMeet(float amount)
    {
        if (Owner.Health <= Owner.maxHealth * MinPercent)
        {
            rcvr = Owner.GetComponent<BuffReciever>();
            Activate(rcvr);

            //Remove Listener
            Owner.OnDeductHealth.RemoveListener(CheckConditionMeet);
        }
    }
}