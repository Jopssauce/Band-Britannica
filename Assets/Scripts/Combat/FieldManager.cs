using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldManager : MonoBehaviour
{
    public Transform parentTransform;
    public List<Transform> AllyTransforms;
    public List<Transform> EnemyTransforms;

    [Header("Characters")]
    public List<Character> AlliesPrefab;
    public List<Character> EnemiesPrefab;

    [Header("Icons")]
    public List<GameObject> AlliesIcons;

    [Header("Spawned Characters")]
    public List<Character> SpawnedAllies;
    public List<Character> SpawnedEnemies;
    public bool HaveSkill = true;

    [Header("Stats UI")]
    public GameObject allyCanvas;
    public GameObject enemyCanvas;

    public UnityEvent OnFieldInitialized = new UnityEvent();

    public StageInfo stageToUnlock;
    public StageInfo currentStage;
    protected CombatManager combat;

    private void Awake()
    {
        SingletonManager.Register<FieldManager>(this,SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeBattlefield());
    }

    IEnumerator InitializeBattlefield()
    {
        // Spawn Ally Characters
        SpawnCharacters(AlliesPrefab, AllyTransforms, SpawnedAllies, allyCanvas, HaveSkill, true);

        // Spawn Enemy Characters
        SpawnCharacters(EnemiesPrefab, EnemyTransforms, SpawnedEnemies, enemyCanvas, true, false);

        yield return new WaitUntil(() => SingletonManager.Get<CombatManager>());

        combat = SingletonManager.Get<CombatManager>();
        if (stageToUnlock != null) combat.stageToUnlock = this.stageToUnlock;
        if (currentStage != null) combat.currentStage = this.currentStage;

        OnFieldInitialized.Invoke();
    }

    void SpawnCharacters(List<Character> team, List<Transform> transforms, List<Character> spawnList, GameObject canvas, bool haveSkill, bool hasUniqueIcon)
    {
        for (int i = 0; i < team.Count; i++)
        {
            Vector3 position;
            Quaternion rotation;

            if (transforms[i] == null)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }
            else
            {
                position = transforms[i].position;
                rotation = transforms[i].rotation;
            }

            Character character = Instantiate(team[i], position, rotation, parentTransform) as Character;
            if (haveSkill == false) RemoveUniqueSkills(character.GetComponent<SkillActor>());

            //Canvas
            GameObject statCanvas = Instantiate(canvas);
            statCanvas.GetComponent<StatsUIDisplay>().character = character;
            statCanvas.transform.SetParent(character.transform);

            if (hasUniqueIcon)
            {
                Instantiate(AlliesIcons[i], statCanvas.transform.GetChild(0));
            }

            spawnList.Add(character);
        }
    }

    void RemoveUniqueSkills(SkillActor skillActor)
    {
        Skill defSkill = skillActor.skills.First();

        skillActor.skills.Clear();

        if (defSkill.SkillRequirements.Count != 0) return;
        skillActor.skills.Add(defSkill);
    }
}
