using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public enum Direction
{
    Up = 0,
    Down = 180,
    Left = 90,
    Right = 270
}

public class PointerAnim : MonoBehaviour
{
    [Header("Movement Variables")]
    public RectTransform[] points;
    public RectTransform self;

    public float animationTime;

    [Header("Rotation Variables")]
    public int[] indexesToRotate;
    public Direction[] directionToRotate;

    protected int moveCounter = 0;
    protected int rotCounter = 0;
    protected Vector3 originPos;
    protected Quaternion originRot;

    // Start is called before the first frame update
    private void Awake()
    {
        originPos = self.localPosition;
        originRot = self.localRotation;
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetAnimation();
        StartMoveRoutine();
    }

    public IEnumerator MoveRoutine()
    {
        while (true)
        {
            self.DOAnchorPos(points[moveCounter].localPosition, animationTime);
            moveCounter++;
            
            yield return new WaitForSeconds(animationTime);

            if (CheckToRotate())
            {
                self.DORotateQuaternion(Quaternion.Euler(self.localRotation.x, self.localRotation.y, (float)directionToRotate[rotCounter]), 0.5f);
                rotCounter++;
            }

            if (moveCounter == (points.Length))
            {
                ResetAnimation();

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public bool CheckToRotate()
    {
        if (rotCounter >= indexesToRotate.Length) return false;

        if (moveCounter == indexesToRotate[rotCounter])
        {
            return true;
        }

        else return false;
    }

    public void ResetAnimation()
    {
        self.anchoredPosition = originPos;
        self.localRotation = originRot;
        moveCounter = 0;
        rotCounter = 0;
    }

    public void StartMoveRoutine()
    {
        StartCoroutine(MoveRoutine());
    }
}