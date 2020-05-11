using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatSkills : MonoBehaviour
{
    public List<Skill> AllSkills = new List<Skill>();
    public List<Skill> CastableSkills = new List<Skill>();
    public List<Skill> DisplayAllSkills = new List<Skill>();
    public List<Skill> DisplaySkills = new List<Skill>();

    public UnityEvent OnCastableSkillsUpdated;
    public UnityEvent OnAllSkillUpdated;

    private List<AttributeType> matchedTypes;

    private FieldManager fieldManager;
    private CombatManager combatManager;
    private BoardHandler boardHandler;

    private void Awake()
    {
        SingletonManager.Register<CombatSkills>(this, SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        fieldManager = SingletonManager.Get<FieldManager>();
        combatManager = SingletonManager.Get<CombatManager>();
        boardHandler = SingletonManager.Get<BoardHandler>();

        //Add Listener
        fieldManager.OnFieldInitialized.AddListener(GetSkills);
        combatManager.OnCurrentRoundEnd.AddListener(UpdateAvailableSkills);
        combatManager.OnCurrentRoundEnd.AddListener(ClearCastableSkills);
        combatManager.OnTutorialRoundEnd.AddListener(UpdateAvailableSkills);

        boardHandler.OnAddTile.AddListener(CheckCastableSkills);
        boardHandler.OnRemoveTile.AddListener(CheckCastableSkills);
        boardHandler.OnDeselectTiles.AddListener(ClearCastableSkills);
    }

    void GetSkills()
    {
        GetAllSkillsFrom(fieldManager.SpawnedAllies);
    }

    void UpdateAvailableSkills()
    {
        GetAllSkillsFrom(combatManager.AllyTeam);
    }

    void GetAllSkillsFrom(List<Character> characters)
    {
        AllSkills.Clear();
        DisplayAllSkills.Clear();

        foreach (var character in characters)
        {
            foreach (var skill in character.GetComponent<SkillActor>().skills)
            {
                AllSkills.Add(skill);
            }
        }

        foreach (var character in characters)
        {
            foreach (var skill in character.GetComponent<SkillActor>().skillInstances)
            {
                DisplayAllSkills.Add(skill);
            }
        }

        OnAllSkillUpdated.Invoke();
    }

    void CheckCastableSkills()
    {
        ClearCastableSkills();

        matchedTypes = ConvertTileToTypes(boardHandler.Tiles);

        foreach (var skill in AllSkills)
        {
            bool cond = skill.CanCastThisSkill(matchedTypes);

            if(cond == true)
            {
                CastableSkills.Add(skill);
            }
        }

        foreach (var skill in DisplayAllSkills)
        {
            bool cond = skill.CanCastThisSkill(matchedTypes);

            if (cond == true)
            {
                DisplaySkills.Add(skill);
            }
        }

        OnCastableSkillsUpdated.Invoke();
    }

    void ClearCastableSkills()
    {
        CastableSkills.Clear();
        DisplaySkills.Clear();
    }

    List<AttributeType> ConvertTileToTypes(List<Tile> tiles)
    {
        List<AttributeType> types = new List<AttributeType>();

        foreach (Tile tile in tiles)
        {
            types.Add(tile.Type);
        }

        return types;
    }
}
