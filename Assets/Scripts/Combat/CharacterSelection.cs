using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CharacterEvent : UnityEvent<Character> {};

public class CharacterSelection : MonoBehaviour
{
    public CharacterEvent OnSelectCharacter;
    
    private Ray ray;
    private RaycastHit hit;
    private CombatManager cManager;

    private void Awake()
    {
        SingletonManager.Register<CharacterSelection>(this, SingletonType.Occasional);
    }

    private void Start()
    {
        cManager = SingletonManager.Get<CombatManager>();
        
        //Add Event Listener
        cManager.OnCharacterDead.AddListener(AutoSetSelectedCharacter);

        StartCoroutine("Initialize");
    }

    IEnumerator Initialize()
    {
        while(true)
        {
            //Call after other script Update function called
            yield return new WaitForEndOfFrame();

            if (cManager.EnemyTeam.Count != 0)
            {
                cManager.SelectedEnemy = cManager.EnemyTeam.First();

                OnSelectCharacter.Invoke(cManager.SelectedEnemy);
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && cManager.CurCombatState == CombatState.Match3Turn)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            ManualSetSelectedCharacter();
        }
    }

    void ManualSetSelectedCharacter()
    {
        if (Physics.Raycast(ray, out hit))
        {
            Character character = hit.collider.gameObject.GetComponent<Character>();

            if (character != null)
            {
                if (cManager.EnemyTeam.Contains(character) == true)
                {
                    cManager.SelectedEnemy = character;
                }

                OnSelectCharacter.Invoke(character);
            }
        }
    }

    void AutoSetSelectedCharacter(Character character, List<Character> team)
    {
        Character newSelected = null;

        if (team.Count != 0)
        {
            newSelected = team.First();

            //Get Lowest HP Character
            foreach (var c in team)
            {
                if (c == null) return;

                if (c.Health < newSelected.Health) newSelected = c;
            }
        }

        if(team == cManager.EnemyTeam)
        {
            cManager.SelectedEnemy = newSelected;

            OnSelectCharacter.Invoke(newSelected);
        }
    }
}
