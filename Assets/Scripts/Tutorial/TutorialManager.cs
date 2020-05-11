using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NextDialogueEvent : UnityEvent<int> { }

[System.Serializable]
public struct TileRequirement
{
    public int InIndex;
    public List<DoubleInt> TileCoords;
}

[System.Serializable]
public struct TileType
{
    public AttributeType type;
    public int Minimum;
    public int Maximum;
}

[System.Serializable]
public struct TileTypeRequirement
{
    public int InIndex;
    public bool IsLevelThree;
    public List<TileType> tileTypes;

    public int MinimumTileCount
    {
        get
        {
            int num = 0;

            foreach (var item in tileTypes)
            {
                num += item.Minimum;
            }

            return num;
        }
    }

    public int MaximumTileCount
    {
        get
        {
            int num = 0;

            foreach (var item in tileTypes)
            {
                num += item.Maximum;
            }

            return num;
        }
    }
}

[System.Serializable]
public struct PopUpInstruction
{
    public int Index;
    public List<PanelPopUp> PopUps;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    public List<PopUpInstruction> PopUpInstructions;

    public List<DialogueSystem> DialogueList;
    public List<TileRequirement> TileRequirements;
    public List<TileTypeRequirement> TileTypeRequirements;

    [Header("Character Skills")]
    public List<GameObject> TilePrefabForSkill;
    public List<Skill> WarriorSkills;
    public List<Skill> MageSkills;
    public List<Skill> RogueSkills;

    [Header("Events")]
    public NextDialogueEvent OnNextDialogue;
    public UnityEvent OnTutorialEnd;
    public UnityEvent OnPopUpInstruction;
    public NextDialogueEvent OnPopUpActive;
    public NextDialogueEvent OnPopUp;
    
    public int Index = 0;

    private List<GameObject> originalPrefabs;

    private DialogueSystem dialogue;
    private CombatManager combatManager;
    private BoardHandler boardHandler;
    private BoardGenerator boardGenerator;
    private CombatSkills combatSkills;
    private TeamAttack teamAttack;

    private void Awake()
    {
        SingletonManager.Register<TutorialManager>(this, SingletonType.Occasional);
    }

    private void Start()
    {
        combatManager = SingletonManager.Get<CombatManager>();
        boardHandler = SingletonManager.Get<BoardHandler>();
        boardGenerator = SingletonManager.Get<BoardGenerator>();
        combatSkills = SingletonManager.Get<CombatSkills>();
        teamAttack = SingletonManager.Get<TeamAttack>();

        foreach (var item in DialogueList)
        {
            item.OnNextDialogueStart.AddListener(NextLineOfDialogue);
        }

        combatManager.OnCombatEnd.AddListener(HidePersistentPopUps);
    }

    void NextDialogue()
    {
        if(dialogue != null)
        {
            dialogue.enabled = false;
            dialogue.OnDialogueEnd.RemoveAllListeners();
            DialogueList.Remove(dialogue);
            Destroy(dialogue);
            Index++;
        }

        if (DialogueList.Count != 0)
        {
            combatManager.ChangeCombatState(CombatState.DialogueMode);

            dialogue = DialogueList.First();
            dialogue.enabled = true;
        }
        else
        {
            Debug.Log("Tutorial Done");
            OnTutorialEnd.Invoke();
        }

        NextLineOfDialogue();
    }

    public void CastSpecificSkill()
    {
        /*
        if (combatSkills.CastableSkills.Contains(RequireSkill) == true)
        {
            Debug.Log("Have " + RequireSkill.skillName + " casted.");
            //Handle RemoveListener
            boardHandler.OnEndBoardInteraction.RemoveListener(CastSpecificSkill);

            //AddListener
            boardHandler.OnEndBoardInteraction.AddListener(combatManager.StartTurnBasedCombat);
            boardHandler.OnEndBoardInteraction.AddListener(teamAttack.AddToTeamAttackAmount);

            combatManager.StartTurnBasedCombat();

            //AddSkillToAllCharacters
            AddSkillToAllies();

            combatManager.OnCurrentRoundEnd.AddListener(ForthDialogueStart);
        }
        else
        {
            combatManager.OnCurrentRoundEnd.Invoke();
            combatManager.ChangeCombatState(CombatState.Match3Turn);
        }
        */
    }

    public void ForthDialogueStart()
    {
        //Handle RemoveListener
        combatManager.OnCurrentRoundEnd.RemoveListener(ForthDialogueStart);

        //Update Dialogue
        NextDialogue();

        //AddListener
        DialogueList.First().OnDialogueEnd.AddListener(ForthDialogueEnd);
    }

    public void ForthDialogueEnd()
    {
        //Update Dialogue
        NextDialogue();

        OnEndTutorial();
    }

    public void SkipTutorial()
    {
        ShowPersistentPopUps();

        //AddSkillToAllCharacters
        AddSkillToAllies();

        combatManager.OnCurrentRoundEnd.Invoke();

        OnEndTutorial();
    }

    void AddSkillToAllies()
    {
        SkillActor warriorSActor = combatManager.AllyTeam.Find(c => c.CharacterName == "Warrior").GetComponent<SkillActor>();
        SkillActor mageSActor = combatManager.AllyTeam.Find(c => c.CharacterName == "Mage").GetComponent<SkillActor>();
        SkillActor rogueSActor = combatManager.AllyTeam.Find(c => c.CharacterName == "Rogue").GetComponent<SkillActor>();

        AddSkillToEachCharacter(warriorSActor, WarriorSkills);
        AddSkillToEachCharacter(mageSActor, MageSkills);
        AddSkillToEachCharacter(rogueSActor, RogueSkills);
    }

    void AddSkillToEachCharacter(SkillActor skillActor, List<Skill> skills)
    {
        foreach (var skill in skills)
        {
            if (skillActor.skills.Contains(skill) == false)
            {
                skillActor.AddSkill(skill);
            }
        }
    }

    void OnEndTutorial()
    {
        //Close the Tutorial
        combatManager.IsTutorial = false;

        //Start Match Tiles
        combatManager.ChangeCombatState(CombatState.Match3Turn);
    }

    void NextLineOfDialogue()
    {
        /*
        OverallDialogueIndex += 1;

        OnNextDialogue.Invoke(OverallDialogueIndex);
        */
    }

    void ShowTutorialPanel()
    {
        if (Index >= PopUpInstructions.Count) return;

        foreach (var item in PopUpInstructions[Index].PopUps)
        {
            item.PopUp();
        }

        OnPopUpInstruction.Invoke();
        OnPopUp.Invoke(Index);
    }

    void HideTutorialPanel()
    {
        if (Index >= PopUpInstructions.Count) return;

        foreach (var item in PopUpInstructions[Index].PopUps)
        {
            if(item.IsPersistent == false)
            {
                item.PopDown();
            }
        }
        Index++;
    }

    void ShowHidePopUps()
    {
        OnPopUpActive.Invoke(Index);
    }

    void ShowPersistentPopUps()
    {
        for (int i = 0; i < PopUpInstructions.Count; i++)
        {
            foreach (var item in PopUpInstructions[i].PopUps)
            {
                if (item.IsPersistent)
                {
                    item.PopUp();
                }
            }
        }
    }

    void HidePersistentPopUps()
    {
        for (int i = 0; i < PopUpInstructions.Count; i++)
        {
            foreach (var item in PopUpInstructions[i].PopUps)
            {
                if (item.IsPersistent)
                {
                    item.PopDown();
                }
            }
        }
    }
    #region FunctionCalledByButtons
    public void ContinueTutorial()
    {
        //AddSkillToMage
        SkillActor sActor = combatManager.AllyTeam.Find(c => c.CharacterName == "Mage").GetComponent<SkillActor>();
        sActor.AddSkill(MageSkills.First());

        combatManager.OnTutorialRoundEnd.Invoke();

        OnPopUpInstruction.AddListener(First_Dialogue_Finish);

        ShowTutorialPanel();

        //Dialogues Setup
        if (DialogueList.Count > 1)
        {
            DialogueList.First().OnDialogueEnd.AddListener(First_Dialogue_Finish);
            NextDialogue();
        }
    }
    #endregion

    #region NewTutorialFunctions
    void First_Dialogue_Finish()
    {
        OnPopUpInstruction.RemoveListener(First_Dialogue_Finish);
        
        combatManager.ChangeCombatState(CombatState.Match3Turn);

        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.RemoveListener(teamAttack.AddToTeamAttackAmount);
        boardHandler.OnTileMatchedRelease.AddListener(First_Dialogue_CheckCondition);

        ShowHidePopUps();
    }

    void First_Dialogue_CheckCondition()
    {
        boardHandler.OnTileMatchedRelease.RemoveListener(First_Dialogue_CheckCondition);

        if (ReachTutorialRequirement() == true)
        {
            First_Dialogue_ConditionMet();
        }
        else
        {
            First_Dialogue_Reset();
        }
    }

    void First_Dialogue_Reset()
    {
        //Handle RemoveListener
        boardHandler.OnEndBoardInteraction.RemoveListener(First_Dialogue_Finish);
        boardHandler.OnEndBoardInteraction.AddListener(First_Dialogue_Finish);

        //ClearBoard
        boardHandler.IsClearBoard = true;
        boardHandler.SelectAllTiles();

        combatManager.OnTutorialRoundEnd.Invoke();
    
        ShowHidePopUps();
    }

    void First_Dialogue_ConditionMet()
    {
        //Handle RemoveListener
        boardHandler.OnEndBoardInteraction.RemoveListener(First_Dialogue_Finish);
        boardHandler.OnEndBoardInteraction.AddListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.AddListener(teamAttack.AddToTeamAttackAmount);

        combatManager.OnCurrentRoundEnd.AddListener(Second_Dialogue_Start);

        ShowHidePopUps();
        HideTutorialPanel();
    }

    void Second_Dialogue_Start()
    {
        combatManager.OnCurrentRoundEnd.RemoveListener(Second_Dialogue_Start);

        OnPopUpInstruction.AddListener(Second_Dialogue_Finish);
        
        ShowTutorialPanel();

        if (DialogueList.Count > 1)
        {
            //Update Dialogue
            NextDialogue();

            //AddListener
            DialogueList.First().OnDialogueEnd.AddListener(Second_Dialogue_Finish);
        }
    }

    void Second_Dialogue_Finish()
    {
        //Handle RemoveListener
        if(DialogueList.Count>1) DialogueList.First().OnDialogueEnd.RemoveListener(Second_Dialogue_Finish);
        OnPopUpInstruction.RemoveListener(Second_Dialogue_Finish);

        boardHandler.OnEndBoardInteraction.RemoveListener(teamAttack.AddToTeamAttackAmount);

        //ClearBoard
        boardHandler.IsClearBoard = true;
        boardHandler.SelectAllTiles();
        boardHandler.OnTileRelease(null);

        combatManager.OnCurrentRoundEnd.AddListener(ShowHidePopUps);
        boardHandler.OnTileMatchedRelease.AddListener(Second_Dialogue_CheckCondition);
    }

    void Second_Dialogue_Rematch()
    {
        combatManager.ChangeCombatState(CombatState.Match3Turn);

        ShowHidePopUps();
        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.RemoveListener(teamAttack.AddToTeamAttackAmount);
        boardHandler.OnTileMatchedRelease.AddListener(Second_Dialogue_CheckCondition);
    }

    void Second_Dialogue_CheckCondition()
    {
        boardHandler.OnTileMatchedRelease.RemoveListener(Second_Dialogue_CheckCondition);
        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);

        if(ReachTutorialRequirement() == true)
        {
            Second_Dialogue_ConditionMet();
        }
        else
        {
            ShowHidePopUps();
            Second_Dialogue_Reset();
        }
    }

    void Second_Dialogue_ConditionMet()
    {
        //Handle RemoveListener
        boardHandler.OnEndBoardInteraction.RemoveListener(Second_Dialogue_Rematch);
        boardHandler.OnEndBoardInteraction.AddListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.AddListener(teamAttack.AddToTeamAttackAmount);

        combatManager.OnCurrentRoundEnd.RemoveListener(ShowHidePopUps);
        combatManager.OnCurrentRoundEnd.AddListener(Third_Dialogue_Start);

        ShowHidePopUps();
        HideTutorialPanel();
    }

    void Second_Dialogue_Reset()
    {
        //Handle RemoveListener
        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.RemoveListener(Second_Dialogue_Rematch);
        boardHandler.OnEndBoardInteraction.AddListener(Second_Dialogue_Rematch);
        boardHandler.OnEndBoardInteraction.RemoveListener(teamAttack.AddToTeamAttackAmount);

        //ClearBoard
        boardHandler.IsClearBoard = true;
        boardHandler.SelectAllTiles();

        combatManager.OnTutorialRoundEnd.Invoke();
    }

    void Third_Dialogue_Start()
    {
        combatManager.OnCurrentRoundEnd.RemoveListener(Third_Dialogue_Start);

        //AddSkillToRogue
        SkillActor sActor = combatManager.AllyTeam.Find(c => c.CharacterName == "Rogue").GetComponent<SkillActor>();
        sActor.AddSkill(RogueSkills.First());

        combatManager.OnTutorialRoundEnd.Invoke();

        OnPopUpInstruction.AddListener(Third_Dialogue_Finish);
        ShowTutorialPanel();

        if (DialogueList.Count > 1)
        {
            //Update Dialogue
            NextDialogue();

            //AddListener
            DialogueList.First().OnDialogueEnd.AddListener(Third_Dialogue_Finish);
        }
    }
    
    void Third_Dialogue_Finish()
    {
        //RemoveListener
        if(DialogueList.Count>1) DialogueList.First().OnDialogueEnd.RemoveListener(Third_Dialogue_Finish);
        OnPopUpInstruction.RemoveListener(Third_Dialogue_Finish);

        boardHandler.OnEndBoardInteraction.RemoveListener(teamAttack.AddToTeamAttackAmount);

        //ClearBoard
        boardHandler.IsClearBoard = true;
        boardHandler.SelectAllTiles();
        boardHandler.OnTileRelease(null);

        combatManager.OnCurrentRoundEnd.AddListener(ShowHidePopUps);
        boardHandler.OnTileMatchedRelease.AddListener(Third_Dialogue_CheckCondition);
    }

    void Third_Dialogue_Rematch()
    {
        combatManager.ChangeCombatState(CombatState.Match3Turn);

        ShowHidePopUps();
        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnTileMatchedRelease.AddListener(Third_Dialogue_CheckCondition);
    }

    void Third_Dialogue_CheckCondition()
    {
        boardHandler.OnTileMatchedRelease.RemoveListener(Third_Dialogue_CheckCondition);
        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);

        if (ReachTutorialRequirement() == true)
        {
            Third_Dialogue_ConditionMet();
        }
        else
        {
            ShowHidePopUps();
            Third_Dialogue_Reset();
        }
    }

    void Third_Dialogue_ConditionMet()
    {
        //Handle RemoveListener
        boardHandler.OnEndBoardInteraction.RemoveListener(Third_Dialogue_Rematch);
        boardHandler.OnEndBoardInteraction.AddListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.AddListener(teamAttack.AddToTeamAttackAmount);

        combatManager.OnCurrentRoundEnd.RemoveListener(ShowHidePopUps);
        combatManager.OnCurrentRoundEnd.AddListener(Forth_Dialogue_Start);

        ShowHidePopUps();
        HideTutorialPanel();
    }

    void Third_Dialogue_Reset()
    {
        //Handle RemoveListener
        boardHandler.OnEndBoardInteraction.RemoveListener(combatManager.StartTurnBasedCombat);
        boardHandler.OnEndBoardInteraction.RemoveListener(Third_Dialogue_Rematch);
        boardHandler.OnEndBoardInteraction.AddListener(Third_Dialogue_Rematch);
        boardHandler.OnEndBoardInteraction.RemoveListener(teamAttack.AddToTeamAttackAmount);

        //ClearBoard
        boardHandler.IsClearBoard = true;
        boardHandler.SelectAllTiles();

        combatManager.OnTutorialRoundEnd.Invoke();
    }

    void Forth_Dialogue_Start()
    {
        combatManager.OnCurrentRoundEnd.RemoveListener(Forth_Dialogue_Start);

        ShowTutorialPanel();

        boardHandler.OnTileMatchedRelease.AddListener(Forth_Action_End);
    }

    void Forth_Action_End()
    {
        boardHandler.OnTileMatchedRelease.RemoveListener(Forth_Action_End);

        combatManager.OnCurrentRoundEnd.AddListener(Five_Action_Start);

        HideTutorialPanel();
    }

    void Five_Action_Start()
    {
        combatManager.OnCurrentRoundEnd.RemoveListener(Five_Action_Start);

        ShowTutorialPanel();

        boardHandler.OnTileMatchedRelease.AddListener(Five_action_End);
    }

    void Five_action_End()
    {
        boardHandler.OnTileMatchedRelease.RemoveListener(Five_action_End);

        HideTutorialPanel();
        ShowTutorialPanel();

        AddSkillToAllies();
        OnEndTutorial();
    }
    #endregion

    #region Methods
    int FindIndexInTileRequirements()
    {
        int index = -1;

        //Find Index
        foreach (var item in TileRequirements)
        {
            if (item.InIndex == Index)
            {
                index = item.InIndex;
                break;
            }
        }

        return index;
    }
    int FindIndexInTileTypeRequirements()
    {
        int index = -1;

        //Find Index
        foreach (var item in TileTypeRequirements)
        {
            if (item.InIndex == Index)
            {
                index = item.InIndex;
                break;
            }
        }

        return index;
    }
    bool ReachTutorialRequirement()
    {
        if (MeetTileRequirements() != false) return true;
        if (MeetTileTypeRequirements() != false) return true;

        return false;
    }
    bool MeetTileRequirements()
    {
        int index = FindIndexInTileRequirements();
        if (index != -1)
        {
            List<DoubleInt> coords = TileRequirements[index].TileCoords;
            int count = coords.Count;

            //For Checking if requirements meet
            int counter = 0;

            //First Stage Checking
            if (boardHandler.Tiles.Count == coords.Count)
            {
                //Search same Index, if Found, Counter ++
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        if (boardHandler.Tiles[i].Coord.x == coords[j].x && boardHandler.Tiles[i].Coord.y == coords[j].y)
                        {
                            counter++;
                            break;
                        }
                    }
                }

                //Condition Meet
                if (counter == count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }   
        }
        return false;
    }
    bool MeetTileTypeRequirements()
    {
        int index = FindIndexInTileTypeRequirements();
        if (index != -1)
        {
            List<Tile> tiles = boardHandler.Tiles;
            TileTypeRequirement requirements = TileTypeRequirements[index];

            if(requirements.IsLevelThree)
            {
                int minimum = 9;
                int overallCount = 0;

                foreach (var tileType in requirements.tileTypes)
                {
                    int count = tiles.FindAll(c => c.Type == tileType.type).Count;

                    if (count < tileType.Minimum && count > tileType.Maximum) return false;
                    overallCount += count;
                }

                if (overallCount < minimum) return false;
            }
            else
            {
                foreach (var tileType in requirements.tileTypes)
                {
                    int count = tiles.FindAll(c => c.Type == tileType.type).Count;

                    if (count < tileType.Minimum && count > tileType.Maximum) return false;
                }
            }

            return true;
        }
        return false;
    }
    #endregion
}