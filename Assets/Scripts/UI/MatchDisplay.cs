using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchDisplay : MonoBehaviour
{
    public Text actor;
    public Text skillName;
    public Image[] tiles;
    public Text skillIntensity;

    protected int counter = 0;

    public void UpdateText(Character actor, Skill skill, int intensity)
    {
        this.actor.text = actor.CharacterName;
        this.skillName.text = skill.skillName;
        this.skillIntensity.text = intensity.ToString();
    }

    public void AddTile (Tile tile)
    {
        if(tile.Type == AttributeType.Water) tiles[counter].color = new Color32(0, 0,255,255);
        if(tile.Type == AttributeType.Fire) tiles[counter].color = new Color32(255, 0,0,255);
        if(tile.Type == AttributeType.Air) tiles[counter].color = new Color32(0, 255, 0,255);
        if(tile.Type == AttributeType.Earth) tiles[counter].color = new Color32(255,192,203,255);
        
        counter++;
    }

    public void RemoveTile()
    {
        tiles[counter].color = new Color32(0, 0, 0, 255);

        counter--;
    }
}
