using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NotReaper.Notifications
{
    

public class Notification : MonoBehaviour {

    [Header("OBJECT")]
    public GameObject notificationObject;
    public Animator notificationAnimator;

    [Header("OBJECT")]
    public TextMeshProUGUI titleObject;
    public TextMeshProUGUI descriptionObject;

    [Header("VARIABLES")]
    public string titleText;
    public string descriptionText;
    public string animationNameIn;
    public string animationNameOut;

    private bool isPlayed = false;

    void Start() {
        notificationObject.SetActive (false);
    }

    public void ShowNotification(string title = "", string desc = "", float waitTime = 0.0f) {
        notificationObject.SetActive(true);
        titleObject.text = title;
        descriptionObject.text = desc;

        notificationAnimator.Play(animationNameIn);

        StartCoroutine(WaitForOut(waitTime));


    }

    IEnumerator WaitForOut(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        notificationAnimator.Play(animationNameOut);
        notificationObject.SetActive(false);
    }

}
}