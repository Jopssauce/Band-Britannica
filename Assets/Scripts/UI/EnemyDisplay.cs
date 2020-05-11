using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    public Text enemyName;
    public Text enemyHp;
    public Text[] enemySkills;

    protected CombatManager combat;
    protected CharacterSelection characterSelect;

    private Character selCharacter;

    void Start()
    {
        combat = SingletonManager.Get<CombatManager>();
        characterSelect = SingletonManager.Get<CharacterSelection>();

        //Add Listeners
        characterSelect.OnSelectCharacter.AddListener(UpdateDisplay);
    }

    void UpdateDisplay(Character character)
    {
        if(combat.SelectedEnemy == null) return;

        if (selCharacter == null || selCharacter != combat.SelectedEnemy)
        {
            selCharacter = combat.SelectedEnemy;
            selCharacter.OnDeductHealth.AddListener(UpdateDisplay);
        }
        else selCharacter.OnDeductHealth.RemoveListener(UpdateDisplay);

        UpdateTexts();
    }

    void UpdateDisplay(float amount)
    {
        if (combat.SelectedEnemy == null) return;

        UpdateTexts();
    }

    void UpdateTexts()
    {
        enemyName.text = combat.SelectedEnemy.CharacterName.ToString();
        enemyHp.text = ((float)Math.Round(combat.SelectedEnemy.Health, 1)).ToString();

        for (int i = 0; i < enemySkills.Length; i++)
        {
            if (i < combat.SelectedEnemy.GetComponent<SkillActor>().skills.Count)
            {
                enemySkills[i].text = combat.SelectedEnemy.GetComponent<SkillActor>().skills[i].skillName;
                enemySkills[i].transform.parent.GetComponent<DisplaySkillInfo>()._Skill = combat.SelectedEnemy.GetComponent<SkillActor>().skills[i];
            }
            else
            {
                enemySkills[i].text = "";
            }
        }
    }
}
