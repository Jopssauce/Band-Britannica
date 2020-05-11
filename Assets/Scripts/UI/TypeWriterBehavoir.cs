using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TypeWriterBehavoir : MonoBehaviour
{
    public float TextSpeed = 0.05f;

    private Coroutine coroutine;
    private Text textCom;
    private string mainText;

    private void OnEnable()
    {
        textCom = GetComponent<Text>();

        coroutine = StartCoroutine(TypeWriter(textCom.text));
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);

        textCom.text = mainText;
    }

    IEnumerator TypeWriter(string text)
    {
        //Use by Later
        mainText = text;

        bool skipText = false;
        textCom.text = string.Empty;

        for (int i = 0; i < mainText.Length; i++)
        {
            if (mainText[i] == '<') skipText = true;

            if (skipText == false)
            {
                textCom.text += mainText.Substring(0, i + 1).Last();
                yield return new WaitForSeconds(TextSpeed);
            }

            if (mainText[i] == '>') skipText = false;
        }

        textCom.text = mainText;
    }
}
