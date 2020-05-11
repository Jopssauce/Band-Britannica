using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptObject/StageInfo")]
public class StageInfo : ScriptableObject
{
    public string missionTitle;

    [TextArea]
    public string missionDescription;
    public bool unlocked;
    public bool completed;
    public string missionNumber;
}
