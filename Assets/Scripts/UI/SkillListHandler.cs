using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillListHandler : MonoBehaviour
{
    public GameObject CharacterCSPrefab;
    public List<TileElement> TileImageList;
    public List<DisplaySkillInfo> SkillDisplayPrefabs;
    public DisplaySkillInfo EmptyDisplay;
    public int SkillCount = 4;

    private CombatSkills combatSkills;
    private CombatManager combatManager;
    private List<GameObject> currentSkillDisplayedList = new List<GameObject>();

    private void Awake()
    {
        SingletonManager.Register<SkillListHandler>(this, SingletonType.Occasional);
    }

    private void Start()
    {
        combatSkills = SingletonManager.Get<CombatSkills>();
        combatManager = SingletonManager.Get<CombatManager>();

        //AddListener
        combatManager.OnInitialTeamSynced.AddListener(UpdateSkillDisplay);
        combatManager.OnCharacterDead.AddListener(delegate { UpdateSkillDisplay(); });
        combatManager.OnTutorialRoundEnd.AddListener(UpdateSkillDisplay);
        combatManager.OnCurrentRoundEnd.AddListener(UpdateSkillDisplay);
    }

    void UpdateSkillDisplay()
    {
        if (currentSkillDisplayedList.Count != 0) ClearSkillDisplayed();

        foreach (var ally in combatManager.AllyTeam)
        {
            //Requirements
            CharacterUI characterUI = ally.GetComponent<CharacterUI>();
            List<Skill> skills = ally.GetComponent<SkillActor>().skills;

            GameObject characterCS = Instantiate(CharacterCSPrefab, transform);
            CharacterCSUI cs = characterCS.GetComponent<CharacterCSUI>();

            //Spawn its Icon to CheatSheet
            Instantiate(characterUI.IconPrefab, cs.IconTransform);

            int spawnCount = 0;

            foreach (var skill in skills)
            {
                DisplaySkillInfo skillInfo = SkillDisplayPrefabs.Find(s => s._Skill == skill);

                if (skillInfo != null)
                {
                    GameObject skillObj = Instantiate(skillInfo.gameObject, cs.SkillTransform);
                    spawnCount++;
                }
            }

            while (spawnCount < SkillCount)
            {
                GameObject emptyDisplayObj = Instantiate(EmptyDisplay.gameObject, cs.SkillTransform);
                spawnCount++;
            }

            currentSkillDisplayedList.Add(characterCS);
        }
    }

    void ClearSkillDisplayed()
    {
        int count = currentSkillDisplayedList.Count;

        for (int i = 0; i < count; i++)
        {
            Destroy(currentSkillDisplayedList[i]);
        }

        currentSkillDisplayedList.Clear();
    }
}
