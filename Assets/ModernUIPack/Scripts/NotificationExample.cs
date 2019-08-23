using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class NotificationExample : MonoBehaviour
    {
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

        void Start()
        {
            //notificationObject.SetActive (false);
        }

        public void ShowNotification()
        {
            notificationObject.SetActive(true);
            titleObject.text = titleText;
            descriptionObject.text = descriptionText;

            notificationAnimator.Play(animationNameIn);
            notificationAnimator.Play(animationNameOut);
        }
    }
}
