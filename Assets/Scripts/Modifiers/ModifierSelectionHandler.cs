using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper;
using UnityEngine.UI;
using System.Linq;
using System;
using NotReaper.Timing;
using Sirenix.Utilities;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NotReaper.Modifier
{
    public class ModifierSelectionHandler : MonoBehaviour
    {
        public static List<TimelineEntry> selectables = new List<TimelineEntry>();

        private static List<TimelineEntry> selectedEntries = new List<TimelineEntry>();
        public static TimelineEntry selectedEntry = null;

        private static List<TimelineEntry> copiedEntries = new List<TimelineEntry>();

        public static bool isPasting = false;

        private static Transform posGetter = null;

        private void Start()
        {
            posGetter = GameObject.Instantiate(new GameObject("PosGetter").transform);
            
        }
        public static void AddEntry(TimelineEntry entry)
        {
            ModifierHandler.Instance.HideWindow(true);
            if (selectedEntries.Contains(entry))
            {
                RemoveEntry(entry);
                return;
            }
            entry.Select(true);
            selectedEntries.Add(entry);
            
        }

        public static void RemoveEntry(TimelineEntry entry)
        {
            if (!selectedEntries.Contains(entry)) return;
            entry.Select(false);
            selectedEntries.Remove(entry);

            if(selectedEntries.Count == 0) ModifierHandler.Instance.HideWindow(false);
        }

        public static void RemoveSelectedEntry(TimelineEntry entry)
        {
            if (entry == selectedEntry) selectedEntry = null;
        }

        public static void RemoveAllEntries()
        {
            foreach (TimelineEntry entry in selectedEntries) entry.Select(false);
            selectedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);
        }

        public static void DeleteSelectedModifiers()
        {
            foreach(TimelineEntry entry in selectedEntries)
            {
                entry.DeleteModifier();
            }
            selectedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);
        }

        public static void CopySelectedModifiers()
        {
            copiedEntries.Clear();
            copiedEntries = selectedEntries.ToList();
            copiedEntries.Sort((mod1, mod2) => mod1.GetContainer().data.startTick.CompareTo(mod2.GetContainer().data.startTick));
        }

        public static void PasteCopiedModifiers()
        {
            if (copiedEntries.Count == 0) return;

            QNT_Timestamp newStartTick = Timeline.time;
            ulong tickOffset = newStartTick.tick - copiedEntries.First().GetContainer().data.startTick.tick;
            posGetter.position = Vector3.zero;
            posGetter.SetParent(Timeline.timelineNotesStatic);
            float positionOffset = posGetter.localPosition.x - copiedEntries.First().GetContainer().data.startPosX;
            if (tickOffset == 0) return;
            isPasting = true;
            List<Modifier> temp = new List<Modifier>();
            foreach(TimelineEntry te in copiedEntries) temp.Add(te.GetContainer().data.modifier);
            List<Modifier> clonedList = CloneList(temp);


            foreach (Modifier modifier in clonedList)
            {
                //Modifier m = modifier.GetContainer().data.modifier;//DeepCopyModifier(entry.GetContainer().data.modifier);

                modifier.startTick += tickOffset;
                Debug.Log("Pos before: " + modifier.startPosX);
                modifier.startPosX += positionOffset;
                Debug.Log("Intended position: " + modifier.startPosX);
                if (modifier.endTick != 0)
                {
                    modifier.endTick += tickOffset;
                    modifier.endPosX += positionOffset;
                }
                ModifierHandler.Instance.LoadModifier(modifier);
            }
            isPasting = false;
            RemoveAllEntries();
            copiedEntries.Clear();
        }

        private static List<TimelineEntry> CloneList<TimelineEntry>(List<TimelineEntry> oldList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<TimelineEntry>)formatter.Deserialize(stream);
        }

        private static Modifier DeepCopyModifier(Modifier modifier)
        {
            return null;
            /*
            Modifier m = null;
            ModifierType _type;
            Enum.TryParse(modifier.type, true, out _type);
            float amount = modifier.amount;
            QNT_Timestamp startTick = new QNT_Timestamp((uint)modifier.startTick);
            QNT_Timestamp endTick = new QNT_Timestamp((uint)modifier.endTick);
            ModifierType type = _type;
            float startPosX = modifier.startPosX;
            float endPosX = modifier.endPosX;
            bool option1 = false;
            bool option2 = false;
            float[] leftHandColor;
            float[] rightHandColor;
            string value1 = "";
            string value2 = "";
            switch (type)
            {
                case ModifierType.ArenaBrightness:
                    ArenaBrightness ab = modifier as ArenaBrightness;
                    option1 = ab.continuous;
                    option2 = ab.strobo;
                    break;
                case ModifierType.ArenaRotation:
                    ArenaRotation ar = modifier as ArenaRotation;
                    option1 = ar.continuous;
                    break;
                case ModifierType.ColorChange:
                    ColorChange cc = modifier as ColorChange;
                    leftHandColor = cc.leftHandColor;
                    rightHandColor = cc.rightHandColor;
                    break;
                case ModifierType.ColorUpdate:
                    ColorUpdate cu = modifier as ColorUpdate;
                    leftHandColor = cu.leftHandColor;
                    rightHandColor = cu.rightHandColor;
                    break;
                case ModifierType.zOffset:
                    ZOffset zo = modifier as ZOffset;
                    value1 = zo.transitionNumberOfTargets.ToString();
                    break;
                case ModifierType.ArenaChange:
                    ArenaChange ac = modifier as ArenaChange;
                    value1 = ac.arena1;
                    value2 = ac.arena2;
                    option1 = ac.preload;
                    break;
                default:
                    break;
            }
            switch (type)
            {
                case ModifierType.AimAssist:
                    return new AimAssistChange(type.ToString(), startTick.tick, endTick.tick, amount);
                case ModifierType.ArenaBrightness:
                    return new ArenaBrightness(type.ToString(), startTick.tick, endTick.tick, amount, option1, option2);
                case ModifierType.ArenaChange:
                    return new ArenaChange(type.ToString(), startTick.tick, endTick.tick, value1, value2, option1);
                case ModifierType.ArenaRotation:
                    return new ArenaRotation(type.ToString(), startTick.tick, endTick.tick, amount, option1);
                case ModifierType.ColorChange:
                    return new ColorChange(type.ToString(), startTick.tick, endTick.tick, leftHandColor, rightHandColor);
                case ModifierType.ColorSwap:
                    return new 

            }
            */
        }

        private void Update()
        {
            if (!ModifierHandler.activated) return;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    CopySelectedModifiers();
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    PasteCopiedModifiers();
                }
            }
        }
    }
}

