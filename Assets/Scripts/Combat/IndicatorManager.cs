using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IndicatorManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject DamageIndicator;
    public GameObject HealIndicator;

    public string AddSignString = "+ ";
    public string MinusSignString = "- ";

    public List<Character> characters = new List<Character>();
    public GameObject spawnedIndicator;

    private CombatManager combatManager;
    private List<Character> allyTeam = new List<Character>();
    private List<Character> enemyTeam = new List<Character>();

    private void Awake()
    {
        SingletonManager.Register<IndicatorManager>(this, SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        combatManager = SingletonManager.Get<CombatManager>();
        combatManager.OnInitialTeamSynced.AddListener(GetTeamRef);
        //combatManager.OnCurrentRoundEnd.AddListener(UpdateCharacters);
    }

    void GetTeamRef()
    {
        AddCharacters(combatManager.AllyTeam);
        AddCharacters(combatManager.EnemyTeam);
    }

    void AddCharacters(List<Character> charactersRef)
    {
        foreach (var character in charactersRef)
        {
            character.OnAddHealth.AddListener(AddHealth);
            character.OnDeductHealth.AddListener(DeductHealth);
            character.OnAddHealth.AddListener(delegate { SetIndicatorTarget(character); });
            character.OnDeductHealth.AddListener(delegate { SetIndicatorTarget(character); });

            characters.Add(character);
        }
    }

    void RemoveListenersToCharacters()
    {
        foreach (var character in characters)
        {
            character.OnAddHealth.RemoveAllListeners();
            character.OnDeductHealth.RemoveAllListeners();
        }

        characters.Clear();
    }

    void UpdateCharacters()
    {
        spawnedIndicator = null;
        RemoveListenersToCharacters();

        GetTeamRef();
    }

    void AddHealth(float amount)
    {
        spawnedIndicator = Instantiate(HealIndicator) as GameObject;
        IndicatorBehavior behavior = spawnedIndicator.GetComponent<IndicatorBehavior>();
        behavior.Text = AddSignString + amount.ToString();
    }

    void DeductHealth(float amount)
    {
        spawnedIndicator = Instantiate(DamageIndicator) as GameObject;
        IndicatorBehavior behavior = spawnedIndicator.GetComponent<IndicatorBehavior>();
        behavior.Text = MinusSignString + amount.ToString();
    }

    void SetIndicatorTarget(Character character)
    {
        IndicatorBehavior behavior = spawnedIndicator.GetComponent<IndicatorBehavior>();
        behavior.SetCharacter(character);
    }
}
