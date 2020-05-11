using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPanelHandler : MonoBehaviour
{
    public PanelPopUp[] panels;

    // Start is called before the first frame update
    void Start()
    {
        foreach (PanelPopUp item in panels)
        {
            if (item.isUp == true)
            {
                item.PopDown();
            }
        }
    }
}
