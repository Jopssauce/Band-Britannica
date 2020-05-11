using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EndingTextAnimation : MonoBehaviour
{
    public float textScrollDuration;
    protected AudioManager audio;

    void Start()
    {
        StartCoroutine(Up());
        audio = SingletonManager.Get<AudioManager>();

        audio.PlayBGM("BGM_TITLE");
        
    }
    
    IEnumerator Up()
    {
        Sequence s = DOTween.Sequence();

        s.Append(this.transform.DOMoveY(180f, textScrollDuration).SetEase(Ease.Linear));
        yield return new WaitForSeconds(textScrollDuration);
        ReturnToTitleScene();
    }

    public void ReturnToTitleScene()
    {
        PersistentDataManager.instance.stageStates[0].completed = false;
        for (int i = 1; i < PersistentDataManager.instance.stageStates.Count; i++)
        {
            PersistentDataManager.instance.stageStates[i].completed = false;
            PersistentDataManager.instance.stageStates[i].unlocked = false;
        }
        SceneController.instance.LoadScene("Title Scene");
    }
}
