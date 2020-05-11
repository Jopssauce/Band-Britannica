using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptObject/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea]
    public List<string> Dialogues;
}
