using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class IntroDialogue : MonoBehaviour
{
    [TextArea]
    public string[] strings;
    public TextMeshProUGUI tmp;
    public float seconds = 0;

    public UnityEvent OnStringsEmpty;

    Coroutine typeText;
    int index = 0;

    private void Start()
    {
        NextString();
    }

    public void NextString()
    {
        SingletonManager.Get<AudioManager>().PlaySFX("Button_Click");
        if (index >= strings.Length)
        {
            SceneController.instance.LoadScene("Map Scene");
            return;
        }
        if (typeText != null)StopCoroutine(typeText);
        typeText = StartCoroutine(TypeText(strings[index]));
        index++;
        
    }

    public IEnumerator TypeText(string text)
    {
        tmp.text = "";
        int index = 0;
        while (tmp.text.Length < text.Length)
        {
            tmp.text += text[index];
            yield return new WaitForSeconds(seconds);
            index++;
        }

        yield return null;
    }
   
}
