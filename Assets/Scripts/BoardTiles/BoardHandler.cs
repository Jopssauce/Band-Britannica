using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoardEvent : UnityEvent { }

[System.Serializable]
public class TileMatchedEvent : UnityEvent<List<Tile>> { }

public class BoardHandler : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        RightUp,
        RightDown,
        LeftUp,
        LeftDown
    }

    public List<Tile> Tiles;

    public AttributeType CurrentSelectedTileType;
    public List<AttributeType> SelectedTileType = new List<AttributeType>();
    public int IntervalCount = 3;
    public float DestroyTimer = 0.1f;
    public bool IsClearBoard = false;

    [Header("Highlight Setting")]
    public Color HighlightCol = new Color(1, 1, 1, 0.75f);
    private Material selectedTileMat;

    [Header("Events")]
    public BoardEvent OnDestroyTiles;
    public BoardEvent OnFillTile;
    public BoardEvent OnAddTile;
    public BoardEvent OnRemoveTile;
    public BoardEvent OnDeselectTiles;
    public UnityEvent OnTileMatchedRelease;
    public BoardEvent OnEndBoardInteraction;
    public TileMatchedEvent OnStartBoardEffect;

    [Header("CameraShake Settings")]
    public float ShakeStrength = 3.0f;
    public float ShakeTimer = 0.2f;

    private Direction direction;
    private GameObject[,] Board;
    private BoardGenerator bGenerator;
    private CombatManager cManager;
    private AudioManager audioManager;

    private CameraShake camShake;
    private ActionMana actionMana;

    public List<Tile> tilesToManualMove = new List<Tile>();
    public int calledEvent = 0;
    private List<int> currentMatchCountList;
    private int offsetX = 0;
    private int currentRow = 0;
    private Coroutine destroyTilesCort;

    private void Awake()
    {
        SingletonManager.Register<BoardHandler>(this,SingletonType.Occasional);
    }

    private void Start()
    {
        //Get Component
        bGenerator = SingletonManager.Get<BoardGenerator>();
        cManager = SingletonManager.Get<CombatManager>();
        camShake = Camera.main.GetComponent<CameraShake>();
        actionMana = SingletonManager.Get<ActionMana>();
        audioManager = SingletonManager.Get<AudioManager>();

        //AddListener
        OnDestroyTiles.AddListener(StartDestroyMatchedTiles);
        bGenerator.OnBoardGenerateDone.AddListener(GetBoard);
        cManager.OnCurrentRoundEnd.AddListener(OnRoundEnded);
    }

    public void OnTileClick(Tile tile)
    {
        if(Tiles.Count == 0)
        {
            //Cache Tile Type
            CurrentSelectedTileType = tile.Type;
            SelectedTileType.Add(CurrentSelectedTileType);

            tile.GroupIndex = SelectedTileType.Count;
            AddTile(tile);
        }
    }

    public void OnTileConnect(Tile tile)
    {
        if (Tiles.Count == 0) return;

        //Deselect previous selected tiles
        if (Tiles.Contains(tile))
        {
            if(tile != Tiles.Last())
            {
                //Set to CurrentSelectedTileType
                CurrentSelectedTileType = tile.Type;
                int a = 0;

                if (tile.GroupIndex > (SelectedTileType.Count / 2))
                {
                    //Get Last one
                    a = SelectedTileType.FindLastIndex(item => item == CurrentSelectedTileType);
                }
                else
                {
                    //Get First one
                    a = SelectedTileType.FindIndex(item => item == CurrentSelectedTileType);
                }

                int count = SelectedTileType.Count;

                for (int i = count - 1; i > a; i--)
                {
                    SelectedTileType.RemoveAt(i);
                }

                //Remove the tile from the list and the tiles next to this
                int index = Tiles.IndexOf(tile);
                RemoveMatchedTiles(index);
            }
            return;
        }
        
        //All Matching Methods
        if (actionMana.CanConnectMore() == true)
        {
            DoConnectable(tile);
        }
    }

    public void OnTileDisconnect(Tile tile)
    {
        if (Tiles.Count == 0) return;

        //Maintain connected tile
        if (Tiles.Contains(tile)) return;

        if (tile.Type == CurrentSelectedTileType)
        {
            RemoveTile(tile);
        }
    }

    public void OnTileRelease(Tile tile)
    {
        if (Tiles.Count == 0) return;

        OnTileMatchedRelease.Invoke();
        OnMatchingDone(Tiles);
    }

    void DoConnectable(Tile tile)
    {
        DoubleInt coord = tile.Coord;
        DoubleInt passiveCoord = Tiles.Last().Coord;

        if (CheckIsSurround(passiveCoord, coord) == false || SelectedTileType.Count == 0) return;

        if (tile.Type != SelectedTileType[SelectedTileType.IndexOf(CurrentSelectedTileType)] && 
            Tiles.Count >= IntervalCount * SelectedTileType.Count && 
            CheckValidToNextTileType())
        {
            CurrentSelectedTileType = tile.Type;
            SelectedTileType.Add(CurrentSelectedTileType);
        }

        if(tile.Type == CurrentSelectedTileType)
        {
            tile.GroupIndex = SelectedTileType.Count;
            AddTile(tile);
        }
    }
    
    bool CheckIsSurround(DoubleInt passiveCoord, DoubleInt coord)
    {
        if (passiveCoord.x - 1 == coord.x && passiveCoord.y == coord.y ||
            passiveCoord.x + 1 == coord.x && passiveCoord.y == coord.y ||
            passiveCoord.y + 1 == coord.y && passiveCoord.x == coord.x ||
            passiveCoord.y - 1 == coord.y && passiveCoord.x == coord.x ||
            passiveCoord.x - 1 == coord.x && passiveCoord.y - 1 == coord.y ||
            passiveCoord.x + 1 == coord.x && passiveCoord.y - 1 == coord.y ||
            passiveCoord.x - 1 == coord.x && passiveCoord.y + 1 == coord.y ||
            passiveCoord.x + 1 == coord.x && passiveCoord.y + 1 == coord.y) return true;
        else return false;
    }

    bool CheckValidToNextTileType()
    {
        int group = 0;

        foreach (var type in SelectedTileType)
        {
            int count = 0;
            group++;

            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i].Type == type && Tiles[i].GroupIndex == group)
                {
                    count++;
                }
            }

            if (count < IntervalCount) return false;
        }

        return true;
    }

    void OnMatchingDone(List<Tile> tiles)
    {
        int count = tiles.Count;
        DoubleInt max = bGenerator.Grid;
        currentMatchCountList = new List<int>();

        cManager.ChangeCombatState(CombatState.MatchClearing);

        //Won't Affect Tiles
        OnStartBoardEffect.Invoke(tiles);

        //Get How much tiles need to be fill for every row
        for (int y = 0; y < max.y; y++)
        {
            currentMatchCountList.Add(0);
        }

        foreach (var item in tiles)
        {
            ClearMatchedTiles(item.Coord.x, item.Coord.y);
        }

        OnDestroyTiles.Invoke();

        //Camera Shake
        camShake.CameraShakeByInput(ShakeStrength, ShakeTimer);
    }

    void ClearMatchedTiles(int x, int y)
    {
        int column = x;
        currentRow = y;

        int xMax = bGenerator.Grid.x;

        if (column + 1 != xMax)
        {
            for (int c = column + 1; c < xMax; c++)
            {
                SwapTile(c - 1, c, currentRow);

                Tile tile = Board[c - 1, currentRow].GetComponent<Tile>();
                if(tilesToManualMove.Contains(tile) == false) tilesToManualMove.Add(tile);
            }
        }
    }

    void SwapTile(int x1, int x2, int y)
    {
        //Swap Gameobject
        GameObject temp = Board[x1, y];
        Board[x1, y] = Board[x2, y];
        Board[x2, y] = temp;

        //Get Tile Component
        Tile activeT = Board[x1, y].GetComponent<Tile>();
        Tile passiveT = Board[x2, y].GetComponent<Tile>();

        //Swap Main Location
        RectTransform activeRT = Board[x1, y].GetComponent<RectTransform>();
        RectTransform passiveRT = Board[x2, y].GetComponent<RectTransform>();
    
        //Swap Coords
        activeT.Coord.x = x1;
        passiveT.Coord.x = x2;
    }

    void StartDestroyMatchedTiles()
    {
        destroyTilesCort = StartCoroutine(DestroyTiles());
    }

    IEnumerator DestroyTiles()
    {
        List<DoubleInt> tilesCoords = new List<DoubleInt>();
        List<Tile> tiles = new List<Tile>();
        foreach (var tile in Tiles)
        {
            tiles.Add(tile);
        }

        foreach (var tile in tiles)
        {
            tilesCoords.Add(new DoubleInt(tile.Coord.x, tile.Coord.y));
            tilesToManualMove.Remove(tile);
            tile.Destroy();
            yield return new WaitForSeconds(DestroyTimer);
        }

        SpawnNewTilesAndRelocate(tilesCoords);
        if(destroyTilesCort != null) StopCoroutine(destroyTilesCort);
    }

    void SpawnNewTilesAndRelocate(List<DoubleInt> tilesCoords)
    {
        DoubleInt grid = bGenerator.Grid;

        for (int y = 0; y < grid.y; y++)
        {
            foreach (var coord in tilesCoords)
            {
                offsetX = currentMatchCountList[y];

                if (coord.y == y)
                {
                    GameObject tileGO;
                    if (cManager.IsTutorial)
                    {
                        tileGO = bGenerator.CreatePresetTile(grid.x + offsetX, coord.x, y);
                    }
                    else tileGO = bGenerator.CreateTile(grid.x + offsetX, y);

                    Tile tile = tileGO.GetComponent<Tile>();
                    tile.Coord.x = coord.x;
                    Board[coord.x, coord.y] = tileGO;

                    tilesToManualMove.Add(tile);

                    currentMatchCountList[y]++;
                }
            }
        }

        MoveAllTilesToItsDesiredLocation();
    }

    void AddTileByCoord(int coordX, int coordY)
    {
        //Get Tile Component
        Tile currentTile = Board[coordX, coordY].GetComponent<Tile>();

        //Condition: Must be same type and is not yet in the list
        if (Tiles.Contains(currentTile) == false)
        {
            //Add to the list
            AddTile(currentTile);
        }
    }

    void AddTile(Tile tile)
    {
        //Change Color to SelectedColor
        tile.SetMaterialColor(HighlightCol);

        //Off Tile Animation
        TileAnimation tileAnim = tile.GetComponent<TileAnimation>();
        if(tileAnim != null) tileAnim.EnableFloating = false;

        if (audioManager) audioManager.PlaySFX("Tile_Select");
        Tiles.Add(tile);

        OnAddTile.Invoke();
    }

    void RemoveTile(Tile tile)
    {
        //Change Color to Original Color
        tile.SetMaterialColor(tile.AttributeColor);
        tile.GroupIndex = 0;

        Tiles.Remove(tile);

        OnRemoveTile.Invoke();
    }

    /// <summary>
    /// Exclude the index passed
    /// </summary>
    /// <param name="index"></param>
    void RemoveMatchedTiles(int index)
    {
        int count = Tiles.Count;

        for (int i = count - 1; i > index; i--)
        {
            RemoveTile(Tiles[i]);
        }
    }

    void GetBoard(GameObject[,] board)
    {
        Board = board;
    }

    public void DeseletSelectedTiles()
    {
        int count = Tiles.Count;

        for (int i = 0; i < count; i++)
        {
            RemoveTile(Tiles.First());
        }

        SelectedTileType.Clear();

        OnDeselectTiles.Invoke();

        OnTileRelease(null);
    }

    public void OnRoundEnded()
    {
        //After Clearing, Empty the list of selected tiles
        Tiles.Clear();
        SelectedTileType.Clear();
    }

    void MoveAllTilesToItsDesiredLocation()
    {
        foreach (var tile in tilesToManualMove)
        {
            tile.MoveToAnotherPosition(tile.GetDerivedRectTransformLocalPosition());
            tile.OnReachLocation.AddListener(ReachItsLocation);
        }
    }

    /// <summary>
    /// Listen to Tiles that need to relocation its location
    /// </summary>
    /// <param name="tile"></param>
    void ReachItsLocation(Tile tile)
    {
        tile.OnReachLocation.RemoveAllListeners();

        calledEvent++;

        if(calledEvent == tilesToManualMove.Count)
        {
            tilesToManualMove.Clear();
            calledEvent = 0;

            if (IsClearBoard == true)
            {
                OnRoundEnded();
                cManager.OnCurrentRoundEnd.AddListener(TurnClearBoardOff);
            }

            //Start Combat
            OnEndBoardInteraction.Invoke();
        }
    }

    /// <summary>
    /// Select all tiles on the board
    /// </summary>
    public void SelectAllTiles()
    {
        //Clear List First
        OnRoundEnded();

        for (int y = 0; y < bGenerator.Grid.y; y++)
        {
            for (int x = 0; x < bGenerator.Grid.x; x++)
            {
                Tiles.Add(bGenerator.Board[x, y].GetComponent<Tile>());
            }
        }
    }

    void TurnClearBoardOff()
    {
        cManager.OnCurrentRoundEnd.RemoveListener(TurnClearBoardOff);

        IsClearBoard = false;
    }
}
