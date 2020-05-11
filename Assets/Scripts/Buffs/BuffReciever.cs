using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffReciever : MonoBehaviour
{
    public List<Buff> buffs;

    public List<Buff> PresetBuffs;

    private void Start()
    {
        foreach (var buff in PresetBuffs)
        {
            buff.ActivateRightAway = false;

            AddBuff(buff);
        }
    }

    public Buff AddBuff(Buff buff)
    {
        Buff temp = Instantiate(buff, this.transform);
        buffs.Add(temp);

        temp.Owner = GetComponent<Character>();
        if(temp.ActivateRightAway == true) temp.Activate(this);
        return temp;
    }

    public void RemoveBuff(Buff buff)
    {
        Buff temp = FindBuff(buff);
        if (temp != null)
        {
            buffs.Remove(temp);
            Destroy(temp.gameObject);
        }
    }

    public Buff FindBuff(Buff buff)
    {
        return buffs.FirstOrDefault(b => b.buffName == buff.buffName);
    }
}
