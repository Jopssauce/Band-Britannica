using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterUI : MonoBehaviour
{
    public GameObject SelectedIndicatorPrefab;
    public Transform ParentObject;

    public Vector3 Offset;

    private GameObject enemyIndicator;

    private CombatManager cManager;
    private CharacterSelection characterSelection;

    // Start is called before the first frame update
    void Start()
    {
        cManager = SingletonManager.Get<CombatManager>();
        characterSelection = SingletonManager.Get<CharacterSelection>();

        //Add Listener
        characterSelection.OnSelectCharacter.AddListener(UpdateIndicator);
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (cManager.SelectedEnemy != null)
            {
                enemyIndicator = Instantiate(SelectedIndicatorPrefab, SetIndicatorPosition(cManager.SelectedEnemy), Quaternion.identity, ParentObject) as GameObject;
                yield break;
            }
        }
    }

    void UpdateIndicator(Character selected)
    {
        SyncIndicator(cManager.EnemyTeam, cManager.SelectedEnemy, enemyIndicator);
    }

    void SyncIndicator(List<Character> characterList, Character character, GameObject indicator)
    {
        if(indicator != null)
        {
            if (characterList.Count != 0)
            {
                indicator.transform.position = SetIndicatorPosition(character);
            }
            else indicator.SetActive(false);
        }
    }

    Vector3 SetIndicatorPosition(Character character)
    {
        if (character != null)
        {
            return (character.transform.position + Offset);
        }
        else return Vector3.zero;
    }
}
