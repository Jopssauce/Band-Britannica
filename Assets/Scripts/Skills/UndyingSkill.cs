using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndyingSkill : Skill
{
    public Buff BuffUndying;

    protected override void Start()
    {
        base.Start();

        transform.parent.GetComponent<BuffReciever>().AddBuff(BuffUndying);
    }
}
