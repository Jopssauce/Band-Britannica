using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchVisualEffect : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private RectTransform rectTransform;

    private BoardHandler boardHandler;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();
        boardHandler = SingletonManager.Get<BoardHandler>();

        //Add Listener
        boardHandler.OnAddTile.AddListener(UpdateVisualLines);
        boardHandler.OnRemoveTile.AddListener(UpdateVisualLines);
        boardHandler.OnStartBoardEffect.AddListener(ClearVisualLines);
    }

    void UpdateVisualLines()
    {
        List<Tile> tiles = boardHandler.Tiles;

        lineRenderer.positionCount = tiles.Count;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null)
            {
                lineRenderer.positionCount = 0;
                return;
            }
            else
            {
                Vector3 position = tiles[i].GetComponent<RectTransform>().position;

                lineRenderer.SetPosition(i, position);
            }
        }
    }

    void ClearVisualLines(List<Tile> tiles)
    {
        lineRenderer.positionCount = 0;
    }
}
