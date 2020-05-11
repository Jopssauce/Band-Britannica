using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PopUpHandler : MonoBehaviour
{
    public List<ShowTutorialPopUp> PopUps;

    private TutorialManager tutorialManager;
    private BoardHandler boardHandler;

    private void Start()
    {
        tutorialManager = SingletonManager.Get<TutorialManager>();
        tutorialManager.OnPopUpActive.AddListener(PausePopUp);

        boardHandler = SingletonManager.Get<BoardHandler>();
        boardHandler.OnTileMatchedRelease.AddListener(CheckPopUp);
    }

    void HandlePopUp(int index)
    {
        List<ShowTutorialPopUp> activePU = new List<ShowTutorialPopUp>();
        List<ShowTutorialPopUp> toRemovePU = new List<ShowTutorialPopUp>();

        //Separate active and passive pop ups
        foreach (var item in PopUps)
        {
            if (item.ToShow.activeSelf) activePU.Add(item);
        }
    }

    void CheckPopUp()
    {
        HandlePopUp(tutorialManager.Index);
    }

    void PausePopUp(int index)
    {
        if (index >= PopUps.Count) return;

        PopUps[index].Pause();
    }
}
