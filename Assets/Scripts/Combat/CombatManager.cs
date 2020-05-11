using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

[System.Serializable]
public class CombatDeadEvent : UnityEvent<Character, List<Character>>{ }

[System.Serializable]
public class CombatStateEvent : UnityEvent<CombatState> { }

[System.Serializable]
public class CombatAttackEvent : UnityEvent<Character, Skill> { }

public enum CombatState
{
    DialogueMode,
    BattleStart,
    Match3Turn,
    MatchClearing,
    AllyTurn,
    EnemyTurn,
    BattleEnd
}

public class CombatManager : MonoBehaviour
{
    const int DEFAULT_ATTACK_INDEX = 0;
    const int DEFAULT_TURN_DEDUCT = 1;
    const int ENEMY_MANA_ADD = 1;
    const int MULTIPLIER_PER_MATCH = 2;
    const int MAXIMUM_SINGLE_TARGET_MATCH = 4;

    public enum Team
    {
        None,
        Ally,
        Enemy
    }

    [Header("Tutorial")]
    public bool IsTutorial = false;

    public CombatState CurCombatState = CombatState.BattleStart;
    public Team CurTeamWin = Team.None;
    public Team AttackSequence = Team.Ally;

    public int CurRound = 0;

    [Header("Teams")]
    public List<Character> AllyTeam;
    public List<Character> EnemyTeam;

    [Header("Selected Character")]
    public Character SelectedEnemy;

    [Header("Attack Order")]
    public Coroutine ActionCoroutine;
    public List<Character> AttackList = new List<Character>();
    public List<Skill> AllySkillToUse = new List<Skill>();
    public int CurAttackTurn = 0;
    public bool IsStartAttack = false;
    public bool IgnoreTurn = false;

    public float BattleBeginTimer = 0.2f;
    [HideInInspector]
    public float MatchDelayTimer = 1.0f;

    [Header("Events")]
    public CombatDeadEvent OnCharacterDead;
    public UnityEvent OnInitialTeamSynced;
    public UnityEvent OnCurrentRoundStart;
    public UnityEvent OnCurrentRoundEnd;
    public UnityEvent OnTutorialRoundEnd;
    public UnityEvent OnCombatEnd;
    public UnityEvent EarlyBuffEffectStart;
    public UnityEvent LateBuffEffectStart;

    public CombatStateEvent OnCombatStateChanged;
    public CombatAttackEvent OnCharacterStartAttack;
    public UnityEvent OnCharacterEndAttack;
    
    private BoardHandler boardHandler;
    private FieldManager fieldManager;
    private CombatSkills combatSkills;

    //for other function use
    public Skill SkillToUse;
    private List<AttributeType> matchedTypes;
    private Character actorCharacter;
    private List<Character> actorList;
    private Character targetCharacter;
    private List<Character> targetList;

    private int allyAttackTurns;
    private int enemyAttackTurns;
    private int matchCount;

    public StageInfo stageToUnlock;
    public StageInfo currentStage;

    private void Awake()
    {
        SingletonManager.Register<CombatManager>(this, SingletonType.Occasional);
    }

    private void Start()
    {
        boardHandler = SingletonManager.Get<BoardHandler>();
        fieldManager = SingletonManager.Get<FieldManager>();
        combatSkills = SingletonManager.Get<CombatSkills>();

        //Add Listener to Event
        boardHandler.OnEndBoardInteraction.AddListener(StartTurnBasedCombat);
        fieldManager.OnFieldInitialized.AddListener(GetTeams);
    }

    void GetTeams()
    {
        AllyTeam = fieldManager.SpawnedAllies;
        EnemyTeam = fieldManager.SpawnedEnemies;

        //Event for Team Synced
        OnInitialTeamSynced.Invoke();

        if (IsTutorial == false) Invoke("MatchingBegin", BattleBeginTimer);
    }

    /// <summary>
    /// Call this for Starting the Turn Based Combat
    /// </summary>
    void MatchingBegin()
    {
        //Change Combat State to Match3 Turn
        ChangeCombatState(CombatState.Match3Turn);
    }

    public void StartTurnBasedCombat()
    {
        Debug.Log("-------------- Start Combat --------------");
        CurRound++;

        OnCurrentRoundStart.Invoke();

        allyAttackTurns = 0;
        enemyAttackTurns = 0;

        matchedTypes = ConvertTileToTypes(boardHandler.Tiles);
        matchCount = matchedTypes.Count;

        if (AttackSequence == Team.Ally)
        {
            SetAllyToAttack();
            SetEnemyToAttack();
        }
        else
        {
            SetEnemyToAttack();
            SetAllyToAttack();
        }

        BeginAttackList();
    }

    void SetAllyToAttack()
    {
        foreach (var skill in combatSkills.CastableSkills)
        {
            bool condition = skill.CanCastThisSkill(matchedTypes);

            if (condition == true)
            {
                ProcessAllyAttack(GetCharacterWithSkill(skill, AllyTeam), skill);
            }
        }

        //Do Buff Effect that need to work on start
        EarlyBuffEffectStart.Invoke();
    }

    void SetEnemyToAttack()
    {
        //Enemy Turn
        foreach (var enemy in EnemyTeam)
        {
            AttackList.Add(enemy);
            enemyAttackTurns++;
        }
    }

    public void BeginAttackList()
    {
        ActionCoroutine = StartCoroutine(StartCombatFight(allyAttackTurns, enemyAttackTurns, matchCount));
    }

    public void BeginAttackList(int allyTurns, int enemyTurns)
    {
        ActionCoroutine = StartCoroutine(StartCombatFight(allyAttackTurns, enemyAttackTurns, matchCount));
    }

    public IEnumerator StartCombatFight(int allyAttackTurns, int enemyAttackTurns, int matchCount)
    {
        int firstTeamAttackCount = 0;
        if (AttackList.Count == 0) yield break;

        if (AllyTeam.Contains(AttackList.First()))
        {
            firstTeamAttackCount = allyAttackTurns;
        }
        else firstTeamAttackCount = enemyAttackTurns;

        while (true)
        {
            if (AttackList.Count > 0 && IsStartAttack == false)
            {
                Character attacker = AttackList.First();

                if (attacker == null || CheckCharacterValidToAttack(attacker) == false)
                {
                    AttackList.Remove(attacker);
                    if(AllyTeam.Contains(attacker)) AllySkillToUse.RemoveAt(0);
                    continue;
                }

                //Set StartAttack to true
                IsStartAttack = true;

                SkillActor sActor = attacker.GetComponent<SkillActor>();
                int actualTurnDeduct = DEFAULT_TURN_DEDUCT;

                actorCharacter = attacker;
                
                //Ally Attack
                if (AllyTeam.Contains(attacker) == true)
                {
                    //Debug
                    Debug.Log("-------------- Player Turn --------------");

                    //Change Combat State to Ally Turn
                    ChangeCombatState(CombatState.AllyTurn);

                    //Pass what Skill to use
                    if(AllySkillToUse.Count != 0)
                    {
                        SkillToUse = sActor.skillInstances.Find(s => s.skillName == AllySkillToUse.First().skillName);
                        AllySkillToUse.Remove(AllySkillToUse.First());
                    }

                    //If Skill is still null, set default attack as the skill
                    if (SkillToUse == null) SkillToUse = sActor.skillInstances.First();

                    actorList = AllyTeam;
                    targetCharacter = SelectedEnemy;
                    targetList = EnemyTeam;

                    AllyCombatAction(SkillToUse);
                }
                //Enemy Attack
                else
                {
                    //Debug
                    Debug.Log("-------------- Enemy Turn --------------");

                    //Change Combat State To Enemy Turn
                    ChangeCombatState(CombatState.EnemyTurn);

                    actorList = EnemyTeam;
                    targetCharacter = GetCharacterFromTeam(AllyTeam);
                    targetList = AllyTeam;

                    CombatAction(sActor.skillInstances.First(), actualTurnDeduct, ENEMY_MANA_ADD);
                }

                //After Attack assigned
                AttackList.Remove(attacker);
                firstTeamAttackCount--;
            }

            //ROUND END
            if (AttackList.Count == 0 && IsStartAttack == false)
            {
                Debug.Log("END ATTACKS");

                yield return new WaitForSeconds(MatchDelayTimer);

                LateBuffEffectStart.Invoke();

                //Check If there is a character died or not
                CheckIsDead(AllyTeam);
                CheckIsDead(EnemyTeam);

                if (CurCombatState != CombatState.DialogueMode) MatchingBegin();

                OnCurrentRoundEnd.Invoke();

                //Check Battle Result
                CheckBattleResult();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void AllyCombatAction(Skill skill)
    {
        if (targetCharacter == null)
        {
            EndAttackAction();
            return;
        }

        Debug.Log("Attacker: " + actorCharacter.CharacterName);
        Debug.Log("Receiver: " + targetCharacter.CharacterName);

        //Debug Log if something is empty
        if (actorCharacter == null) Debug.LogError("No Attacker");

        if (SkillToUse != null)
        {
            Debug.Log("SKILL TO USE: " + SkillToUse.skillName);
            OnCharacterStartAttack.Invoke(actorCharacter, SkillToUse);

            //Add Listener to OnDestinationReach
            SkillToUse.OnActivated.AddListener(StartAttackAction);

            //Use Skill
            //- Move Player to destination
            actorCharacter.GetComponent<SkillActor>().UseSkill(actorList.ToArray(), targetCharacter, targetList.ToArray(), SkillToUse);
        }
        else Debug.Log("Skill is null");
    }

    void CombatAction(Skill skill, int deductTurn, int manaAdd)
    {
        bool doAttack = true;

        if (boardHandler.IsClearBoard == false)
        {
            //Deduct Turn and Add Mana
            actorCharacter.CurrentTurns -= deductTurn;
            actorCharacter.Mana += manaAdd;

            //Clamping (0, CurrentTurn) (0, MaxMana)
            actorCharacter.CurrentTurns = Mathf.Clamp(actorCharacter.CurrentTurns, 0, actorCharacter.CurrentTurns);
            actorCharacter.Mana = Mathf.Clamp(actorCharacter.Mana, 0, actorCharacter.maxMana);
        }

        //Debug
        Debug.Log(actorCharacter.CharacterName + " Current Turn: " + actorCharacter.CurrentTurns + " (" + deductTurn + ")");

        if (actorCharacter.CurrentTurns == 0 || IgnoreTurn == true)
        {
            Debug.Log("Attacker: " + actorCharacter.CharacterName);
            if(targetCharacter != null) Debug.Log("Receiver: " + targetCharacter.CharacterName);

            //Debug Log if something is empty
            if (actorCharacter == null) Debug.LogError("No Attacker");
            if (targetCharacter == null) doAttack = false;

            SkillToUse = actorCharacter.GetComponent<SkillActor>().FindSkill(skill);
            SkillToUse.OnActivated.RemoveListener(StartAttackAction);

            if (actorCharacter.Mana >= SkillToUse.cost && doAttack == true)
            {
                if (SkillToUse != null)
                {
                    Debug.Log("SKILL TO USE: " + SkillToUse.skillName);

                    //Add Listener to OnDestinationReach
                    SkillToUse.OnActivated.AddListener(StartAttackAction);

                    //Use Skill
                    //- Move Player to destination
                    actorCharacter.GetComponent<SkillActor>().UseSkill(actorList.ToArray(), targetCharacter, targetList.ToArray(), SkillToUse);

                    OnCharacterStartAttack.Invoke(actorCharacter, SkillToUse);
                }   
            }
            else
            {
                Debug.Log(actorCharacter.CharacterName + " does not have enough Mana.");
                doAttack = false;
            }
        }
        else
        {
            Debug.Log(actorCharacter.CharacterName + "'s CD in Cooldown.");
            doAttack = false;
        }

        if(doAttack != true)
        {
            actorCharacter = null;
            EndAttackAction();
        }
    }

    /// <summary>
    /// To be Called after Actual Damage Done
    /// </summary>
    void StartAttackAction()
    {
        Debug.Log("Called StartAttackAction");

        CheckIsDead(targetList);

        //Event Listeners
        CustomCharacterController characterController = actorCharacter.GetComponent<CustomCharacterController>();
        
        characterController.OnDestinationReach.AddListener(EndAttackAction);
    }

    void OrganizeDeadCharacter(Character character, List<Character> team)
    {
        List<Character> list = team;
        int index = list.IndexOf(character);

        //Remove target Character from Combat Manager List and Remove the Attack Position Transform
        list.RemoveAt(index);

        //Do Destroy/Unuse Logic
        character.Destroy();
        OnCharacterDead.Invoke(character, list);
    }

    /// <summary>
    /// To be Called after Whole process for action is Done
    /// </summary>
    void EndAttackAction()
    {
        Debug.Log("Called EndAttackAction");
        OnCharacterEndAttack.Invoke();

        if (actorCharacter != null)
        {
            //Reset Turns
            actorCharacter.CurrentTurns = actorCharacter.MaxTurns;

            CustomCharacterController characterController = actorCharacter.GetComponent<CustomCharacterController>();
            characterController.OnDestinationReach.RemoveListener(EndAttackAction);
        }

        //nulltify all cache items
        NulltifyAllRefVariable();

        //Give permission for following Attacker
        IsStartAttack = false;
    }

    void NulltifyAllRefVariable()
    {
        actorCharacter = null;
        actorList = null;
        targetCharacter = null;
        targetList = null;
        SkillToUse = null;
        IgnoreTurn = false;
    }

    void CheckBattleResult()
    {
        if (AllyTeam.Count == 0)
        {
            CurTeamWin = Team.Enemy;
        }
        else if (EnemyTeam.Count == 0)
        {
            CurTeamWin = Team.Ally;
            if(stageToUnlock != null) stageToUnlock.unlocked = true;
            if(currentStage != null) currentStage.completed = true;
        }
        else return;

        //Battle Ended
        ChangeCombatState(CombatState.BattleEnd);
        OnCombatEnd.Invoke();
    }

    /// <summary>
    /// Default is randomize
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    Character GetCharacterFromTeam(List<Character> team)
    {
        if (team.Count == 0) return null;

        //Check if Character has TauntBuff
        foreach (var character in team)
        {
            if (character != null)
            {
                List<Buff> buffs = character.GetComponent<BuffReciever>().buffs;

                foreach (var buff in buffs)
                {
                    if (buff is TauntBuff)
                    {
                        return character;
                    }
                }
            }
        }
        
        //Or Else Do Random
        int randNum = Random.Range(0, team.Count);

        return team[randNum];
    }
    
    Character GetCharacterWithSkill(Skill skill, List<Character> team)
    {
        foreach (var character in team)
        {
            bool condition = character.GetComponent<SkillActor>().skills.Contains(skill);

            if (condition == true)
            {
                return character;
            }
        }

        return null;
    }

    public int GetCorrespondTurnDeduct(int count)
    {
        int deductTurn = 0;

        while (count > 0)
        {
            deductTurn++;
            count -= MULTIPLIER_PER_MATCH;
        }

        return deductTurn;
    }

    void CheckIsDead(List<Character> list)
    {
        List<Character> deadList = new List<Character>();

        foreach (var character in list)
        {
            if (character.IsAlive == false)
            {
                deadList.Add(character);
            }
        }

        for (int i = 0; i < deadList.Count; i++)
        {
            OrganizeDeadCharacter(deadList[i], list);
        }
    }

    void RemoveNullCharacter(ref List<Character> list)
    {
        int count = list.Count;
        List<Character> newList = new List<Character>();

        for (int i = 0; i < count; i++)
        {
            if (list[i] != null) newList.Add(list[i]);
        }

        list = newList;
    }

    bool CheckCharacterValidToAttack(Character character)
    {
        BuffReciever buffReceiver = character.GetComponent<BuffReciever>();

        foreach (var buff in buffReceiver.buffs)
        {
            if (buff.type == BuffType.Special) return false;
        }

        return true;
    }
    
    public void ChangeCombatState(CombatState state)
    {
        CurCombatState = state;

        OnCombatStateChanged.Invoke(CurCombatState);
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

    public void ProcessAllyAttack(Character character, Skill skillToUse)
    {
        AttackList.Add(character);
        AllySkillToUse.Add(skillToUse);
        allyAttackTurns++;
    }
}