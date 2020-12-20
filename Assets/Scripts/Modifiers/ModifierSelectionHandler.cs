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
        public static ModifierSelectionHandler Instance = null;
        private List<Modifier> selectedEntries = new List<Modifier>();

        private List<ModifierDTO> copiedEntries = new List<ModifierDTO>();

        public static bool isPasting = false;

        public Transform posGetter = null;

        private Camera main;
        private CopyMode mode = CopyMode.Copy;

        private void Start()
        {
            if (Instance is null) Instance = this;
            else
            {
                Debug.LogWarning("Trying to create a second ModifierSelectionhandler instance.");
                return;
            }

            posGetter = GameObject.Instantiate(new GameObject("PosGetter").transform);
            posGetter.SetParent(Timeline.timelineNotesStatic);
            main = Camera.main;
            
        }
        public void AddEntry(Modifier entry)
        {
            ModifierHandler.Instance.HideWindow(true);
            if (selectedEntries.Contains(entry))
            {
                RemoveEntry(entry);
                return;
            }
            //entry.Select(true);
            selectedEntries.Add(entry);
            
        }

        public void RemoveEntry(Modifier entry)
        {
            if (!selectedEntries.Contains(entry)) return;
            //entry.Select(false);
            selectedEntries.Remove(entry);

            if(selectedEntries.Count == 0) ModifierHandler.Instance.HideWindow(false);
        }

        public void RemoveSelectedEntry(TimelineEntry entry)
        {
            //if (entry == selectedEntry) selectedEntry = null;
        }

        public void RemoveAllEntries()
        {
            //foreach (TimelineEntry entry in selectedEntries) entry.Select(false);
            selectedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);
        }

        public void DeleteSelectedModifiers()
        {
            for(int i = 0; i < selectedEntries.Count; i++)
            {
                selectedEntries[i].Delete();
            }
            selectedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);
            StartCoroutine(ModifierHandler.Instance.IUpdateLevels());
        }

        public void CopySelectedModifiers()
        {
            mode = CopyMode.Copy;
            copiedEntries.Clear();
            //copiedEntries = selectedEntries.ToList();
            copiedEntries = GetDTOList();
        }

        public void CutSelectedModifiers()
        {
            mode = CopyMode.Cut;
            copiedEntries.Clear();
            copiedEntries = GetDTOList();
            for (int i = 0; i < selectedEntries.Count; i++)
            {
                selectedEntries[i].Delete();
            }
            selectedEntries.Clear();

            StartCoroutine(ModifierHandler.Instance.IUpdateLevels());
        }

        private List<ModifierDTO> GetDTOList()
        {
            List<ModifierDTO> list = new List<ModifierDTO>();
            foreach (Modifier m in selectedEntries)
            {
               list.Add(m.GetDTO());
            }
            return list;//CloneList(list);
        }

        public void PasteCopiedModifiers()
        {
            copiedEntries.Sort((mod1, mod2) => mod1.startTick.CompareTo(mod2.startTick));

            QNT_Timestamp newStartTick = Timeline.time;
            QNT_Timestamp firstTick = new QNT_Timestamp((ulong)copiedEntries.First().startTick);
            float tickOffsetForward = newStartTick.tick - copiedEntries.First().startTick;
            float tickOffsetInversed = copiedEntries.First().startTick - newStartTick.tick;
            float tickOffset = (newStartTick >= firstTick ? tickOffsetForward : tickOffsetInversed);
            posGetter.position = Vector3.zero;
            float positionOffset = posGetter.position.x - copiedEntries.First().startPosX;
            if (tickOffset == 0 && mode != CopyMode.Cut) return;

            isPasting = true;
            foreach(ModifierDTO dto in copiedEntries)
            {
                dto.startTick += tickOffset;
                dto.startPosX += positionOffset;
                if (dto.endTick != 0)
                {
                    dto.endTick += tickOffset;
                    dto.endPosX += positionOffset;
                }
            }
               
            StartCoroutine(ModifierHandler.Instance.LoadModifiers(copiedEntries));
            isPasting = false;
            DeselectAllModifiers();
        }

        private List<ModifierDTO> CloneList<ModifierDTO>(List<ModifierDTO> oldList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<ModifierDTO>)formatter.Deserialize(stream);
        }

        private Modifier DeepCopyModifier(Modifier modifier)
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

        public void DeselectAllModifiers()
        {
            foreach(Modifier m in selectedEntries)
            {
                m.Select(false);
            }
            selectedEntries.Clear();
            copiedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);
        }

        public void SelectModifier(Modifier m, bool singleSelect)
        {
          
            if (selectedEntries.Contains(m))
            {
                if (singleSelect)
                {
                    DeselectAllModifiers();
                    bool reselect = selectedEntries.Count > 1;
                    if (reselect)
                    {
                        selectedEntries.Add(m);
                        m.Select(true);
                    }
                }
                else
                {
                    selectedEntries.Remove(m);
                    m.Select(false);
                }                
            }
            else
            {
                if (singleSelect)
                {
                    foreach (Modifier mod in selectedEntries) mod.Select(false);
                    selectedEntries.Clear();
                    selectedEntries.Add(m);
                    m.Select(true);
                }
                else
                {
                    selectedEntries.Add(m);
                    m.Select(true);
                }                
            }
            bool singleActive = selectedEntries.Count < 2;
            ModifierHandler.Instance.HideWindow(!singleActive);
            ModifierHandler.Instance.FillData(m, singleActive, selectedEntries.Count == 0);
        }

        private void Update()
        {
            if (!ModifierHandler.activated) return;
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000000f, 1 << LayerMask.NameToLayer("Modifier"));

                if (hit.collider != null)
                {
                    Modifier m = hit.transform.GetComponent<ClickNotifier>().GetModifier();

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        SelectModifier(m, false);
                    }
                    else
                    {
                        SelectModifier(m, true);
                    }
                    
                    
                   
                }
            }            
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
                if (Input.GetKeyDown(KeyCode.X))
                {
                    CutSelectedModifiers();
                }
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                ModifierHandler.Instance.DeleteModifier();
            }
            
        }

        private enum CopyMode
        {
            Copy,
            Cut
        }
    }
}

