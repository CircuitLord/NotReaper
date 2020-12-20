using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Timing;
using NotReaper;
using NotReaper.UI;
using NotReaper.Models;
using NotReaper.Modifier;
using System.Linq;
using System;

namespace NotReaper.Modifier
{
    public class ModifierTimeline : MonoBehaviour
    {
        public static ModifierTimeline Instance = null;
        //public bool startSet => currentPair.startMarkTop != null;
       // public bool endMarkExists => currentPair.endMarkTop is null;

        private Modifier currentModifier;

        [Header("References")]
        [SerializeField] private GameObject modifierStartPrefab;
        [SerializeField] private GameObject modifierEndPrefab;
        [SerializeField] private LineRenderer modifierConnectorPrefab;

        //public List<ModifierContainer> modifiers = new List<ModifierContainer>();
        //private ModifierContainer currentPair;
        private Vector3 pStart = new Vector3(0f, 5.1f, -3f);

        /*public void UpdateConnectors(float newScale)
        {
            foreach (ModifierContainer mc in modifiers)
            {
                if (mc.connector is null) continue;
                Vector3 scale = mc.connector.transform.localScale;
                scale.x = newScale;
                mc.connector.transform.localScale = scale;

            }
        }*/

        public void Start()
        {
            if (Instance is null) Instance = this;
            else
            {
                Debug.LogWarning("Trying to create a second instance of ModifierTimeline.");
                return;
            }
        }
        /*
        public void InitializeModifier(ModifierType type, string shorthand)
        {
            currentModifier = new Modifier(type.ToString(), shorthand);
        }
        */
        /*
        public void OptimizeModifiers()
        {

            if (NRSettings.config.optimizeInvisibleTargets)
            {
                if (ModifierHandler.activated)
                {
                    foreach (ModifierContainer mc in modifiers)
                    {

                        if (mc.startMarkTop.GetComponent<TimelineEntry>().transform.TransformPoint(transform.localPosition).x > 50f || mc.startMarkTop.GetComponent<TimelineEntry>().transform.TransformPoint(transform.localPosition).x < -50f)
                        {
                            ActivateModifierContainer(mc, false);
                        }
                        else
                        {
                            ActivateModifierContainer(mc, true);
                        }
                    }
                }

            }
        }
        
        private void ActivateModifierContainer(ModifierContainer container, bool active)
        {
            if (container.startMarkTop != null) container.startMarkTop.SetActive(active);
            if (container.startMarkBottom != null) container.startMarkBottom.SetActive(active);
            if (container.endMarkBottom != null) container.endMarkBottom.SetActive(active);
            if (container.endMarkTop != null) container.endMarkTop.SetActive(active);
            if (container.connector != null) container.connector.gameObject.SetActive(active);

        }
        */
        /*
        public void RemoveModifier(ModifierContainer container)
        {
            modifiers.Remove(container);
        }
        */
        /*public void CreateModifier(ModifierHandler.ModifierData data, bool fromLoad = false)
        {
            LookForOtherModifiers(data.startTick, data.endTick);
            CreateConnector();
            UpdateLineBoxCollider();
            currentPair.data = data;
            if (fromLoad) FixPosition();
            currentPair.startMarkTop.transform.position = new Vector3(currentPair.startMarkTop.transform.position.x, currentPair.startMarkTop.transform.position.y, -3f);
            currentPair.startMarkTop.GetComponent<SpriteRenderer>().color = Color.white;
            currentPair.startMarkBottom.GetComponent<SpriteRenderer>().color = Color.white;


            if (currentPair.endMarkTop != null)
            {
                currentPair.endMarkTop.GetComponent<SpriteRenderer>().color = Color.white;
                currentPair.endMarkBottom.GetComponent<SpriteRenderer>().color = Color.white;
                currentPair.connector.colorGradient = GetGradient(1f);
                //UpdateLineBoxCollider();
            }
            modifiers.Add(currentPair);
            currentPair.startMarkTop.GetComponent<TimelineEntry>().SetContainer(currentPair, fromLoad);
            
            currentPair = new ModifierContainer();
            // modifiers.Add(currentPair);

        }*/
        /*
        public void SelectModifier(ModifierContainer container)
        {
            //Debug.Log(container.startMarkTop.transform.name + " " + container.startMarkTop.transform.position);
            DropMarks();
            currentPair = container;
            modifiers.Remove(container);
        }

        private void PrintData()
        {
            Debug.Log(currentPair.startMarkTop.name);
            Debug.Log(currentPair.endMarkTop.name);
            Debug.Log(currentPair.data.startPosX);
            Debug.Log(currentPair.startTick);
        }

        public void FixPosition()
        {
            //Vector3 pos = currentPair.startMarkTop.transform.localPosition;
            //pos.z = -3f;    
            //currentPair.startMarkTop.transform.localPosition = pos;
            //currentPair.startMarkTop.transform.localScale = new Vector3(.3f, .3f, .3f);
            //UpdateLinePositions();
            //UpdateLineBoxColliderLoad();
        }
        */
        /*public float GetStartPosX()
        {
            return currentPair.startMarkTop.transform.localPosition.x;
        }
        public float GetEndPosX()
        {
            return currentPair.endMarkTop is null ? 0f : currentPair.endMarkTop.transform.localPosition.x;
        }*/
        /*
        private void LookForOtherModifiers(QNT_Timestamp startTick, QNT_Timestamp endTick)
        {
            //if (currentPair.raised) return;
            QNT_Timestamp _endTick = endTick;
            if (endTick.tick == 0) _endTick = startTick;
            for (int i = 0; i < Enum.GetNames(typeof(ModifierType)).Length; i++)
            {
                bool skip = false;

                foreach (ModifierContainer mc in modifiers)
                {
                    if (mc.data.startTick <= startTick && mc.data.endTick >= _endTick)
                    {
                        if (mc.level == i)
                        {
                            skip = true;
                            break;
                        }

                    }
                    else if (startTick <= mc.data.startTick && _endTick >= mc.data.startTick)
                    {
                        if (mc.level == i)
                        {
                            skip = true;
                            break;
                        }
                    }
                }
                if (skip) continue;
                if (currentPair.level >= i) return;
                currentPair.level = i;
                break;
            }
            float addY = currentPair.level * .3f;
            if (currentPair.startMarkTop != null)
            {
                pStart.x = currentPair.startMarkTop.transform.position.x;
                currentPair.startMarkTop.transform.position = new Vector3(pStart.x, pStart.y - addY, pStart.z);
            }
            if (currentPair.endMarkTop != null)
            {
                pStart.x = currentPair.endMarkTop.transform.position.x;
                currentPair.endMarkTop.transform.position = new Vector3(pStart.x, pStart.y - addY, pStart.z);
            }
        }

        private void LookForOtherModifiers(QNT_Timestamp tick)
        {
            for (int i = 0; i < Enum.GetNames(typeof(ModifierType)).Length; i++)
            {
                bool skip = false;

                foreach (ModifierContainer mc in modifiers)
                {
                    if (mc.data.startTick <= tick && mc.data.endTick >= tick)
                    {
                        if (mc.level == i)
                        {
                            skip = true;
                            break;
                        }

                    }
                    else if (mc.data.startTick == tick || mc.data.endTick == tick)
                    {
                        if (mc.level == i)
                        {
                            skip = true;
                            break;
                        }
                    }
                }
                if (skip) continue;
                currentPair.level = i;
                break;
            }
            float addY = currentPair.level * .3f;
            if (currentPair.startMarkTop != null)
            {
                pStart.x = currentPair.startMarkTop.transform.position.x;
                currentPair.startMarkTop.transform.position = new Vector3(pStart.x, pStart.y - addY, pStart.z);
            }
            if (currentPair.endMarkTop != null)
            {
                pStart.x = currentPair.endMarkTop.transform.position.x;
                currentPair.endMarkTop.transform.position = new Vector3(pStart.x, pStart.y - addY, pStart.z);
            }
        }
        */
        /*public bool CanCreateModifier(ModifierHandler.ModifierType type, QNT_Timestamp tick)
        {
            if (type != ModifierHandler.ModifierType.ColorUpdate && type != ModifierHandler.ModifierType.PsychedeliaUpdate) return true;

            foreach (ModifierContainer mc in modifiers)
            {
                if (mc.data.startTick < tick && mc.data.endTick > tick)
                {
                    if (mc.data.type == ModifierHandler.ModifierType.ColorChange && type == ModifierHandler.ModifierType.ColorUpdate)
                    {
                        return true;
                    }

                    else if (mc.data.type == ModifierHandler.ModifierType.Psychedelia && type == ModifierHandler.ModifierType.PsychedeliaUpdate) return true;
                }
                else if (mc.data.endTick.tick == 0 && tick.tick > 0)
                {
                    if (mc.data.type == ModifierHandler.ModifierType.ColorChange && type == ModifierHandler.ModifierType.ColorUpdate) return true;
                    else if (mc.data.type == ModifierHandler.ModifierType.Psychedelia && type == ModifierHandler.ModifierType.PsychedeliaUpdate) return true;
                }
            }

            return false;
        }*/
        /*
        public void DropMarksSave()
        {
            CreateModifier(currentPair.data);
        }

        public void DropMarks()
        {
            if (currentPair.startMarkTop != null)
            {
                GameObject.Destroy(currentPair.startMarkTop);
                GameObject.Destroy(currentPair.startMarkBottom);
            }
            if (currentPair.endMarkTop != null)
            {
                GameObject.Destroy(currentPair.endMarkTop);
                GameObject.Destroy(currentPair.endMarkBottom);
            }
            if (currentPair.connector != null)
            {
                GameObject.Destroy(currentPair.connector.gameObject);
            }


            currentPair = new ModifierContainer();
        }
        */
        /*
        public void ShowModifiers(bool show)
        {
            if (modifiers.Count == 0) return;
            foreach (ModifierContainer mc in modifiers)
            {
                if (mc.startMarkTop != null)
                {
                    mc.startMarkTop.SetActive(show);
                }

                if (mc.startMarkBottom != null) mc.startMarkBottom.SetActive(show);
                if (mc.endMarkTop != null)
                {
                    mc.endMarkTop.SetActive(show);
                }
                if (mc.endMarkBottom != null) mc.endMarkBottom.SetActive(show);
                if (mc.connector != null)
                {
                    mc.connector.gameObject.SetActive(show);
                }

            }
        }
        */
        

        /*public void UpdateMark(UpdateType type, ulong tick = 0)
        {
            switch (type)
            {
                case UpdateType.MoveStart:
                    currentPair.startMarkTop.transform.position = new Vector3(0f, currentPair.startMarkTop.transform.position.y, 0f);
                    currentPair.startMarkBottom.transform.position = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(new QNT_Timestamp(tick)), 0f, 0f);
                    LookForOtherModifiers(currentPair.startTick, currentPair.endTick);
                    CreateConnector();
                    UpdateLineBoxCollider();
                    break;
                case UpdateType.UpdateStart:
                    currentPair.startMarkTop.transform.position = new Vector3(0f, currentPair.startMarkTop.transform.position.y, 0f);
                    currentPair.startMarkBottom.transform.position = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(new QNT_Timestamp(tick)), 0f, 0f);
                    if (currentPair.connector != null) GameObject.Destroy(currentPair.connector);
                    if (currentPair.endMarkTop != null) GameObject.Destroy(currentPair.endMarkTop);
                    if (currentPair.endMarkBottom != null) GameObject.Destroy(currentPair.endMarkBottom);
                    currentPair.endMarkTop = null;
                    currentPair.endMarkBottom = null;
                    break;
                case UpdateType.UpdateEnd:
                    if (currentPair.endMarkBottom != null) GameObject.Destroy(currentPair.endMarkBottom);
                    if (currentPair.endMarkTop != null) GameObject.Destroy(currentPair.endMarkTop);
                    if (currentPair.connector != null) GameObject.Destroy(currentPair.connector);
                    currentPair.endMarkTop = null;
                    currentPair.endMarkBottom = null;
                    break;
            }
        }

        public void SetModifierMark(ModifierHandler.ModifierType type, QNT_Timestamp tick, string shorthand, bool startMarker, float posX, bool usePosX = false)
        {
            GameObject modifierBottom = Instantiate(startMarker ? modifierStartPrefab : modifierEndPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, MiniTimeline.Instance.bookmarksParent);
            GameObject modifierTop = Instantiate(startMarker ? modifierStartPrefab : modifierEndPrefab, ModifierSelectionHandler.isPasting ? Timeline.timelineNotesStatic : null);
            if (startMarker) modifierTop.GetComponent<IconTextSetter>().SetText(shorthand);
            Color background = Color.white;
            background.a = .5f;
            modifierBottom.transform.localPosition = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(tick), 0f, 0f);
            if (!usePosX)
            {
                modifierTop.transform.position = startMarker ? new Vector3(0f, modifierTop.transform.position.y, -3f) : new Vector3(0f, currentPair.startMarkTop.transform.position.y, -3f);
            }
            else if (ModifierSelectionHandler.isPasting)
            {
                modifierTop.transform.localPosition = startMarker ? new Vector3(posX, modifierTop.transform.localPosition.y, -3f) : new Vector3(posX, currentPair.startMarkTop.transform.localPosition.y, -3f);
                //modifierTop.transform.position = startMarker ? new Vector3(posX, modifierTop.transform.position.y, -3f) : new Vector3(posX, currentPair.startMarkTop.transform.position.y, -3f);
            }
            else
            {
                modifierTop.transform.position = startMarker ? new Vector3(posX, modifierTop.transform.position.y, -3f) : new Vector3(posX, currentPair.startMarkTop.transform.position.y, -3f);
            }
               
            modifierTop.transform.SetParent(Timeline.timelineNotesStatic);
            modifierTop.transform.localScale = new Vector3(0.3f, 0.3f);
            modifierBottom.transform.localScale = new Vector3(10f, 10f);
            modifierBottom.GetComponent<SpriteRenderer>().color = background;
            modifierTop.GetComponent<SpriteRenderer>().color = background;

            if (startMarker)
            {

                currentPair.startTick = tick;
                if (currentPair.endMarkTop != null) currentPair.endMarkTop.transform.SetParent(Timeline.timelineNotesStatic);
                if (currentPair.startMarkBottom != null) GameObject.Destroy(currentPair.startMarkBottom);
                if (currentPair.startMarkTop != null) GameObject.Destroy(currentPair.startMarkTop);

                currentPair.startMarkBottom = modifierBottom;
                currentPair.startMarkTop = modifierTop;

                CreateConnector();

                LookForOtherModifiers(tick);
                UpdateLineBoxCollider();
                if (currentPair.endMarkTop != null) currentPair.endMarkTop.transform.SetParent(currentPair.startMarkTop.transform);
            }
            else
            {
                currentPair.endTick = tick;
                if (currentPair.endMarkBottom != null) GameObject.Destroy(currentPair.endMarkBottom);
                if (currentPair.endMarkTop != null) GameObject.Destroy(currentPair.endMarkTop);
                currentPair.endMarkBottom = modifierBottom;
                currentPair.endMarkTop = modifierTop;
                currentPair.endMarkTop.GetComponent<ClickNotifier>().SetEntry(currentPair.startMarkTop);
                CreateConnector();
                LookForOtherModifiers(currentPair.startTick, currentPair.endTick);
                UpdateLineBoxCollider();
                //currentPair.endMarkTop.transform.SetParent(currentPair.startMarkTop.transform);
            }
        }

        private void CreateConnector()
        {
            if (currentPair.endMarkTop != null)
            {
                LineRenderer lr = GameObject.Instantiate(modifierConnectorPrefab, null);
                lr.SetPosition(0, currentPair.startMarkTop.transform.position);
                lr.SetPosition(1, currentPair.endMarkTop.transform.position);
                lr.transform.SetParent(Timeline.timelineNotesStatic);
                lr.colorGradient = GetGradient(.5f);
                lr.GetComponent<ClickNotifier>().SetEntry(currentPair.startMarkTop);
                if (currentPair.connector != null) GameObject.Destroy(currentPair.connector.gameObject);
                currentPair.connector = lr;
            }
        }

        private void UpdateLineBoxCollider()
        {
            UpdateLinePositions();
            if (currentPair.connector != null)
            {

                BoxCollider boxCollider = currentPair.connector.GetComponent<BoxCollider>();

                boxCollider.center = new Vector3((currentPair.connector.GetPosition(1).x + currentPair.connector.GetPosition(0).x) / 2, currentPair.connector.GetPosition(0).y, -1.5f);
                boxCollider.size = new Vector3(currentPair.connector.GetPosition(1).x - currentPair.connector.GetPosition(0).x - .33f, boxCollider.size.y, -1.5f);
                boxCollider.enabled = false;
            }
        }

        public void UpdateLineBoxColliderLoad()
        {
            if (currentPair.connector != null)
            {

                BoxCollider boxCollider = currentPair.connector.GetComponent<BoxCollider>();

                boxCollider.center = new Vector3(currentPair.startMarkTop.transform.localPosition.x / 2, currentPair.connector.GetPosition(0).y, -1.5f);
                //boxCollider.size = new Vector2(currentPair.connector.GetPosition(0).x, boxCollider.size.y);
                boxCollider.size = new Vector3(currentPair.connector.GetPosition(0).x, boxCollider.size.y, -1.5f);
                boxCollider.enabled = false;
            }
        }

        private void UpdateLinePositions()
        {
            if (currentPair.connector is null || currentPair.endMarkTop is null) return;

            currentPair.connector.SetPosition(0, currentPair.startMarkTop.transform.position);
            currentPair.connector.SetPosition(1, currentPair.endMarkTop.transform.position);
        }

        private Gradient GetGradient(float alpha)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0f) }
                );
            return gradient;
        }
        
        public struct ModifierContainer
        {
            public GameObject startMarkTop;
            public GameObject startMarkBottom;
            public GameObject endMarkTop;
            public GameObject endMarkBottom;
            public LineRenderer connector;
            public int level;
            public QNT_Timestamp startTick;
            public QNT_Timestamp endTick;
            public ModifierHandler.ModifierData data;

            public void Reset()
            {
                startMarkBottom = null;
                startMarkTop = null;
                endMarkTop = null;
                endMarkBottom = null;
                connector = null;
            }
        }

        public enum UpdateType
        {
            UpdateStart,
            MoveStart,
            UpdateEnd
        }
        */
    }

}
