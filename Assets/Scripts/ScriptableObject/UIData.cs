using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Combinations
{
    public AttributeType[] elements;
}

[CreateAssetMenu(fileName = "UIData", menuName = "Data SciptableObjects")]
public class UIData : ScriptableObject
{
    //Data for mana display
    public int currentMana;
    public int maxMana;

    //Data for Match Display
    public Character[] actors;
    public Skill[] skills;
    public Combinations[] combinations;
    public float[] intensity;

    //Data for enemy stats display
    public Character selectedEnemy;
    public Skill[] enemySkills;
}
