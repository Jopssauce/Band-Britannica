using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class SkillActor : MonoBehaviour
{
    public List<Skill> skills;
    public List<Skill> skillInstances;

    public Skill lastSkillUsed;
    void Start()
    {
        InstantiateSkills();
    }

    public void InstantiateSkills()
    {
        foreach (var skill in skills)
        {
            //if (FindSkill(skill) != null) continue;
            {
                Skill s = Instantiate(skill, this.transform);
                s.actor = this;
                skill.actor = this;
                skillInstances.Add(s);
            }
        }
    }

    public Skill UseSkill(Character[] allies, Character enemy, Character[] enemies, Skill skill)
    {
        lastSkillUsed = FindSkill(skill);
        lastSkillUsed.Activate(this, allies, enemy, enemies);
        return lastSkillUsed;
    }

    public Skill FindSkill(Skill skill)
    {
        return skillInstances.FirstOrDefault(s => s.skillName == skill.skillName);
    }

    public Skill GetSkillByName(string skillName)
    {
        return skillInstances.FirstOrDefault(s => s.skillName == skillName);
    }

    public void AddSkill(Skill skill)
    {
        Skill s = Instantiate(skill, this.transform);
        s.actor = this;
        skill.actor = this;
        skillInstances.Add(s);
        skills.Add(skill);
    }

    public void RemoveSkill(Skill skill)
    {
        skillInstances.FirstOrDefault(s => s.skillName == skill.skillName);
    }

    public void RemoveSkillByString(string skill)
    {
        skillInstances.First(s => s.skillName == skill);
    }
}
