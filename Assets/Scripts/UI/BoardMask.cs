using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMask : MonoBehaviour
{
    public GameObject BoardMaskPanel;

    private CombatManager combatManager;

    // Start is called before the first frame update
    void Start()
    {
        combatManager = SingletonManager.Get<CombatManager>();

        combatManager.OnCombatStateChanged.AddListener(MaskBoard);
    }

    void MaskBoard(CombatState combatState)
    {
        if(combatState != CombatState.Match3Turn)
        {
            BoardMaskPanel.SetActive(true);
        }
        else
        {
            BoardMaskPanel.SetActive(false);
        }
    }
}
