using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    public GameObject DialoguePanel;
    public Dialogue DialogueScriptObj;
    public KeyCode NextKey = KeyCode.Mouse0;

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;
    public UnityEvent OnDialogueDisplayEnd;
    public UnityEvent OnNextDialogueStart;

    [HideInInspector]
    public int curDialogueIndex = 0;

    protected bool canClick = false;
    public GameObject continueText;

    //Managers
    private CombatManager cm;

    private Text dialogueText;
    private List<string> dialogues;
    private string currentText = string.Empty;
    private readonly float textSpeed = 0.05f;
    private Coroutine displayTextCoroutine = null;

    private void OnEnable()
    {
        if (cm == null) cm = SingletonManager.Get<CombatManager>();

        //Save the dialogues' texts
        dialogues = DialogueScriptObj.Dialogues;

        //Get Text component
        dialogueText = DialoguePanel.GetComponentInChildren<Text>();

        //Show Up the Dialogue Panel
        DialoguePanel.SetActive(true);

        //Add Listener
        OnDialogueEnd.AddListener(DialogueDone);
        OnDialogueDisplayEnd.AddListener(ShowContinueText);

        ShowDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(NextKey) && cm != null && cm.CurCombatState == CombatState.DialogueMode)
        {
            if (curDialogueIndex < dialogues.Count && displayTextCoroutine != null)
            {
                //Manage Coroutine
                StopCoroutine(displayTextCoroutine);
                displayTextCoroutine = null;

                dialogueText.text = currentText;
                ShowContinueText();
            }
            else
            {
                //Start of next Dialogue
                //Hide Continue Text
                continueText.SetActive(false);

                if (curDialogueIndex < dialogues.Count - 1)
                {
                    curDialogueIndex++;
                    ShowDialogue();
                }
                else
                {
                    OnDialogueEnd.Invoke();
                }

                canClick = false;
                OnNextDialogueStart.Invoke();
            }
        }
    }

    void ShowDialogue()
    {
        displayTextCoroutine = StartCoroutine(TypeWriter(dialogues[curDialogueIndex]));
    }

    void DialogueDone()
    {
        Debug.Log("On Dialogue End");
        DialoguePanel.SetActive(false);
    }

    void ShowContinueText()
    {
        canClick = true;
        continueText.SetActive(canClick);
    }

    IEnumerator TypeWriter(string text)
    {
        //Use by Later
        currentText = text;

        bool skipText = false;
        dialogueText.text = string.Empty;

        for (int i = 0; i < currentText.Length; i++)
        {
            if (currentText[i] == '<') skipText = true;

            if (skipText == false)
            {
                dialogueText.text += currentText.Substring(0, i + 1).Last();
                yield return new WaitForSeconds(textSpeed);
            }

            if (currentText[i] == '>') skipText = false;
        }

        dialogueText.text = currentText;
        OnDialogueDisplayEnd.Invoke();
        Debug.Log("OnDialogueDisplay Completed");

        displayTextCoroutine = null;
    }
}
