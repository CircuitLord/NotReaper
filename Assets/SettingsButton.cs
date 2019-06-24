using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public GameObject SettingsPanel;

    private bool open;
    
    public void Start()
    {
        open = false;
    }
    public void ButtonPress()
    {
        if(open)
        {
            open = false;
            SettingsPanel.SetActive(false);
        }
        else
        {
            open = true;
            SettingsPanel.SetActive(true);
        }
    }
}
