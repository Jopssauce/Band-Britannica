using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SkillEvent:UnityEvent<SkillActor, Character[]> { }

public enum SkillType
{
    Offensive,
    Utility
}

public class Skill : MonoBehaviour
{
    public string skillName;
    public float cost;
    public SkillType type;
    [TextArea]
    public string description;
    [TextArea]
    public List<string> Descriptions;

    public List<int> InsertIndexes;

    public List<AttributeType> SkillRequirements;
    public Intensity _Intensity;

    public SkillEvent OnActivate;
    public UnityEvent OnActivated;

    [HideInInspector]
    public SkillActor actor;
    protected Character[] allies;
    protected Character enemy;
    protected Character[] enemies;

    private BoardHandler boardHandler;
    protected AudioManager audioManager;
    protected int matchedCount = 0;

    [HideInInspector]
    public int Lvl1Req = 5;
    [HideInInspector]
    public int Lvl2Req = 8;
    [HideInInspector]
    public int Lvl3Req = 10;

    protected virtual void Start()
    {
        StartCoroutine(InitializeListener());
    }

    IEnumerator InitializeListener()
    {
        yield return new WaitUntil(() => SingletonManager.Get<BoardHandler>());

        boardHandler = SingletonManager.Get<BoardHandler>();
        audioManager = SingletonManager.Get<AudioManager>();

        try
        {
            //Add Listener
            boardHandler.OnAddTile.AddListener(SetMatchCount);
            boardHandler.OnRemoveTile.AddListener(SetMatchCount);
        }
        catch (System.Exception)
        {
            Debug.LogError("Need BoardHandler Ref");
        }
    }

    public virtual void Activate(SkillActor actor, Character[] allies, Character enemy, Character[] enemies)
    {
        this.actor = actor;
        this.allies = allies;
        this.enemy = enemy;
        this.enemies = enemies;
        OnActivate.Invoke(actor, enemies);
        SkillEffect();
    }

    public virtual void SkillEffect() { }
    public virtual void SkillEffectEnd()
    {
        OnActivated.RemoveAllListeners();
        OnActivate.RemoveAllListeners();
    }
    protected virtual void UpdateSkillDescription() { }

    void SetMatchCount()
    {
        matchedCount = boardHandler.Tiles.Count;
    }

    public bool CanCastThisSkill(List<AttributeType> types)
    {
        if (SkillRequirements.Count == 0) return false;

        int count = SkillRequirements.Count;

        List<AttributeType> temp = new List<AttributeType>();
        List<int> indexes = new List<int>();

        if(SkillRequirements.Count(a => a == SkillRequirements.First()) > 1 && types.Count(a => a == SkillRequirements.First()) > 1)
        {
            indexes = GetAllIndexOfType(SkillRequirements.First(), types);

            while (indexes.Count > SkillRequirements.Count)
            {
                indexes.Remove(indexes.Last());
            }
        }
        else
        {
            foreach (var type in SkillRequirements)
            {
                int index = 0;

                if (indexes.Count == 0)
                {
                    index = types.FindIndex(indexes.Count, a => a == type);
                }
                else index = types.FindIndex(indexes.Last(), a => a == type);

                //No Requirement in current match, so return false
                if (index < 0)
                {
                    return false;
                }

                indexes.Add(index);
            }
        }

        if (indexes.Count == 0) return false;

        int num = indexes.First();
        
        for (int i = 1; i < indexes.Count; i++)
        {
            if (num < indexes[i]) num = indexes[i];
            else return false;
        }

        return true;
    }

    List<int> GetAllIndexOfType(AttributeType refType, List<AttributeType> typeList)
    {
        List<int> list = new List<int>();

        for (int i = 0; i < typeList.Count; i++)
        {
            if(typeList[i] == refType)
            {
                list.Add(i);
            }
        }

        return list;
    }

    public virtual int IntensityLevel
    {
        get
        {
            if (matchedCount <= Lvl1Req)
            {
                return 1;
            }
            else if (matchedCount > Lvl1Req && matchedCount <= Lvl2Req)
            {
                return 2;
            }
            else if (matchedCount > Lvl2Req && matchedCount <= Lvl3Req || matchedCount > Lvl3Req)
            {
                return 3;
            }
            else return 0;
        }
    }

    public string Description
    {
        get
        {
            UpdateSkillDescription();

            description = "";
            foreach (var text in Descriptions)
            {
                description += text;
            }
            return description;
        }
    }
}