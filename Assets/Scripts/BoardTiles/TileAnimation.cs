using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileAnimAxis
{
    X,
    Y,
    Z,
    XR
}

public class TileAnimation : MonoBehaviour
{
    public TileAnimAxis CurAnimAxis = TileAnimAxis.Y;

    [Header("Float Settings")]
    public bool EnableFloating = true;
    public float Strength = 12.0f;
    public float Slow = 0.3f;
    public int Range = 100;
    public bool UseRandomRange = true;

    RectTransform rectTransform;
    Tile tile;
    int randNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        tile = GetComponent<Tile>();
        rectTransform = GetComponent<RectTransform>();

        if (UseRandomRange)
            randNum = Random.Range(-Range, Range);
        else randNum = Range;
    }

    // Update is called once per frame
    void Update()
    {
        if(EnableFloating == true)
        {
            AnimateTile();
        }
        else
        {
            if(rectTransform != null)
            {
                rectTransform.localPosition = tile.InitialLocation;
            }
        }
    }

    void AnimateTile()
    {
        switch (CurAnimAxis)
        {
            case TileAnimAxis.X:
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + (Mathf.Sin(randNum + Time.time * Strength) * Slow), rectTransform.localPosition.y, rectTransform.localPosition.z);
                break;
            case TileAnimAxis.Y:
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + (Mathf.Sin(randNum + Time.time * Strength) * Slow), rectTransform.localPosition.z);
                break;
            case TileAnimAxis.Z:
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z + (Mathf.Sin(randNum + Time.time * Strength) * Slow));
                break;
            case TileAnimAxis.XR:
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x - (Mathf.Sin(randNum + Time.time * Strength) * Slow), rectTransform.localPosition.y, rectTransform.localPosition.z);
                break;
            default:
                break;
        }
    }
}
