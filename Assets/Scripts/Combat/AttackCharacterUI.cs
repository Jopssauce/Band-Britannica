using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackCharacterUI : MonoBehaviour
{
    private Text tMText;
    private CombatManager combatManager;

    // Start is called before the first frame update
    void Start()
    {
        tMText = GetComponent<Text>();
        combatManager = SingletonManager.Get<CombatManager>();

        combatManager.OnCharacterStartAttack.AddListener(DisplayAttackerInfo);
        combatManager.OnCharacterEndAttack.AddListener(RemoveAttackerInfo);
    }

    void DisplayAttackerInfo(Character character, Skill skill)
    {
        tMText.text = character.CharacterName + " - " + skill.skillName;
    }

    void RemoveAttackerInfo()
    {
        tMText.text = "";
    }
}
