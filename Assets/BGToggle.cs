using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGToggle : MonoBehaviour
{
    public GameObject BG;
   

    public void BGToggleButton()
    {
        BG.SetActive(BG.activeSelf ? false : true);
    }
}
