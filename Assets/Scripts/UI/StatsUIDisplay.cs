using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUIDisplay : MonoBehaviour
{
    public float height = 1.5f;
    public Image healthBar;
    public Image turnBG;
    public TextMeshProUGUI turn;

    public GameObject alertParticle;

    public Character character;
    public float animationTime;

    protected float desiredValue;
    protected float initialValue;
    protected float currentValue;

    private void Start()
    {
        //character = transform.parent.GetComponent<Character>();

        if (character == null)
        {
            Debug.LogWarning("INVALID. Destination moved.");
            return;
        }
        character.OnDestroy.AddListener(Destroy);
    }

    void Update()
    {
        UpdateStatsBar();
        SetAlertParticleState();
        transform.LookAt(Camera.main.transform);

        transform.position = character.transform.position + Vector3.up * height;
    }

    void UpdateStatsBar()
    {
        AnimateHealthFill();
        turn.text = character.CurrentTurns.ToString();
    }

    void SetAlertParticleState()
    {
        if (alertParticle == null)
        {
            return;
        }

        if (character.CurrentTurns <= 1)
        {
            alertParticle.SetActive(true);
        }

        if (character.CurrentTurns >= 0 && character.CurrentTurns != 1)
        {
            alertParticle.SetActive(false);
        }

    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void AnimateHealthFill()
    {
        initialValue = currentValue;
        desiredValue = character.Health;

        if(currentValue != desiredValue)
        {
            if(initialValue < desiredValue)
            {
                currentValue += (animationTime * Time.deltaTime) * (desiredValue - initialValue);
                if(currentValue >= desiredValue)
                {
                    currentValue = desiredValue;
                }
            }
            else
            {
                currentValue -= (animationTime * Time.deltaTime) * (initialValue - desiredValue);
                if(currentValue <= desiredValue)
                {
                    currentValue = desiredValue;
                }
            }
            healthBar.fillAmount = currentValue/character.maxHealth;
        }
    }
}