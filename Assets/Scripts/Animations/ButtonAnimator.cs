using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ButtonAnimator : MonoBehaviour
{
    protected float desiredValue;
    protected float initialValue;
    protected float currentValue;

    public GameObject buttonGraphic;

    [Header("Animation Variables")]
    public float highlightTime;
    public Vector3 clickPunchAmount;
    public float onClickTime;
    public float hoverShakeAmount;
    public float hoverShakeTime;
    public float clickShakeAmount;
    public float clickShakeTime;

    [Header("More Animation Stuff")]
    public bool enableHobverHighlight;
    public bool enableHoverShake;
    public bool enableClickHighlight;

    [Header("Event Assignment")]
    public UnityEvent onClick;

    [HideInInspector]
    public Vector3 originPosition;
    [HideInInspector]
    public Vector3 originRotation;

    protected bool overButton;
    private AudioManager audioManager;
    protected MapNodeClick mapNode;

    void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();

        originPosition = this.transform.position;
        originRotation = this.transform.rotation.eulerAngles;
        if(this.GetComponent<MapNodeClick>())
        { 
            mapNode = this.GetComponent<MapNodeClick>();
            CheckForUnlock();
        }
    }

    void Update()
    {
        if(currentValue != desiredValue)
        {
            if(initialValue < desiredValue)
            {
                currentValue += (highlightTime* Time.deltaTime) * (desiredValue - initialValue);
                if(currentValue >= desiredValue)
                {
                    currentValue = desiredValue;
                }
            }
            else
            {
                currentValue -= (highlightTime * Time.deltaTime) * (initialValue - desiredValue);
                if(currentValue <= desiredValue)
                {
                    currentValue = desiredValue;
                }
            }
            if(enableHobverHighlight) buttonGraphic.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", currentValue);
        }

        //if(!mapNode.info.unlocked) this.GetComponent<BoxCollider>().enabled = false;
        //else this.GetComponent<BoxCollider>().enabled = true;
    }

    public void OnMouseDown()
    {
       if(enableClickHighlight) buttonGraphic.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", 1);
       this.transform.position = (originPosition+clickPunchAmount);

       Vector3 punchValue = new Vector3(this.transform.rotation.x + clickShakeAmount, this.transform.rotation.y, this.transform.rotation.z);
       this.transform.DOPunchRotation(punchValue, clickShakeTime);
    }

    public void OnMouseUp()
    {
        if(enableClickHighlight) buttonGraphic.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", 0);
        this.transform.position = originPosition;
        this.transform.DORotate(originRotation, 0);
        if(audioManager) audioManager.PlaySFX("Button_Click");
        
        if(overButton) onClick.Invoke();
    }

    void OnMouseEnter()
    {
        if(enableHobverHighlight)
        {
            initialValue = currentValue;
            desiredValue = 0.3f;
        }
        
        Vector3 punchValue = new Vector3(this.transform.rotation.x + hoverShakeAmount, this.transform.rotation.y, this.transform.rotation.z);
        this.transform.DOPunchRotation(punchValue, hoverShakeTime).OnComplete(ReturnOriginalRotation);
    }

    void OnMouseExit()
    {
        if(enableHobverHighlight)
        {
            initialValue = currentValue;
            desiredValue = 0f;
        }
        overButton = false;
    }
    void OnMouseOver()
    {
        overButton = true;
    }

    void ReturnOriginalRotation()
    {
        this.transform.DORotate(originRotation, 0);
    }

    void CheckForUnlock()
    {
        if(!mapNode.info.unlocked) this.GetComponent<BoxCollider>().enabled = false;
        else this.GetComponent<BoxCollider>().enabled = true;
    }

    public void SetOrigin()
    {
        originPosition = this.transform.position;
        originRotation = this.transform.rotation.eulerAngles;
    }
}