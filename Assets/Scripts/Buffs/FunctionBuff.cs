using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionBuff : Buff
{
    public float BaseValue = 10;
    public float DerivedValue = 0;

    public float Amount
    {
        get
        {
            return BaseValue + (BaseValue * DerivedValue);
        }
    }
}
