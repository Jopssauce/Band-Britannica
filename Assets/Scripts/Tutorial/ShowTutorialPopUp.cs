using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShowTutorialPopUp
{
    public GameObject ToShow;

    public void Pause()
    {
        ToShow.SetActive(!ToShow.activeSelf);
    }
}
