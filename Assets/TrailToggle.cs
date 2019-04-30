using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrailToggle : MonoBehaviour
{
    public GameObject Trail;
    public Toggle tog;

    void Start()
    {
        tog.isOn = PlayerPrefs.GetInt("AllTrails") == 1 ? true : false;
        Trail.SetActive(tog.isOn);

        Timeline.TimelineStatic.UpdateTrail();
    }

    public void toggled()
    {
        Trail.SetActive(tog.isOn);
        Timeline.TimelineStatic.UpdateTrail();
        PlayerPrefs.SetInt("AllTrails", tog.isOn ? 1 : 0);
    }
}
