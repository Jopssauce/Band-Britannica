using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CombatStateUI : MonoBehaviour
{
    public Text BattleModeText;
    public string BattleStartTxt = "Battle Start";
    public string Match3Txt = "Match Tiles";
    public string MatchClearingTxt = "Match Clearing..";
    public string AllyTxt = "Ally Turn";
    public string EnemyTxt = "Enemy Turn";
    public string BattleEndTxt = "Battle End";

    public UnityEvent OnWinTeamDisplay;

    private float timer = 0.6f;
    CombatManager cManager;
    ResultUI resultUI;

    private void Awake()
    {
        SingletonManager.Register<CombatStateUI>(this, SingletonType.Occasional);
    }

    private void Start()
    {
        cManager = SingletonManager.Get<CombatManager>();

        StartCoroutine("UpdateCombatText");
    }

    IEnumerator UpdateCombatText()
    {
        while(true)
        {
            string text = "";

            switch (cManager.CurCombatState)
            {
                case CombatState.BattleStart:
                    text = BattleStartTxt;
                    break;
                case CombatState.Match3Turn:
                    text = Match3Txt;
                    break;
                case CombatState.MatchClearing:
                    text = MatchClearingTxt;
                    break;
                case CombatState.AllyTurn:
                    text = AllyTxt;
                    break;
                case CombatState.EnemyTurn:
                    text = EnemyTxt;
                    break;
                case CombatState.BattleEnd:
                    text = BattleEndTxt;
                    break;
                default:
                    break;
            }

            BattleModeText.text = text;

            if(BattleModeText.text == BattleEndTxt)
            {
                Invoke("EndBattleModeText", timer);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void EndBattleModeText()
    {
        BattleModeText.text = cManager.CurTeamWin.ToString() + " Wins";
        OnWinTeamDisplay.Invoke();
    }
}
