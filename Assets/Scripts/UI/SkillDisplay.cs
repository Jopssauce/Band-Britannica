using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkillDisplay : MonoBehaviour
{
    public GameObject skillDisplayPrefab;
    public GameObject panelBG;
    public GameObject[] tileElements;
    public GameObject tileList;

    public List<GameObject> panelInstances;
    public List<GameObject> tileInstances;

    protected CombatSkills combatSkills;
    protected CombatManager combatManager;
    protected BoardHandler boardHandler;

    void Start()
    {
        combatSkills = SingletonManager.Get<CombatSkills>();
        boardHandler = SingletonManager.Get<BoardHandler>();
        combatManager = SingletonManager.Get<CombatManager>();

        combatSkills.OnCastableSkillsUpdated.AddListener(UpdatePanels);
        boardHandler.OnDeselectTiles.AddListener(ResetDisplay);
        boardHandler.OnEndBoardInteraction.AddListener(ResetDisplay);
    }

    public void UpdatePanels()
    {
        if(panelInstances.Count == combatSkills.CastableSkills.Count)
        {
            SetPanelInfo();
        }
        else if(panelInstances.Count < combatSkills.CastableSkills.Count)
        {
            if(!panelBG.activeSelf) panelBG.SetActive(true);
            //Add
            AddPanel();
        }
        else if(panelInstances.Count > combatSkills.CastableSkills.Count)
        {
            //Remove
            RemovePanel();
            if(combatSkills.CastableSkills.Count == 0) panelBG.SetActive(false);
        }
    }

    public void AddPanel()
    {
        int length = combatSkills.CastableSkills.Count - panelInstances.Count;
        for (int i = 0; i < length; i++)
        {
            GameObject prefabInstance = Instantiate(skillDisplayPrefab, panelBG.transform);
            panelInstances.Add(prefabInstance);
        }

        SetPanelInfo();
    }

    public void RemovePanel()
    {
        //Very crude fix I know -Rivera.
        while (true)
        {
            int index = combatSkills.CastableSkills.Count;
            Destroy(panelInstances[index]);
            panelInstances.RemoveAt(index);
            if(panelInstances.Count == combatSkills.CastableSkills.Count) break;
        }

        if(combatSkills.CastableSkills.Count != 0) SetPanelInfo();

    }

    public void SetPanelInfo()
    {
        for (int i = 0; i < combatSkills.CastableSkills.Count; i++)
        {
            panelInstances[i].GetComponent<CastSkillTextDisplay>().actor.text = combatSkills.CastableSkills[i].actor.GetComponent<Character>().CharacterName;
            panelInstances[i].GetComponent<CastSkillTextDisplay>().skillName.text = combatSkills.CastableSkills[i].skillName;
            //panelInstances[i].GetComponent<CastSkillTextDisplay>().intensity.text = "Lv" + combatSkills.DisplaySkills[i].IntensityLevel.ToString();

            if(combatSkills.CastableSkills[i].actor.GetComponent<Character>().characterHighlight != null) combatSkills.CastableSkills[i].actor.GetComponent<Character>().characterHighlight.Play();
        }
    }

    public void AddTile(Tile tile)
    {
        int num = 0;
        if(tile.Type == AttributeType.Fire) num = 0;
        else if(tile.Type == AttributeType.Water) num = 1;
        else if(tile.Type == AttributeType.Air) num = 2;
        else if(tile.Type == AttributeType.Earth) num = 3;
        GameObject prefabInstance = Instantiate(tileElements[num], tileList.transform);

        tileInstances.Add(prefabInstance);
    }

    public void RemoveTile()
    {
        int num = tileInstances.Count-1;
        if(tileInstances.Count > 0)
        {
            Destroy(tileInstances[num].gameObject);
            tileInstances.RemoveAt(num);
        }
        
    }

    public void ResetDisplay()
    {
        foreach (Character item in combatManager.AllyTeam)
        {
            if(item.characterHighlight != null) item.characterHighlight.Stop();
        }

        if(panelInstances.Count > 0)
        foreach (GameObject item in panelInstances)
        {
            Destroy(item);
        }

        if(tileInstances.Count > 0)
        foreach (GameObject item in tileInstances)
        {
            Destroy(item);
        }

        tileInstances.Clear();
        panelInstances.Clear();

        panelBG.SetActive(false);
    }

    private void Update()
    {
        if (panelInstances.Count == 0) return;

        for (int i = 0; i < panelInstances.Count; i++)
        {
            panelInstances[i].GetComponent<CastSkillTextDisplay>().intensity.text = "Lv" + combatSkills.DisplaySkills[i].IntensityLevel.ToString();
        }
    }
}
