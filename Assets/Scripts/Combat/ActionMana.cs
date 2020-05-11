using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMana : MonoBehaviour
{
    const int MANA_PER_ACTION = 1;

    public int CurrentAction;
    public int MaxAction;

    CombatManager combatManager;
    BoardHandler boardHandler;

    private void Awake()
    {
        SingletonManager.Register<ActionMana>(this,SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        MaxAction = CurrentAction;

        combatManager = SingletonManager.Get<CombatManager>();
        boardHandler = SingletonManager.Get<BoardHandler>();

        combatManager.OnCurrentRoundEnd.AddListener(ResetActions);
        combatManager.OnTutorialRoundEnd.AddListener(ResetActions);
        combatManager.OnCombatStateChanged.AddListener(UpdateActions);
    }

    public float NormalizeActionManaAmount()
    {
        return (float)CurrentAction / (float)MaxAction;
    }

    public void UseAction()
    {
        CurrentAction -= MANA_PER_ACTION;
    }

    public void UnuseAction()
    {
        CurrentAction += MANA_PER_ACTION;
    }

    void ResetActions()
    {
        CurrentAction = MaxAction;
    }

    void UpdateActions(CombatState state)
    {
        if(state != CombatState.Match3Turn)
        {
            boardHandler.OnAddTile.RemoveListener(UseAction);
            boardHandler.OnRemoveTile.RemoveListener(UnuseAction);
        }
        else
        {
            boardHandler.OnAddTile.AddListener(UseAction);
            boardHandler.OnRemoveTile.AddListener(UnuseAction);
        }
    }

    public bool CanConnectMore()
    {
        return CurrentAction > 0;
    }
}
