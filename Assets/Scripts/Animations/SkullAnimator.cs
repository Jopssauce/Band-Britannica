using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkullAnimator : MonoBehaviour
{
    public float moveDownDuration;
    public float rotateSpeed;
    public float floatSpeed;

    public Vector3 point;
    
    void Start()
    {
        StartCoroutine(MoveDownRoutine());    
    }

    public IEnumerator MoveDownRoutine()
    {
        this.transform.DOMove(point ,moveDownDuration);

        yield return new WaitForSeconds(moveDownDuration);

        StartCoroutine(RotateRoutine());
        StartCoroutine(FloatRoutine());
    }

    public IEnumerator RotateRoutine()
    {
        Sequence s = DOTween.Sequence();

        Vector3 rotate = new Vector3(0, 180, 0);
        s.Append(this.transform.DOBlendableRotateBy(rotate, rotateSpeed));
        s.Append(this.transform.DOBlendableRotateBy(rotate, rotateSpeed));
        s.Append(this.transform.DOBlendableRotateBy(rotate, rotateSpeed));
        s.Append(this.transform.DOBlendableRotateBy(rotate, rotateSpeed));

        s.SetLoops(-1, LoopType.Restart);

        yield return null;
    }

    public IEnumerator FloatRoutine()
    {
        Sequence s = DOTween.Sequence();

        s.Append(this.transform.DOMoveY(this.transform.position.y + Vector3.up.y, floatSpeed));
        s.SetLoops(-1, LoopType.Yoyo);

        yield return null;
    }
}
