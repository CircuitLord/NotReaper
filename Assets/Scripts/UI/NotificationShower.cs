using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NotReaper.UserInput;

namespace NotReaper.UI {


    public class NotificationShower : MonoBehaviour {

        public Image nImage;
        public SpriteRenderer successIcon;
        public SpriteRenderer failIcon;
        public SpriteRenderer infoIcon;

        public RectTransform slider;
        public Image sliderImg;

        public TextMeshProUGUI text;


        public float notifOpacity = 0.9f;


        private static List<NRNotification> notifications = new List<NRNotification>();


        private void Start() {
            StartCoroutine(CheckPlayNotifications());

            //Hide the notif by default:
            text.alpha = 0;
            nImage.DOFade(0, 0);
            successIcon.DOFade(0, 0);
        }

        public static void AddNotifToQueue(NRNotification notif) {
            notifications.Add(notif);

        }

        IEnumerator CheckPlayNotifications() {

            while(notifications.Count <= 0) {
                yield return new WaitForSeconds(0.5f);
            }

            if (notifications[0] != null) {
                StartCoroutine(PlayAndHideNotification(notifications[0]));
                yield return new WaitForSeconds(notifications[0].duration + (float)NRSettings.config.UIFadeDuration);
                notifications.RemoveAt(0);
            }

            yield return new WaitForSeconds(1f);

            StartCoroutine(CheckPlayNotifications());

            






        }


        IEnumerator PlayAndHideNotification(NRNotification notification) {
            float fadeDuration = (float)NRSettings.config.UIFadeDuration;

            //TODO: Set icon to success or fail or info
            switch (notification.type) {
                case NRNotifType.Success:

                    break;
            }

            //Reset the notif;
            text.SetText(notification.content);
            sliderImg.color = EditorInput.GetSelectedColor();
            slider.sizeDelta = new Vector2(0, 0);

            //FADE IN
            nImage.DOFade(notifOpacity, fadeDuration / 2f);
            successIcon.DOFade(1f, fadeDuration / 2f);
            text.DOFade(1f, fadeDuration / 2f);
            sliderImg.DOFade(1f, fadeDuration / 2f);
            

            //Start the slider going
            DOTween.To(SetSliderWidth, 0f, 183.13f, notification.duration).SetEase(Ease.InOutCubic);

            yield return new WaitForSeconds(notification.duration);

            //FADE OUT
            nImage.DOFade(0, fadeDuration / 2f);
            successIcon.DOFade(0, fadeDuration / 2f);
            text.DOFade(0, fadeDuration / 2f);
            sliderImg.DOFade(0, fadeDuration / 2f);





            yield break;

        }

        private void SetSliderWidth(float width) {
            slider.sizeDelta = new Vector2(width, 2f);
        }

        


        

        




    }



    public class NRNotification {
        public NRNotifType type = NRNotifType.Success;
        public string content = "";
        public float duration;

        public NRNotification(string c, float dur = 4.0f) {
            content = c;
            duration = dur;
        }
        
    }

    public enum NRNotifType {
        Success,
        Fail,
        Info
    }

}