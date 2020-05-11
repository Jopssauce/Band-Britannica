using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDisplay : TextAndFillDisplay
{
    public SkillDisplay skillDisplay;
    protected ActionMana actionMana;

    protected int counter = 0;
    
    void Start()
    {
        if(textDisplay != null) textDisplay.text = fillNormalizer.ToString();
        if(fillDisplay != null) fillDisplay.fillAmount = 1;

        board = SingletonManager.Get<BoardHandler>();
        combat = SingletonManager.Get<CombatManager>();
        actionMana = SingletonManager.Get<ActionMana>();

        combat.OnCurrentRoundEnd.AddListener(ResetDisplayValue);
        combat.OnTutorialRoundEnd.AddListener(ResetDisplayValue);
        board.OnAddTile.AddListener(SubtractDisplayValue);
        board.OnRemoveTile.AddListener(AddDisplayValue);

        desiredValue = actionMana.CurrentAction;
        resetToValue = actionMana.CurrentAction;
        fillNormalizer = actionMana.CurrentAction;
    }

    public override void AddDisplayValue()
    {
        if(board.Tiles.Count > 0) skillDisplay.RemoveTile();
        counter--;   
        base.AddDisplayValue();
    }

    public override void SubtractDisplayValue()
    {
        if(board.Tiles.Count >= 1)
        {
            skillDisplay.AddTile(board.Tiles[counter]);
            counter++;
        }
        base.SubtractDisplayValue();
    }

    public override void ResetDisplayValue()
    {
        counter = 0;
        base.ResetDisplayValue();
    }
}