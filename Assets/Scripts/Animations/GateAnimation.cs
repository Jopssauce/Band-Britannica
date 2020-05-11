using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GateAnimation : MonoBehaviour
{
    public bool enableMoveAnimation;
    public string sceneToLoad;
    public GameObject leftGate;
    public GameObject rightGate;
    public float gateSpeed;
    public float lookAtGateSpeed;
    public float lookAtGateDuration;
    public Transform[] travelPoints;
    public float travelSpeed;

    public Camera mainCamera;

    public List<Vector3> points;

    protected bool look;

    void Start()
    {
        for (int i = 0; i < travelPoints.Length; i++)
        {
            points.Add(travelPoints[i].position);
        }
    }

    void Update()
    {

    }

    public void StartGateAnimation()
    {
        StartCoroutine(AnimationRoutine());
    }

    public IEnumerator AnimationRoutine()
    {
        mainCamera.GetComponent<TitleSceneCamera>().enabled = false;
        Sequence s = DOTween.Sequence();
        s.Append(leftGate.transform.DOBlendableRotateBy(new Vector3(0,90,0), gateSpeed));
        s.Insert(0, rightGate.transform.DOBlendableRotateBy(new Vector3(0,-90,0), gateSpeed));
        yield return new WaitForSeconds(gateSpeed);

        if(enableMoveAnimation)
        {
            s.Append(mainCamera.transform.DOPath(points.ToArray(), travelSpeed, PathType.CatmullRom, PathMode.Sidescroller2D, 5, null).OnUpdate(LookTo));
            yield return new WaitForSeconds(travelSpeed);
            SceneController.instance.LoadScene(sceneToLoad);
        }
        else SceneController.instance.LoadScene(sceneToLoad);
    }

    void LookTo()
    {
        mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, Quaternion.Euler(1,35,0) , lookAtGateSpeed*Time.deltaTime);
    }
}
