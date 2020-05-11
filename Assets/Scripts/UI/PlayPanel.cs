using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class PlayPanel : MonoBehaviour
{
    public float openTime;
    public float closeTime;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    public void OpenPanel()
    {
        StartCoroutine(OpenPanelRoutine());
    }
    public void ClosePanel()
    {
        StartCoroutine(ClosePanelRoutine());
    }

    public IEnumerator OpenPanelRoutine()
    {
        this.transform.DOBlendableMoveBy(new Vector3(0,-115,0), openTime);
        yield return new WaitForSeconds(openTime);
        OnOpen.Invoke();

        foreach (var item in this.GetComponent<UIController>().buttons)
        {
            item.GetComponent<ButtonAnimator>().originPosition = item.transform.position;
        }

        
    }

    public IEnumerator ClosePanelRoutine()
    {
        this.transform.DOBlendableMoveBy(new Vector3(0, 115,0), closeTime);
        yield return new WaitForSeconds(closeTime);
        OnClose.Invoke();
    }
}