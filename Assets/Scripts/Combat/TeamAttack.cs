using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeamAttack : MonoBehaviour
{
    public int CurrentTeamAttackAmount;
    public int MinTeamAttackAmount = 0;
    public int MaxTeamAttackAmount = 10;

    [Header("Events")]
    public UnityEvent OnTeamAttackFull;
    public UnityEvent OnTeamAttackValueAdded;
    public UnityEvent OnTeamAttackDone;

    private CombatManager combatManager;
    private CombatSkills combatSkills;
    private BoardHandler boardHandler;

    private void Awake()
    {
        SingletonManager.Register<TeamAttack>(this, SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        combatManager = SingletonManager.Get<CombatManager>();
        combatSkills = SingletonManager.Get<CombatSkills>();
        boardHandler = SingletonManager.Get<BoardHandler>();

        //Add Listeners
        boardHandler.OnEndBoardInteraction.AddListener(AddToTeamAttackAmount);
        OnTeamAttackFull.AddListener(DoTeamAttack);
    }

    public float NormalizeTeamAttackAmount()
    {
        return (float)CurrentTeamAttackAmount / (float)MaxTeamAttackAmount;
    }

    public int GetToBeAddAmount()
    {
        List<AttributeType> usedType = new List<AttributeType>();

        foreach (var skill in combatSkills.CastableSkills)
        {
            foreach (var type in skill.SkillRequirements)
            {
                usedType.Add(type);
            }
        }

        return Mathf.Abs((boardHandler.Tiles.Count - usedType.Count));
    }

    public void AddToTeamAttackAmount()
    {
        CurrentTeamAttackAmount += GetToBeAddAmount();
        CurrentTeamAttackAmount = Mathf.Clamp(CurrentTeamAttackAmount, MinTeamAttackAmount, MaxTeamAttackAmount);

        OnTeamAttackValueAdded.Invoke();

        if (CurrentTeamAttackAmount == MaxTeamAttackAmount) OnTeamAttackFull.Invoke();
    }

    void DoTeamAttack()
    {
        foreach (var ally in combatManager.AllyTeam)
        {
            if (ally == null || ally.GetComponent<SkillActor>().skills.Count == 0) return;

            Debug.Log("Team Attacker: " + ally.CharacterName);
            combatManager.ProcessAllyAttack(ally, ally.GetComponent<SkillActor>().skillInstances.First());
        }
        
        combatManager.OnCurrentRoundEnd.AddListener(ResetTeamAttackAmount);
    }

    void ResetTeamAttackAmount()
    {
        OnTeamAttackDone.Invoke();
        CurrentTeamAttackAmount = MinTeamAttackAmount;
        combatManager.OnCurrentRoundEnd.RemoveListener(ResetTeamAttackAmount);
    }
}
