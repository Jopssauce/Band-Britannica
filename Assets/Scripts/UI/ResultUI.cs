using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public GameObject ResultPanel;
    public TextMeshProUGUI StateText;

    [Header("Strings")]
    public string Complete = "Mission Complete!";
    public string Fail = "Mission Fail!";

    private CombatManager combatManager;
    private CombatStateUI stateUI;

    private void Awake()
    {
        SingletonManager.Register<ResultUI>(this, SingletonType.Occasional);
    }

    private void Start()
    {
        combatManager = SingletonManager.Get<CombatManager>();
        stateUI = SingletonManager.Get<CombatStateUI>();

        //Add Listener
        stateUI.OnWinTeamDisplay.AddListener(ShowResult);
    }

    public void ShowResult()
    {
        //Set Text
        if (combatManager.CurTeamWin == CombatManager.Team.Ally) StateText.text = Complete;
        else StateText.text = Fail;

        //Make sure ResultPanel is active
        ResultPanel.SetActive(true);
    }
}
