using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace NotReaper.UI {


    public class ParallaxBG : MonoBehaviour {


        [SerializeField] private RectTransform bgImage;

        private void Start() {
            //StartCoroutine(MoveBG());
        }


        IEnumerator MoveBG() {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 bgPos = new Vector2(mousePos.x * -0.3f, mousePos.y * -0.3f);

            bgImage.DOMove(new Vector3(bgPos.x, bgPos.y, 52f), 0.3f).SetEase(Ease.InOutCubic);

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(MoveBG());

        }

        private void Update() {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 bgPos = new Vector2(mousePos.x * -0.8f * NRSettings.config.bgMoveMultiplier, mousePos.y * -0.8f * NRSettings.config.bgMoveMultiplier);

            bgImage.DOLocalMove(new Vector3(bgPos.x, bgPos.y, 52f), 0.1f).SetEase(Ease.InOutQuad);
        }

    }

}