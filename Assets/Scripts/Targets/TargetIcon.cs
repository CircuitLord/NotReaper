using System;
using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.Targets {

	public enum TargetIconLocation {
		Timeline,
		Grid
	}

    public class TargetIcon : MonoBehaviour {

        public GameObject standard;
        public GameObject hold;
        public GameObject horizontal;
        public GameObject vertical;
        public GameObject chainStart;
        public GameObject chain;
        public GameObject melee;
        public GameObject line;
        public GameObject pathBuilder;

        public SpriteRenderer standardOutline;
        public SpriteRenderer holdOutline;
        public SpriteRenderer horzOutline;
        public SpriteRenderer vertOutline;
        public SpriteRenderer chainStartOutline;
        public SpriteRenderer chainOutline;
        public SpriteRenderer meleeOutline;
        public SpriteRenderer pathBuilderOutline;
        public SphereCollider sphereCollider;

        public TargetData data;

        public float sustainDirection = 0.6f;
        public bool isSelected = false;
        public TargetIconLocation location;

        public ParticleSystem holdParticles;

        /// <summary>
        /// For when the note is right clicked on. Bool is for if it should gen an undo action
        /// </summary>
        public event Action OnTryRemoveEvent;

        public void OnTryRemove() {
            OnTryRemoveEvent();
        }

        public void Remove() {
            Destroy(gameObject);
        }

        public event Action IconEnterLoadedNotesEvent;
        public event Action IconExitLoadedNotesEvent;

        public void IconEnterLoadedNotes() {
            IconEnterLoadedNotesEvent();
        }

        public void IconExitLoadedNotes() {
            IconExitLoadedNotesEvent();
        }

        public event Action TrySelectEvent;
        public event Action TryDeselectEvent;

        public void TrySelect() {
            TrySelectEvent();
        }

        public void TryDeselect() {
            TryDeselectEvent();
        }

        public void Init(TargetData targetData) {
            data = targetData;
            data.HandTypeChangeEvent += OnHandTypeChanged;
            data.BehaviourChangeEvent += OnBehaviorChanged;
            data.BeatLengthChangeEvent += OnSustainLengthChanged;
        }

        public void EnableSelected(TargetBehavior behavior) {
            standardOutline.enabled = (behavior == TargetBehavior.Standard);
            holdOutline.enabled = (behavior == TargetBehavior.Hold);
            horzOutline.enabled = (behavior == TargetBehavior.Horizontal);
            vertOutline.enabled = (behavior == TargetBehavior.Vertical);
            chainStartOutline.enabled = (behavior == TargetBehavior.ChainStart);
            chainOutline.enabled = (behavior == TargetBehavior.Chain);
            meleeOutline.enabled = (behavior == TargetBehavior.Melee);
            pathBuilderOutline.enabled = (behavior == TargetBehavior.NR_Pathbuilder);

            isSelected = true;
        }

        public void DisableSelected() {
            if (!standardOutline) return;
            standardOutline.enabled = false;
            holdOutline.enabled = false;
            horzOutline.enabled = false;
            vertOutline.enabled = false;
            chainStartOutline.enabled = false;
            chainOutline.enabled = false;
            meleeOutline.enabled = false;
            pathBuilderOutline.enabled = false;

            isSelected = false;
        }


        public void SetOutlineColor(Color color) {
            standardOutline.color = color;
            holdOutline.color = color;
            horzOutline.color = color;
            vertOutline.color = color;
            chainStartOutline.color = color;
            chainOutline.color = color;
            meleeOutline.color = color;
            pathBuilderOutline.color = color;
        }

        private void OnHandTypeChanged(TargetHandType handType) {
            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>(true)) {

                if (r.name == "WhiteRing") continue;

                switch (handType) {
                    case TargetHandType.Left:
                        r.material.SetColor("_Tint", NRSettings.config.leftColor);
                        break;
                    case TargetHandType.Right:
                        r.material.SetColor("_Tint", NRSettings.config.rightColor);
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
                        l.startColor = NRSettings.config.leftColor;
                        l.endColor = NRSettings.config.leftColor;
                        sustainDirection = 0.6f;
                        break;
                    case TargetHandType.Right:
                        l.startColor = NRSettings.config.rightColor;
                        l.endColor = NRSettings.config.rightColor;
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


                if (data.behavior == TargetBehavior.Hold && l.positionCount >= 3) {
                    l.SetPosition(1, new Vector3(0.0f, sustainDirection, 0.0f));
                    var pos2 = l.GetPosition(2);
                    l.SetPosition(2, new Vector3(pos2.x, sustainDirection, pos2.z));
                }
            }
        }

        private void OnSustainLengthChanged(float beatLength) {
            UpdateTimelineSustainLength();
        }

        public void UpdateTimelineSustainLength() {
            if (data.behavior != TargetBehavior.Hold) {
                return;
            }

            float scale = 20.0f / Timeline.scale;
            float beatLength = data.beatLength;

            var lineRenderers = gameObject.GetComponentsInChildren<LineRenderer>(true);
            foreach (LineRenderer l in lineRenderers) {
                if (l.positionCount < 3) {
                    continue;
                }

                if (beatLength >= 1) {
                    l.SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
                    l.SetPosition(1, new Vector3(0.0f, sustainDirection, 0.0f));
                    beatLength = beatLength / 480;
                    l.SetPosition(2, new Vector3((beatLength / 0.7f) * scale, sustainDirection, 0.0f));
                }
                else {
                    l.SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
                    l.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
                    l.SetPosition(2, new Vector3(0.0f, 0.0f, 0.0f));
                }
            }
        }

        private void OnBehaviorChanged(TargetBehavior behavior) {
            standard.SetActive(behavior == TargetBehavior.Standard);
            hold.SetActive(behavior == TargetBehavior.Hold);
            horizontal.SetActive(behavior == TargetBehavior.Horizontal);
            vertical.SetActive(behavior == TargetBehavior.Vertical);
            chainStart.SetActive(behavior == TargetBehavior.ChainStart);
            chain.SetActive(behavior == TargetBehavior.Chain);
            melee.SetActive(behavior == TargetBehavior.Melee);
            pathBuilder.SetActive(behavior == TargetBehavior.NR_Pathbuilder);

            sphereCollider.radius = 0.5f;
            if (behavior == TargetBehavior.Chain && location == TargetIconLocation.Timeline) {
                sphereCollider.radius = 0.25f;
            }

            if(behavior == TargetBehavior.NR_Pathbuilder) {
                data.handType = TargetHandType.None;
                data.velocity = TargetVelocity.None;
            }
        }
    }
}