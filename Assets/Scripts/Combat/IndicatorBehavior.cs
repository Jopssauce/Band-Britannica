using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndicatorBehavior : MonoBehaviour
{
    public string Text = "";

    [Range(12.0f, 30.0f)]
    public float TargetFontSize = 12.0f;

    [Range(0.15f, 0.50f)]
    public float Interval = 0.15f;

    private float offset = 5.5f;
    private float initialFontSize = 0;

    private Character target;
    private TextMeshPro textMesh;

    private Coroutine displayBehaviorCoroutine;

    IEnumerator StartBehavior()
    {
        bool reachMax = false;

        textMesh = transform.GetChild(0).GetComponent<TextMeshPro>();
        textMesh.text = Text;
        initialFontSize = textMesh.fontSize;

        while (true)
        {
            transform.LookAt(transform.position - Camera.main.transform.position);
            if(target != null) transform.position = target.transform.position + Vector3.up * offset;
            
            if(reachMax != true)
            {
                reachMax = ReachMaxTarget();
            }
            else
            {
                if (ReturnToOriginal() == true)
                {
                    StartCoroutine(DestroyThis());
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    bool ReachMaxTarget()
    {
        if (textMesh.fontSize <= TargetFontSize)
        {
            textMesh.fontSize += Interval;
            return false;
        }
        else return true;
    }

    bool ReturnToOriginal()
    {
        if (textMesh.fontSize > initialFontSize)
        {
            textMesh.fontSize -= Interval;
            return false;
        }
        else return true;
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForEndOfFrame();

        Destroy(this.gameObject);
        StopAllCoroutines();
    }

    public void SetCharacter(Character character)
    {
        target = character;

        //Start Coroutine
        displayBehaviorCoroutine = StartCoroutine(StartBehavior());
    }
}
