using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookmarkColor : MonoBehaviour
{
    public Color color;
    public GameObject glow;


    private void Awake()
    {
        color = GetComponent<Image>().color;
    }

    public void EnableGlow(bool enable)
    {
        glow.SetActive(enable);
    }
}
