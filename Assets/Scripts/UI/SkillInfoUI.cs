using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoUI : MonoBehaviour
{
    [HideInInspector]
    public GameObject InfoPanel;

    public Text SkillNameText;
    public Text SkillDescriptionText;

    private void Awake()
    {
        SingletonManager.Register<SkillInfoUI>(this, SingletonType.Occasional);

        InfoPanel = this.gameObject;
    }
}
