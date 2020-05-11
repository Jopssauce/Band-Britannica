using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TileEvent : UnityEvent<Tile> { }

public enum AttributeType
{
    Air,
    Earth,
    Fire,
    Water,
}

public class Tile : MonoBehaviour
{
    const float MINIMUM_DIST = 0.01f;

    public DoubleInt Coord;
    public AttributeType Type;
    public Color AttributeColor = Color.white;
    public KeyCode DeselectKey = KeyCode.Mouse1;

    [Header("Particle System")]
    public GameObject particleSystem;

    [Header("Events")]
    public TileEvent OnSelectTile;
    public TileEvent OnConnectTile;
    public TileEvent OnDisconnectTile;
    public TileEvent OnDeselectTile;
    public TileEvent OnReachLocation;

    [Header("Which Group From The List")]
    public int GroupIndex = 0;

    [Header("Animation")]
    public float Speed = 8.0f;
    public Vector3 NewLocation;
    public Vector3 InitialLocation;

    BoardGenerator boardGenerator;
    BoardHandler boardHandler;
    BoxCollider bCollider;
    RectTransform rectTransform;
    SpriteRenderer spriteRenderer;

    CombatManager cManager;
    AudioManager audioManager;

    bool enableMatch3 = false;

    private void Start()
    {
        boardHandler = SingletonManager.Get<BoardHandler>();
        cManager = SingletonManager.Get<CombatManager>();
        bCollider = GetComponent<BoxCollider>();
        rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = SingletonManager.Get<AudioManager>();

        //AddListener
        OnSelectTile.AddListener(boardHandler.OnTileClick);
        OnConnectTile.AddListener(boardHandler.OnTileConnect);
        OnDisconnectTile.AddListener(boardHandler.OnTileDisconnect);
        OnDeselectTile.AddListener(boardHandler.OnTileRelease);
    }

    private void Update()
    {
        if (this == null) return;

        if(Input.GetKeyDown(DeselectKey) && cManager.CurCombatState == CombatState.Match3Turn)
        {
            boardHandler.DeseletSelectedTiles();
        }

        //Temporary Handle State
        if (enableMatch3 == false && cManager.CurCombatState == CombatState.Match3Turn)
        {
            enableMatch3 = true;

            SetBoxColliderActive(true);
        }
        else if (cManager.CurCombatState != CombatState.Match3Turn)
        {
            enableMatch3 = false;

            SetBoxColliderActive(false);
        }
    }

    private void OnMouseDown()
    {
        //Call Event
        OnSelectTile.Invoke(this);
    }

    private void OnMouseEnter()
    {
        //Conditions: 1. Must have tile selected 2
        if (boardHandler.Tiles.Count == 0) return;

        //Call Event
        OnConnectTile.Invoke(this);
    }

    private void OnMouseExit()
    {
        //Conditions: 1. Must have tile selected 2. this must not be the one selected already
        if (boardHandler.Tiles.Count == 0 || boardHandler.Tiles.Contains(this) == false || boardHandler.Tiles.First() == this) return;

        //Call Event
        OnDisconnectTile.Invoke(this);
    }

    private void OnMouseUp()
    {
        //Call Event
        OnDeselectTile.Invoke(this);
    }

    public void SetMaterialColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material.color = color;
        }
    }

    public Vector3 GetDerivedRectTransformLocalPosition()
    {
        //Handle Get BoardGenerator
        if (boardGenerator == null)
        {
            boardGenerator = SingletonManager.Get<BoardGenerator>();
        }

        Vector2 minimumPosition = boardGenerator.MinimumPosition;
        Vector2 Spacing = boardGenerator.Spacing;
        float OffsetZ = boardGenerator.OffsetZ;

        return new Vector3(minimumPosition.x + (Spacing.x * Coord.x), minimumPosition.y + (Spacing.y * Coord.y), OffsetZ);
    }

    public void SetBoxColliderActive(bool condition)
    {
        bCollider.enabled = condition;
    }

    public void MoveToAnotherPosition(Vector3 targetPosition)
    {
        if (this != null)
        {
            StartCoroutine(MoveTo(targetPosition));
        }
    }

    IEnumerator MoveTo(Vector3 targetPosition)
    {
        NewLocation = targetPosition;
        InitialLocation = targetPosition;

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (Vector3.Distance(rectTransform.localPosition, NewLocation) > MINIMUM_DIST)
            {
                rectTransform.localPosition = Vector3.MoveTowards(rectTransform.localPosition, NewLocation, Speed);
            }
            else
            {
                OnReachLocation.Invoke(this);
                yield break;
            }
        }
    }

    public void SetCoord(int x, int y)
    {
        Coord.x = x;
        Coord.y = y;
    }

    public void Destroy()
    {
        if(audioManager) audioManager.PlaySFX("Tile_Break");

        GameObject particle = Instantiate(particleSystem, this.transform.position, particleSystem.transform.rotation) as GameObject;
        Destroy(this.gameObject);
    }
}
