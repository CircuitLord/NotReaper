using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.Targets {


    public class TargetIcon : MonoBehaviour {

        public GameObject standard;
        public GameObject hold;
        public GameObject horizontal;
        public GameObject vertical;
        public GameObject chainStart;
        public GameObject chain;
        public GameObject melee;
        public GameObject line;


        public SpriteRenderer standardOutline;
        public SpriteRenderer holdOutline;
        public SpriteRenderer horzOutline;
        public SpriteRenderer vertOutline;
        public SpriteRenderer chainStartOutline;
        public SpriteRenderer chainOutline;
        public SpriteRenderer meleeOutline;


        private Color leftColor;
        private Color rightColor;
        private Color eitherColor;
        private Color noneColor;

        public float sustainDirection = 0.6f;


        public bool IsGrid() {
            if (standardOutline == null) return false;
            else return false;
        }


        //Sets the proper scale for the icons.
        public void SetSizes() {
            if (!IsGrid()) return;

        }


        public void EnableSelected(TargetBehavior behavior) {
            standardOutline.enabled = (behavior == TargetBehavior.Standard);
            holdOutline.enabled = (behavior == TargetBehavior.Hold);
            horzOutline.enabled = (behavior == TargetBehavior.Horizontal);
            vertOutline.enabled = (behavior == TargetBehavior.Vertical);
            chainStartOutline.enabled = (behavior == TargetBehavior.ChainStart);
            chainOutline.enabled = (behavior == TargetBehavior.Chain);
            meleeOutline.enabled = (behavior == TargetBehavior.Melee);

        }

        public void DisableSelected() {
            if (standardOutline == null) return;
            standardOutline.enabled = false;
            holdOutline.enabled = false;
            horzOutline.enabled = false;
            vertOutline.enabled = false;
            chainStartOutline.enabled = false;
            chainOutline.enabled = false;
            meleeOutline.enabled = false;
        }


        public void SetHandType(TargetHandType handType) {
            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>(true)) {
                switch (handType) {
                    case TargetHandType.Left:
                        r.material.SetColor("_Tint", UserPrefsManager.leftColor);
                        break;
                    case TargetHandType.Right:
                        r.material.SetColor("_Tint", UserPrefsManager.rightColor);
                        break;
                    case TargetHandType.Either:
                        r.material.SetColor("_Tint", UserPrefsManager.bothColor);
                        break;
                    default:
                        r.material.SetColor("_Tint", UserPrefsManager.neitherColor);
                        break;
                }
            }
            foreach (LineRenderer l in gameObject.GetComponentsInChildren<LineRenderer>(true)) {
                switch (handType) {
                    case TargetHandType.Left:
                        l.startColor = UserPrefsManager.leftColor;
                        l.endColor = UserPrefsManager.leftColor;
                        sustainDirection = 0.6f;
                        break;
                    case TargetHandType.Right:
                        l.startColor = UserPrefsManager.rightColor;
                        l.endColor = UserPrefsManager.rightColor;
                        sustainDirection = -0.6f;
                        break;
                    case TargetHandType.Either:
                        l.startColor = UserPrefsManager.bothColor;
                        l.endColor = UserPrefsManager.bothColor;
                        sustainDirection = 0.6f;
                        break;
                    default:
                        l.startColor = UserPrefsManager.neitherColor;
                        l.endColor = UserPrefsManager.neitherColor;
                        sustainDirection = 0.6f;
                        break;
                }
            }

        }

        public void SetSustainLength(float beatLength) {
            foreach (LineRenderer l in gameObject.GetComponentsInChildren<LineRenderer>(true)) {
                if (beatLength >= 1) {
                    l.SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
                    l.SetPosition(1, new Vector3(0.0f, sustainDirection, 0.0f));
                    beatLength = beatLength / 480;
                    Debug.Log("setting sustain length to: " + beatLength / 0.7f);
                    l.SetPosition(2, new Vector3(beatLength / 0.7f, sustainDirection, 0.0f));
                } else {
                    l.SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
                    l.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
                    l.SetPosition(2, new Vector3(0.0f, 0.0f, 0.0f));
                }
            }
        }

        public void SetBehavior(TargetBehavior behavior) {
            standard.SetActive(behavior == TargetBehavior.Standard);
            hold.SetActive(behavior == TargetBehavior.Hold);
            horizontal.SetActive(behavior == TargetBehavior.Horizontal);
            vertical.SetActive(behavior == TargetBehavior.Vertical);
            chainStart.SetActive(behavior == TargetBehavior.ChainStart);
            chain.SetActive(behavior == TargetBehavior.Chain);
            melee.SetActive(behavior == TargetBehavior.Melee);
        }
    }
}