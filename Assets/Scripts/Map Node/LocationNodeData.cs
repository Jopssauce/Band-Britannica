using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationNodeData : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [System.Serializable]
    public class EnemyTypesInWave
    {
        public List<GameObject> Enemy;
    }
    
    public Difficulty difficulty;
    public List<EnemyTypesInWave> wave;

    
}