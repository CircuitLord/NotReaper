using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace NotReaper.UI {


    public class ParallaxBG : MonoBehaviour {

        public static ParallaxBG I;
        

        [SerializeField] private RectTransform bgImage;


        private Camera _mainCamera;
        
        private void Start() {

            I = this;
            
            _mainCamera = Camera.main;
            //StartCoroutine(MoveBG());



        }


        IEnumerator MoveBG() {


            while (true) {
                
                Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

                Vector2 bgPos = new Vector2(mousePos.x * -0.8f * NRSettings.config.bgMoveMultiplier, mousePos.y * -0.8f * NRSettings.config.bgMoveMultiplier);

                bgImage.DOLocalMove(new Vector3(bgPos.x, bgPos.y, 52f), 0.1f).SetEase(Ease.InOutQuad);

                yield return new WaitForSeconds(0.3f);
            }

            //StartCoroutine(MoveBG());

        }

        private void Update() { 
            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 bgPos = new Vector2(mousePos.x * -0.8f, mousePos.y * -0.8f);

            Vector2 curPos = bgImage.localPosition;

            bgImage.localPosition = Vector2.Lerp(curPos, bgPos, 5f * Time.deltaTime * NRSettings.config.bgMoveMultiplier);
            //bgImage.DOLocalMove(new Vector3(bgPos.x, bgPos.y, 52f), 0.1f).SetEase(Ease.InOutQuad);


        }

        public void OnPlaceNote() {

            if (!NRSettings.config.useBouncyAnimations) return;
            
            DOTween.To((float scale) => {
                bgImage.localScale = new Vector3(scale, scale, 1f);
            }, 0.98f, 1f, 0.3f).SetEase(Ease.OutCubic);
        }

        public void OnMeleeHit(float x) {
            
            if (!NRSettings.config.useBouncyAnimations) return;
            
            if (x > 0) {
                bgImage.transform.localPosition = new Vector3(-4f, bgImage.transform.localPosition.y, bgImage.transform.localPosition.z);
            }
            else {
                bgImage.transform.localPosition = new Vector3(4f, bgImage.transform.localPosition.y, bgImage.transform.localPosition.z);
            }
            
            DOTween.To((float scale) => {
                bgImage.localScale = new Vector3(scale, scale, 1f);
            }, 0.97f, 1f, 0.35f).SetEase(Ease.OutCubic);

            bgImage.DOLocalMoveX(0f, 0.35f).SetEase(Ease.OutCubic);

        }

        /*public IEnumerator OnSustainHit(float time) {
            
            DOTween.To((float scale) => {
                bgImage.localScale = new Vector3(scale, scale, 1f);
            }, 1f, 1.2f, time).SetEase(Ease.Linear);
            
            yield return new WaitForSeconds(time);

            bgImage.DOScale(1f, 0.3f).SetEase(Ease.InOutCubic);

        }*/

    }

}