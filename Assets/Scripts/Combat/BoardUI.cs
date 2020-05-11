using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    [Header("Constant Variables")]
    public string EmptySkillName = "Locked";

    [Header("Selected Characters")]
    public Text AllyCharacterName;
    public Transform AllySkillPanel;
    public Text EnemyCharacterName;
    public Transform EnemySkillPanel;

    private CombatManager cManager;
    private CharacterSelection cSelection;
    
    private void Awake()
    {
        SingletonManager.Register<BoardUI>(this,SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        cManager = SingletonManager.Get<CombatManager>();
        cSelection = SingletonManager.Get<CharacterSelection>();

        cSelection.OnSelectCharacter.AddListener(UpdateStatsByCharacter);
    }

    public void UpdateStatsByDefault()
    {
        UpdateSpecificTeam(cManager.SelectedEnemy, EnemyCharacterName, EnemySkillPanel);
    }

    public void UpdateStatsByCharacter(Character character)
    {
        /*
        if (character == cManager.SelectedAlly)
        {
            UpdateSpecificTeam(cManager.SelectedAlly, AllyCharacterName, AllySkillPanel);
        }
        else UpdateSpecificTeam(cManager.SelectedEnemy, EnemyCharacterName, EnemySkillPanel);
        */
    }

    void UpdateSpecificTeam(Character selectedChar, Text displayNameText, Transform skillPanel)
    {
        //Skip default Skill
        int skillIndex = 1;

        if (selectedChar != null)
        {
            if(displayNameText != null) displayNameText.text = selectedChar.CharacterName;

            SkillActor skillActor = selectedChar.GetComponent<SkillActor>();

            if (skillPanel != null)
            {
                foreach (Transform item in skillPanel)
                {
                    Text txt = item.GetComponent<Text>();
                    Button txtBtn = item.GetComponent<Button>();

                    if (txtBtn == null) return;

                    //Reset
                    SetButtonInteractable(txtBtn, false);
                    txtBtn.onClick.RemoveAllListeners();

                    if (skillIndex < skillActor.skills.Count)
                    {
                        txt.text = skillActor.skills[skillIndex].skillName;

                        if (selectedChar.Mana >= skillActor.skills[skillIndex].cost)
                        {
                            SetButtonInteractable(txtBtn, true);
                            //txtBtn.onClick.AddListener(() => cSkillUIHandler.UseUniqueSkill(txt.text));
                        }

                        skillIndex++;
                    }
                    else
                    {
                        txt.text = EmptySkillName;
                        SetButtonInteractable(txtBtn, false);
                        //txtBtn.onClick.RemoveListener(() => cSkillUIHandler.UseUniqueSkill(txt.text));
                    }
                }
            }
        }
    }

    void SetButtonInteractable(Button button, bool condition)
    {
        button.interactable = condition;
    }
}
