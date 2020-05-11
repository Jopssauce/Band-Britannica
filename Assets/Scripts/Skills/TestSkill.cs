using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : MonoBehaviour
{
    public GameObject actor;
    public Character[] allies;
    public Character enemy;
    public Character[] enemies;

    public int skillIndex;

    public void TrySkill()
    {
        actor.GetComponent<SkillActor>().UseSkill(allies, enemy, enemies, actor.GetComponent<SkillActor>().skillInstances[skillIndex]);
    }
}