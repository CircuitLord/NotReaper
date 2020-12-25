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
using NotReaper.UI;

namespace NotReaper.Modifier
{
    public class ModifierSelectionHandler : MonoBehaviour
    {
        public static ModifierSelectionHandler Instance = null;
        public Transform posGetter = null;
        public GameObject selectionBox;
        public static bool isPasting = false;


        private List<Modifier> selectedEntries = new List<Modifier>();
        private List<ModifierDTO> copiedEntries = new List<ModifierDTO>();

        private Camera main;
        private CopyMode mode = CopyMode.Copy;
        private Vector3 dragStartPos;
        private Renderer rend;

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
            rend = selectionBox.GetComponent<Renderer>();
            selectionBox.SetActive(false);
           
        }

        public void CleanUp()
        {
            selectedEntries.Clear();
            copiedEntries.Clear();
            mode = CopyMode.Copy;
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
            return list;
        }

        public void PasteCopiedModifiers()
        {
            if (copiedEntries.Count == 0) return;
            copiedEntries.Sort((mod1, mod2) => mod1.startTick.CompareTo(mod2.startTick));

            QNT_Timestamp newStartTick = Timeline.time;
            QNT_Timestamp firstTick = new QNT_Timestamp((ulong)copiedEntries.First().startTick);
            float tickOffset = newStartTick.tick - copiedEntries.First().startTick;
            posGetter.position = Vector3.zero;
            float positionOffset = posGetter.position.x - copiedEntries.First().startPosX;
            float miniOffset = MiniTimeline.Instance.GetXForTheBookmarkThingy() - copiedEntries.First().miniStartX;
            if (tickOffset == 0 && mode != CopyMode.Cut) return;
            isPasting = true;
            foreach(ModifierDTO dto in copiedEntries)
            {
                dto.startTick += tickOffset;
                dto.startPosX += positionOffset;
                dto.miniStartX += miniOffset;
                if (dto.endTick != 0)
                {
                    dto.endTick += tickOffset;
                    dto.endPosX += positionOffset;
                    dto.miniEndX += miniOffset;
                }
            }
               
            StartCoroutine(ModifierHandler.Instance.LoadModifiers(copiedEntries));
            isPasting = false;
            DeselectAllModifiers();
        }
        
        public void DeselectAllModifiers()
        {
            ModifierHandler.Instance.DropCurrentModifier();
            foreach (Modifier m in selectedEntries)
            {
                m.Select(false);
            }
            selectedEntries.Clear();
            if(mode == CopyMode.Cut) copiedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);            
        }

        public void SelectModifier(Modifier m, bool singleSelect)
        {
            ModifierHandler.Instance.DropCurrentModifier();
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
            ModifierHandler.Instance.HideWindow(!singleActive || !singleSelect);
            ModifierHandler.Instance.FillData(m, singleActive && singleSelect, selectedEntries.Count == 0);
        }

        private void SelectAll()
        {
            if (selectedEntries.Count > 0)
            {
                ModifierHandler.Instance.DropCurrentModifier();
                foreach (Modifier m in selectedEntries) m.Select(false);
            }
            foreach (Modifier m in ModifierHandler.Instance.modifiers)
            {
                selectedEntries.Add(m);
                m.Select(true);
            }
            ModifierHandler.Instance.HideWindow(true);
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
                if (Input.GetKeyDown(KeyCode.D))
                {
                    DeselectAllModifiers();
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    SelectAll();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    
                    dragStartPos = Timeline.timelineNotesStatic.InverseTransformPoint(main.ScreenToWorldPoint(Input.mousePosition));
                    selectionBox.transform.SetParent(Timeline.timelineNotesStatic);
                    Vector3 newPos = selectionBox.transform.localPosition;
                    newPos.x = dragStartPos.x;
                    selectionBox.transform.localPosition = newPos;
                    
                    
                   
                }
                if (Input.GetMouseButton(0))
                {
                    float sizeX = dragStartPos.x - Timeline.timelineNotesStatic.InverseTransformPoint(main.ScreenToWorldPoint(Input.mousePosition)).x;
                    if (Mathf.Abs(sizeX) > .2f)
                    {
                        if(!selectionBox.activeInHierarchy) selectionBox.SetActive(true);
                        Vector3 newPos = selectionBox.transform.localPosition;
                        
                        sizeX *= -1f;
                        selectionBox.transform.localScale = new Vector2(sizeX, selectionBox.transform.localScale.y);
                        newPos.x = dragStartPos.x + (sizeX / 2);
                        selectionBox.transform.localPosition = new Vector2(newPos.x, selectionBox.transform.localPosition.y);

                        foreach(Modifier m in ModifierHandler.Instance.modifiers)
                        {
                            if(m.transform.localPosition.x > rend.bounds.min.x && m.transform.localPosition.x < rend.bounds.max.x)
                            {
                                if (!selectedEntries.Contains(m))
                                {
                                    SelectModifier(m, false);
                                }
                            }
                            else
                            {
                                if (selectedEntries.Contains(m))
                                {
                                    SelectModifier(m, false);
                                }
                            }
                        }


                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    dragStartPos = Vector3.zero;
                    selectionBox.SetActive(false);
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftControl) && selectionBox.activeInHierarchy)
            {
                selectionBox.SetActive(false);
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

