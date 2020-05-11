using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SplashScreen : MonoBehaviour
{
    public float fadeInDuration;
    public float fadeOutDuration;
    public float buttonMoveDuration;

    public List<Image> images;
    public List<GameObject> buttons;
    public UIController controllerUI;

    protected int counter = 0;

    void Start()
    {
        foreach (var item in images)
        {
            item.canvasRenderer.SetAlpha(0.0f);
        }
        StartCoroutine(ShowImagesRoutine());
    }

    public IEnumerator ShowImagesRoutine()
    {
        images[counter].CrossFadeAlpha(1.0f, fadeInDuration, false);
        yield return new WaitForSeconds(fadeInDuration+1f);
        images[counter].CrossFadeAlpha(0.0f, fadeOutDuration, false);
        yield return new WaitForSeconds(fadeOutDuration);

        counter++;
        if(counter == images.Count)
        {
            StartCoroutine(ShowButtonsRoutine());
        }
        else
        {
            StartCoroutine(ShowImagesRoutine());
        }
    }

    public IEnumerator ShowButtonsRoutine()
    {
        this.GetComponent<Image>().CrossFadeAlpha(0f, fadeOutDuration, false);
        yield return new WaitForSeconds(fadeOutDuration);

        foreach (var item in buttons)
        {
            item.transform.DOBlendableMoveBy(Vector3.down*46, buttonMoveDuration);
        }

        yield return new WaitForSeconds(buttonMoveDuration);
        foreach (var item in buttons)
        {
            item.GetComponent<ButtonAnimator>().SetOrigin();
        }
        controllerUI.EnableAllColliders();
        this.gameObject.SetActive(false);
    }
}
