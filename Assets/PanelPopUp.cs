using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class PanelPopUp : MonoBehaviour
{
    public bool IsPersistent = false;
    public bool isUp = false;
    public float animationTime;
    public float yIncrement;

    public UnityEvent OnPopUp;
    public UnityEvent OnPopDown;

    private void Start()
    {
        OnPopDown.AddListener(HideObject);
    }

    public void PopUp()
    {
        if (this.gameObject.activeSelf == false) this.gameObject.SetActive(true);

        this.transform.DOBlendableLocalMoveBy(new Vector3(0, yIncrement, 0), animationTime);
        if (gameObject.activeSelf) StartCoroutine(AnimationDoneRoutine(OnPopUp));
    }
    public void PopDown()
    {
        this.transform.DOBlendableLocalMoveBy(new Vector3(0, -(yIncrement), 0), animationTime);
        if(gameObject.activeSelf) StartCoroutine(AnimationDoneRoutine(OnPopDown));
    }

    void HideObject()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator AnimationDoneRoutine(UnityEvent eventToPass)
    {
        yield return new WaitForSeconds(animationTime);

        eventToPass.Invoke();
        isUp = !isUp;
    }
}
