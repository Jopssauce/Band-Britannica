using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipTutorialPanelBehavior : MonoBehaviour
{
    public List<ButtonAnimator> _ButtonAnimators;

    private void Start()
    {
        foreach (var confirmButton in _ButtonAnimators)
        {
            confirmButton.onClick.AddListener(SetHidden);
        }
    }

    void SetHidden()
    {
        gameObject.SetActive(false);
    }
}