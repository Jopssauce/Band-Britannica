using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAndFillDisplay : MonoBehaviour
{
    public Text textDisplay;
    public float animationTime;
    public Image fillDisplay;

    //For normalizing the values
    public float fillNormalizer;
    public float resetToValue;

    protected float desiredValue;
    protected float initialValue;
    protected float currentValue;

    protected BoardHandler board;
    protected CombatManager combat;

    void Update()
    {
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

            SetValueToUI();
        }
    }

    protected virtual void SetValueToUI()
    {
        if (textDisplay != null) textDisplay.text = currentValue.ToString("0");
        if (fillDisplay != null) fillDisplay.fillAmount = currentValue / fillNormalizer;
    }

    public virtual void AddDisplayValue()
    {
        initialValue = currentValue;
        desiredValue += 1;
    }

    public virtual void SubtractDisplayValue()
    {
        initialValue = currentValue;
        desiredValue -= 1;
    }

    public virtual void ResetDisplayValue()
    {
        initialValue = currentValue;
        desiredValue = resetToValue;
    }
}
