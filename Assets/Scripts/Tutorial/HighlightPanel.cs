using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightPanel : MonoBehaviour
{
    public TutorialManager tutorial;
    public DialogueSystem dialogue;

    public int dialogueListIndex;
    public int indexToShow;
    public List<GameObject> panels;

    void Start()
    {
        dialogue = tutorial.DialogueList[dialogueListIndex];
    }

    void Update()
    {
        if (dialogue == null) return;
        
        if(dialogue.curDialogueIndex == indexToShow)
        {
            foreach (GameObject item in panels)
            {
                item.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject item in panels)
            {
                item.SetActive(false);
            }
        }
    }
}
