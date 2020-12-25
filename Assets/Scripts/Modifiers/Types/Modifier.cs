using System.Collections;
using UnityEngine;
using System;
using Newtonsoft.Json;
using NotReaper.Timing;
using NotReaper.UI;
using UnityEngine.UI;
using static NotReaper.Modifier.ModifierHandler;

namespace NotReaper.Modifier
{
    public class Modifier : MonoBehaviour
    {
        [Header("DTO Data")]
        public float amount;
        public float startPosX;
        public float endPosX;
        public float miniStartX;
        public float miniEndX;
        public string value1;
        public string value2;
        public bool option1;
        public bool option2;
        public float[] leftHandColor;
        public float[] rightHandColor;

        [Header("Data")]
        public ModifierHandler.ModifierType modifierType;
        public string shorthand;
        public QNT_Timestamp startTime;
        public QNT_Timestamp endTime;
        public bool startSet;
        public int level;

        public GameObject startMark;
        public GameObject endMark;
        public GameObject miniStart;
        public GameObject miniEnd;
        public LineRenderer connector;

        [Header("Prefabs")]
        public GameObject startMarkPrefab;
        public GameObject miniStartPrefab;
        public GameObject endMarkPrefab;
        public GameObject miniEndPrefab;
        public GameObject connectorPrefab;
        public GameObject glow;

        [Header("Trackers")]
        public bool isCreated;
        public bool isSelected;
        public bool startMarkExists => startMark != null;
        public bool endMarkExists => endMark != null;
        public bool miniStartExists => miniStart != null;
        public bool miniEndExists => miniEnd != null;
        public bool connectorExists => connector != null;

        private Vector3 pStartPos = new Vector3(0f, 0.4f, 0f);

        public void Show(bool show)
        {
            if (startMarkExists) startMark.SetActive(show);
            if (endMarkExists) endMark.SetActive(show);
            if (miniStartExists) miniStart.SetActive(show);
            if (miniEndExists) miniEnd.SetActive(show);
            if (connectorExists) connector.gameObject.SetActive(show);
        }
        private float GetStartPosX()
        {
            return startMark.transform.localPosition.x;
        }
        private float GetEndPosX()
        {
            return endMarkExists ? endMark.transform.localPosition.x : 0f;
        }

        private float GetMiniStartX()
        {
            if (!miniStartExists) return 0f;
            return miniStart.transform.localPosition.x;
        }

        private float GetMiniEndX()
        {
            if (!miniEndExists) return 0f;
            return miniEnd.transform.localPosition.x;
        }

        public ModifierDTO GetDTO()
        {
            ModifierDTO dto = new ModifierDTO();
            dto.amount = amount;
            dto.endPosX = GetEndPosX();
            dto.startPosX = GetStartPosX();
            dto.miniStartX = GetMiniStartX();
            dto.miniEndX = GetMiniEndX();
            dto.startTick = startTime.tick;
            dto.endTick = endTime.tick;
            dto.leftHandColor = leftHandColor;
            dto.rightHandColor = rightHandColor;
            dto.option1 = option1;
            dto.option2 = option2;
            dto.value1 = value1;
            dto.value2 = value2;
            dto.type = modifierType.ToString();
            return dto;
        }

        public void LoadFromDTO(ModifierDTO dto)
        {
            amount = dto.amount;
            startPosX = dto.startPosX;
            endPosX = dto.endPosX;
            miniStartX = dto.miniStartX;
            miniEndX = dto.miniEndX;
            startTime = new QNT_Timestamp((ulong)dto.startTick);
            endTime = new QNT_Timestamp((ulong)dto.endTick);
            leftHandColor = dto.leftHandColor;
            rightHandColor = dto.rightHandColor;
            option1 = dto.option1;
            option2 = dto.option2;
            value1 = dto.value1;
            value2 = dto.value2;
            modifierType = (ModifierHandler.ModifierType)Enum.Parse(typeof(ModifierHandler.ModifierType), dto.type);
        }

        public void Scale(float targetScale)
        {
            float s = .3f;
            s *= targetScale;
            Vector3 scale = new Vector3(s, .3f, .3f);

            if (startMarkExists)
            {
                startMark.transform.localScale = scale;
            }
            if (endMarkExists)
            {
                endMark.transform.localScale = scale;
            }
            if (connectorExists)
            {
                connector.transform.localScale = connector.GetComponent<Connector>().originalScale;
            }
        }

        public void CreateModifierMark(bool startMarker, QNT_Timestamp miniTime, bool usePosX = false)
        {
            GameObject modifierBottom = Instantiate(startMarker ? miniStartPrefab : miniEndPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, MiniTimeline.Instance.bookmarksParent);
            GameObject modifierTop = Instantiate(startMarker ? startMarkPrefab : endMarkPrefab, usePosX && !ModifierSelectionHandler.isPasting ? Timeline.timelineNotesStatic : null);
            if (startMarker)
            {
                modifierTop.GetComponent<IconTextSetter>().SetText(shorthand);
            }
               
            Color background = Color.white;
            background.a = .5f;
            modifierBottom.layer = 0;
            Destroy(modifierBottom.GetComponent<Collider2D>());
           
            if (!usePosX)
            {
                modifierTop.transform.position = startMarker ? new Vector3(0f, modifierTop.transform.position.y, 0f) : new Vector3(0f, startMark.transform.position.y, 0f);
                modifierBottom.transform.localPosition = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(miniTime), 0f, 0f);
            }
            else if (ModifierSelectionHandler.isPasting)
            {
                modifierTop.transform.position = startMarker ? new Vector3(startPosX * Timeline.scaleTransform, modifierTop.transform.position.y, 0f) : new Vector3(endPosX * Timeline.scaleTransform, startMark.transform.position.y, 0f);
                modifierBottom.transform.localPosition = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(miniTime), 0f, 0f);
            }
            else
            {
                modifierTop.transform.localPosition = startMarker ? new Vector3(startPosX, modifierTop.transform.localPosition.y, 0f) : new Vector3(endPosX, startMark.transform.localPosition.y, 0f);                
                modifierBottom.transform.localPosition = new Vector3(startMarker ? miniStartX : miniEndX, 0f, 0f);
            }

            modifierTop.transform.SetParent(Timeline.timelineNotesStatic);
            modifierTop.transform.localScale = new Vector3(0.3f, 0.3f);
            modifierBottom.transform.localScale = new Vector3(7f, 7f);
            modifierBottom.GetComponent<SpriteRenderer>().color = background;
            modifierTop.GetComponent<SpriteRenderer>().color = background;

            if (startMarker)
            {
                miniStart = modifierBottom;
                startMark = modifierTop;
                glow = startMark.transform.GetChild(0).gameObject;
                startMark.GetComponent<ClickNotifier>().SetModifier(this);
                LookForOtherModifiers(startTime, endTime, LookAtType.Start);
            }
            else
            {
                if (miniEndExists) GameObject.Destroy(miniEnd);
                if (endMarkExists) GameObject.Destroy(endMark);
                miniEnd = modifierBottom;
                endMark = modifierTop;
                endMark.GetComponent<ClickNotifier>().SetModifier(this);
                CreateConnector();
                LookForOtherModifiers(startTime, endTime, LookAtType.End);
                UpdateLinePositions();
            }
        }

        public void Select(bool select)
        {
            isSelected = select;
            glow.SetActive(select);
        }

        public void UpdateMark(UpdateType type, ulong tick = 0)
        {
            switch (type)
            {
                case UpdateType.MoveStart:
                    startMark.transform.position = new Vector3(0f, startMark.transform.position.y, 0f);
                    miniStart.transform.localPosition = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(new QNT_Timestamp(tick)), 0f, 0f);
                    LookForOtherModifiers(startTime, startTime, LookAtType.Start);
                    CreateConnector();
                    UpdateLinePositions();
                    break;
                case UpdateType.UpdateStart:
                    startMark.transform.position = new Vector3(0f, startMark.transform.position.y, 0f);
                    miniStart.transform.localPosition = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(new QNT_Timestamp(tick)), 0f, 0f);
                    if (connectorExists) GameObject.Destroy(connector);
                    if (endMarkExists) GameObject.Destroy(endMark);
                    if (miniEndExists) GameObject.Destroy(miniEnd);
                    LookForOtherModifiers(startTime, endTime, LookAtType.Start);
                    endMark = null;
                    miniEnd = null;
                    connector = null;
                    break;
                case UpdateType.UpdateEnd:
                    if (miniEndExists) GameObject.Destroy(miniEnd);
                    if (endMarkExists) GameObject.Destroy(endMark);
                    if (connectorExists) GameObject.Destroy(connector);
                    endMark = null;
                    miniEnd = null;
                    connector = null;
                    break;
            }
        }

        public void CreateModifier(bool save = false)
        {       
            CreateConnector();
            UpdateLinePositions();
            startMark.GetComponent<SpriteRenderer>().color = Color.white;
            miniStart.GetComponent<SpriteRenderer>().color = Color.white;


            if (endMarkExists)
            {
                endMark.GetComponent<SpriteRenderer>().color = Color.white;
                miniEnd.GetComponent<SpriteRenderer>().color = Color.white;
                connector.colorGradient = GetGradient(1f);
            }
            startPosX = GetStartPosX();
            endPosX = GetEndPosX();
            miniStartX = GetMiniStartX();
            miniEndX = GetMiniEndX();
            isCreated = true;
            if(!save) Select(false);
        }

        public void Delete()
        {
            if (ModifierHandler.Instance.modifiers.Contains(this)) ModifierHandler.Instance.modifiers.Remove(this);
                     
            if (startMarkExists)
            {
                GameObject.Destroy(startMark.gameObject);
                GameObject.Destroy(miniStart.gameObject);
            }
               
            if (endMarkExists)
            {
                GameObject.Destroy(endMark.gameObject);
                GameObject.Destroy(miniEnd.gameObject);
                GameObject.Destroy(connector.gameObject);
            }
            Destroy(this.gameObject);                
        }

        public void UpdateLevel()
        {
            level = 0;
            LookForOtherModifiers(startTime, endTime, LookAtType.Start);
            UpdateLinePositions(true);
        }

        private void LookForOtherModifiers(QNT_Timestamp startTick, QNT_Timestamp endTick, LookAtType type)
        {
            startTick.tick -= 120;
            QNT_Timestamp _endTick = endTick;
            if (endTick.tick == 0) _endTick = startTick;
            _endTick.tick += 120;
            int lowestLevelFound = 0;
            for (int i = 0; i < Enum.GetNames(typeof(ModifierType)).Length; i++)
            {
                bool skip = false;
                foreach (Modifier mod in ModifierHandler.Instance.modifiers)
                {
                    if (mod == this) continue;
                    if (mod.startTime <= startTick && mod.endTime >= _endTick)
                    {
                        if (mod.level == i)
                        {
                            skip = true;
                            break;
                        }
                    }
                    else if (startTick <= mod.startTime && _endTick >= mod.startTime)
                    {
                        if (mod.level == i)
                        {
                            skip = true;
                            break;
                        }
                    }
                    else if(startTick == mod.endTime && startTick.tick != 0)
                    {
                        if (mod.level == i)
                        {
                            skip = true;
                            break;
                        }
                    }
                    else if (startTick > mod.startTime && startTick < mod.endTime && _endTick > mod.endTime)
                    {
                        if (mod.level == i)
                        {
                            skip = true;
                            break;
                        }
                    }
                }
                if (skip) continue;
                lowestLevelFound = i;
                break;
            }           
            if (type == LookAtType.End)
            {
                if (lowestLevelFound < level) return;
            }

            level = lowestLevelFound;
            float addY = level * .3f;
            ModifierSelectionHandler.Instance.posGetter.localPosition = pStartPos;
            Vector3 newPos = ModifierSelectionHandler.Instance.posGetter.localPosition;
            newPos.y -= addY;

            if (startMarkExists)
            {                
                newPos.x = startMark.transform.localPosition.x;              
                startMark.transform.localPosition = newPos;
            }
            if (endMarkExists)
            {

                newPos.x = endMark.transform.localPosition.x;
                endMark.transform.localPosition = newPos;
            }
        }

        private enum LookAtType
        {
            Start,
            End
        }

        private void CreateConnector()
        {
            if (endMarkExists)
            {
                GameObject go = GameObject.Instantiate(connectorPrefab, null);
                LineRenderer lr = go.GetComponent<LineRenderer>();
                lr.SetPosition(0, startMark.transform.position);
                lr.SetPosition(1, endMark.transform.position);
                lr.transform.SetParent(Timeline.timelineNotesStatic);
                lr.colorGradient = GetGradient(.5f);
                lr.GetComponent<ClickNotifier>().SetModifier(this);
                if (connectorExists) GameObject.Destroy(connector.gameObject);
                connector = lr;
            }
        }

        private void UpdateLinePositions(bool useLocal = false)
        {
            if (!connectorExists) return;
            Vector3 newPos = startMark.transform.position;
            if (useLocal) newPos.x = connector.GetPosition(0).x;
            connector.SetPosition(0, newPos);
            newPos = endMark.transform.position;
            if (useLocal) newPos.x = connector.GetPosition(1).x;
            connector.SetPosition(1, newPos);
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

        public void ReportClick(bool singleSelect)
        {
            Select(singleSelect);
        }

        public void Optimize(bool enable)
        {
            if (startMarkExists) startMark.SetActive(enable);
            if (endMarkExists) endMark.SetActive(enable);
            if (connectorExists) connector.gameObject.SetActive(enable);
            if (miniStartExists) miniStart.SetActive(enable);
            if (miniEndExists) miniEnd.SetActive(enable);
        }

        public enum UpdateType
        {
            UpdateStart,
            MoveStart,
            UpdateEnd
        }
    }
}
