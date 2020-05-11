using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DisplaySkillInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Skill _Skill;

    [Header("Local Skill Variable for Display")]
    public Text SkillNameText;
    public Vector2 ResizeValue = new Vector2(19.0f, 19.0f);

    public Transform ElementParent;

    protected Animator animator;
    protected BoardHandler board;
    protected SkillInfoUI skillInfo;
    protected SkillListHandler skillListHandler;
    protected CombatManager combatManager;

    void Start()
    {
        board = SingletonManager.Get<BoardHandler>();
        skillInfo = SingletonManager.Get<SkillInfoUI>();
        skillListHandler = SingletonManager.Get<SkillListHandler>();
        combatManager = SingletonManager.Get<CombatManager>();

        animator = skillInfo.InfoPanel.GetComponent<Animator>();

        //Load Skill Info
        LoadSkillInfo();
    }
    
    void LoadSkillInfo()
    {
        if (_Skill == null) return;
        SkillNameText.text = _Skill.skillName;

        for (int i = 0; i < 2; i++)
        {
            if (_Skill.SkillRequirements[0] == _Skill.SkillRequirements[1])
            {
                transform.Find("Elements").GetComponent<RectTransform>().anchoredPosition += new Vector2(30, 0);
                break;
            }

            GameObject displayTile = skillListHandler.TileImageList.Find(a => a.Type == _Skill.SkillRequirements[0]).gameObject;

            if (displayTile != null)
            {
                GameObject obj = Instantiate(displayTile, ElementParent);
                RectTransform rt = obj.GetComponent<RectTransform>();

                rt.sizeDelta = ResizeValue;
            }
        }

        foreach (var item in _Skill.SkillRequirements)
        {
            GameObject displayTile = skillListHandler.TileImageList.Find(a => a.Type == item).gameObject;

            if (displayTile != null)
            {
                GameObject obj = Instantiate(displayTile, ElementParent);
                RectTransform rt = obj.GetComponent<RectTransform>();

                rt.sizeDelta = ResizeValue;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (board.Tiles.Count > 0 || _Skill == null || combatManager.CurCombatState != CombatState.Match3Turn) return;
        
        animator.SetBool("open", true);
        skillInfo.SkillNameText.text = _Skill.skillName;
        skillInfo.SkillDescriptionText.text = _Skill.Description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_Skill == null) return;

        animator.SetBool("open", false);
    }
}