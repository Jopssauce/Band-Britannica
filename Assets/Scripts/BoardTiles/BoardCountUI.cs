using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardCountUI : MonoBehaviour
{
    public TextMeshPro MatchCountText;
    public float TargetY = 40.0f;
    public float Speed = 35.0f;

    private CombatManager cManager;
    private BoardHandler bHandler;
    private RectTransform textRectTransform;
    private Coroutine animationCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        cManager = SingletonManager.Get<CombatManager>();
        bHandler = SingletonManager.Get<BoardHandler>();

        bHandler.OnStartBoardEffect.AddListener(ShowMatchCount);
        textRectTransform = MatchCountText.GetComponent<RectTransform>();
    }

    public void ShowMatchCount(List<Tile> tiles)
    {
        MatchCountText.enabled = true;

        Vector3 localPos = GetCenterPoint(tiles);
        localPos.z = textRectTransform.localPosition.z;
        Vector3 animPos = localPos;
        animPos.y += TargetY;

        textRectTransform.localPosition = localPos;
        MatchCountText.text = "-" + cManager.GetCorrespondTurnDeduct(tiles.Count).ToString();
        MatchCountText.color = tiles.First().AttributeColor;

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(StartAnimate(animPos));
    }

    IEnumerator StartAnimate(Vector3 targetLocation)
    {
        while(true)
        {
            textRectTransform.localPosition = Vector3.MoveTowards(textRectTransform.localPosition, targetLocation, Time.deltaTime * Speed);
            
            if(Vector2.Distance(textRectTransform.localPosition,targetLocation) < 0.01)
            {
                MatchCountText.enabled = false;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    Vector2 GetCenterPoint(List<Tile> tiles)
    {
        Vector3 centerPoint = Vector3.zero;

        foreach (var tile in tiles)
        {
            centerPoint += tile.GetDerivedRectTransformLocalPosition();
        }

        centerPoint /= tiles.Count;

        return centerPoint;
    }
}
