using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DoubleInt
{
    public int x;
    public int y;

    public DoubleInt(int column, int row)
    {
        x = column;
        y = row;
    }
}

[System.Serializable]
public struct CustomTileGenerator
{
    /// <summary>
    /// These coords with be the location to preset
    /// </summary>
    public List<DoubleInt> Coords;
    /// <summary>
    /// Specific Tile Prefab
    /// </summary>
    public GameObject TilePrefab;
}

[System.Serializable]
public class BoardGenerateEvent : UnityEvent<GameObject[,]> { }

public class BoardGenerator : MonoBehaviour
{
    [Header("Tutorial")]
    public List<CustomTileGenerator> PresetTiles;

    public List<GameObject> TilePrefabs;
    public Transform ParentTransform;

    public DoubleInt Grid;
    public Vector2 MinimumPosition;
    public Vector2 Spacing;
    public float OffsetZ = -10;
    public GameObject[,] Board;

    public BoardGenerateEvent OnBoardGenerateDone;

    private void Awake()
    {
        SingletonManager.Register<BoardGenerator>(this,SingletonType.Occasional);
    }

    private void Start()
    {
        //Initialize
        Board = new GameObject[Grid.x,Grid.y];

        if (SingletonManager.Get<CombatManager>().IsTutorial)
        {
            StartCoroutine(GeneratePresetTiles());
        }
        else StartCoroutine(GenerateTiles());
    }

    IEnumerator GenerateTiles()
    {
        for (int y = 0; y < Grid.y; y++)
        {
            for (int x = 0; x < Grid.x; x++)
            {
                CreateTile(x, y);
            }
        }

        yield return new WaitUntil(() => SingletonManager.Get<BoardHandler>());

        OnBoardGenerateDone.Invoke(Board);
    }

    IEnumerator GeneratePresetTiles()
    {
        foreach (var a in PresetTiles)
        {
            foreach (var b in a.Coords)
            {
                CreatePresetTile(b.x, b.x, b.y);
            }
        }

        yield return new WaitUntil(() => SingletonManager.Get<BoardHandler>());

        OnBoardGenerateDone.Invoke(Board);
    }

    public GameObject CreateTile(int x, int y)
    {
        DoubleInt coord;
        coord.x = x;
        coord.y = y;

        int randnum = Random.Range(0, TilePrefabs.Count);

        GameObject tile = Instantiate(TilePrefabs[randnum], ParentTransform) as GameObject;
        Tile tileComp = tile.GetComponent<Tile>();
        RectTransform rectTransform = tile.GetComponent<RectTransform>();

        //Temporary X and Y
        tileComp.SetCoord(coord.x, coord.y);

        Vector3 localPos = tileComp.GetDerivedRectTransformLocalPosition();
        
        //Initialize Local Position
        rectTransform.localPosition = localPos;

        HandleOutOfRange(ref coord.x, Grid.x);

        //Updated X and Y
        tileComp.SetCoord(coord.x, coord.y);
        localPos = tileComp.GetDerivedRectTransformLocalPosition();

        //Initialize Positions
        tileComp.InitialLocation = localPos;
        tileComp.NewLocation = localPos;

        //Assign to the Board
        Board[coord.x, coord.y] = tile;

        return tile;
    }

    public GameObject CreatePresetTile(int Offsetx, int x, int y)
    {
        DoubleInt coord;
        coord.x = Offsetx;
        coord.y = y;

        GameObject tile = Instantiate(GetPresetTilePrefab(x, y), ParentTransform) as GameObject;
        Tile tileComp = tile.GetComponent<Tile>();
        RectTransform rectTransform = tile.GetComponent<RectTransform>();

        //Temporary X and Y
        tileComp.SetCoord(coord.x, coord.y);

        Vector3 localPos = tileComp.GetDerivedRectTransformLocalPosition();

        //Initialize Local Position
        rectTransform.localPosition = localPos;

        HandleOutOfRange(ref coord.x, Grid.x);

        //Updated X and Y
        tileComp.SetCoord(coord.x, coord.y);
        localPos = tileComp.GetDerivedRectTransformLocalPosition();

        //Initialize Positions
        tileComp.InitialLocation = localPos;
        tileComp.NewLocation = localPos;

        //Assign to the Board
        Board[coord.x, coord.y] = tile;

        return tile;
    }

    void HandleOutOfRange(ref int value, int max)
    {
        if (value >= max)
        {
            value = max - 1;
        }
    }

    GameObject GetPresetTilePrefab(int x, int y)
    {
        foreach (var type in PresetTiles)
        {
            foreach (var i in type.Coords)
            {
                if(i.x == x && i.y == y)
                {
                    return type.TilePrefab;
                }
            }
        }
        Debug.LogError("There is no ideal coord. Please double check");
        return null;
    }
}