using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TeamAttackDisplay : TextAndFillDisplay, IPointerEnterHandler, IPointerExitHandler
{
    protected TeamAttack teamAttack;

    [Header("Animation Variables")]
    public GameObject particleGlow;
    public GameObject tileParticle;
    public float particleTravelDuration;
    
    protected List<Vector3> points = new List<Vector3>();
    protected GameObject loopingInstance;

    void Start()
    {
        board = SingletonManager.Get<BoardHandler>();
        combat = SingletonManager.Get<CombatManager>();
        teamAttack = SingletonManager.Get<TeamAttack>();

        teamAttack.OnTeamAttackFull.AddListener(PlayLoopGlow);
        teamAttack.OnTeamAttackDone.AddListener(StopLoopGlow);
        teamAttack.OnTeamAttackDone.AddListener(ResetDisplayValue);
        teamAttack.OnTeamAttackValueAdded.AddListener(AddDisplayValue);

        desiredValue = teamAttack.CurrentTeamAttackAmount;
        resetToValue = teamAttack.MinTeamAttackAmount;
        fillNormalizer = teamAttack.MaxTeamAttackAmount;

        SetValueToUI();
        if (textDisplay != null) SetGameobjectActive(textDisplay.gameObject, false);

        points.Add(this.transform.position-(Vector3.down*-25));
        points.Add(points[0]-Vector3.left*4);
        points.Add(points[0]+Vector3.left*4);
    }

    public override void AddDisplayValue()
    {
        //only play glow and tile particle effect when there is value to add to team attack
        if(teamAttack.GetToBeAddAmount() > teamAttack.MinTeamAttackAmount)
        {
            PlayGlow();
            PlayTileParticle();
        }
        
        initialValue = currentValue;

        desiredValue = teamAttack.CurrentTeamAttackAmount;
        desiredValue = Mathf.Clamp(desiredValue, teamAttack.MinTeamAttackAmount, teamAttack.MaxTeamAttackAmount);
    }

    protected override void SetValueToUI()
    {
        base.SetValueToUI();

        if (textDisplay != null) textDisplay.text = currentValue.ToString("0") + " / " + fillNormalizer;
    }

    public void PlayGlow()
    {
        GameObject particleInstance = Instantiate(particleGlow, this.transform.position+(Vector3.up*0.5f), particleGlow.transform.rotation, this.transform);
    }
    public void PlayLoopGlow()
    {
        loopingInstance = Instantiate(particleGlow, this.transform.position+(Vector3.up*0.5f), particleGlow.transform.rotation, this.transform);
        var main = loopingInstance.GetComponent<ParticleSystem>().main;
        main.loop = true;
    }
    public void StopLoopGlow()
    {
        var main = loopingInstance.GetComponent<ParticleSystem>().main;
        main.loop = false;
    }
    public void PlayTileParticle()
    {
        for (int i = 0; i < points.Count; i++)
        {
            GameObject particleInstance = Instantiate(tileParticle, points[i], tileParticle.transform.rotation, this.transform);

            particleInstance.transform.DOMove( this.transform.position , particleTravelDuration);
            Destroy(particleInstance, particleTravelDuration);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetGameobjectActive(textDisplay.gameObject, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetGameobjectActive(textDisplay.gameObject, false);
    }

    void SetGameobjectActive(GameObject gameObject, bool condition)
    {
        gameObject.SetActive(condition);
    }
}