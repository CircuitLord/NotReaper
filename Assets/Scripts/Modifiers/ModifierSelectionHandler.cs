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
using NotReaper.UserInput;

namespace NotReaper.Modifier
{
    public class ModifierSelectionHandler : MonoBehaviour
    {
        public static ModifierSelectionHandler Instance = null;
        public Transform posGetter = null;
        public GameObject selectionBox;
        public static bool isPasting = false;
        public List<Modifier> tempCopiedModifiers = new List<Modifier>();

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
            bool couldAdd = false;
            if(!ZOffsetBaker.baking) couldAdd = ModifierUndoRedo.Instance.AddAction(selectedEntries.ToList(), Action.Delete);
            for (int i = 0; i < selectedEntries.Count; i++)
            {
                selectedEntries[i].Delete();
            }
            selectedEntries.Clear();
            ModifierHandler.Instance.HideWindow(false);
            StartCoroutine(ModifierHandler.Instance.IUpdateLevels());
        }

        public void Restore(List<ModifierDTO> dtoList)
        {
            mode = CopyMode.Restore;
            copiedEntries.Clear();
            copiedEntries = dtoList.ToList();
            PasteCopiedModifiers();
        }

        public void CopySelectedModifiers()
        {
            mode = CopyMode.Copy;
            copiedEntries.Clear();
            copiedEntries = GetDTOList();
        }

        public void CutSelectedModifiers()
        {
            if (selectedEntries.Count == 0) return;
            mode = CopyMode.Cut;
            copiedEntries.Clear();
            copiedEntries = GetDTOList();
            ModifierUndoRedo.Instance.AddAction(selectedEntries.ToList(), Action.Delete);
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
            ModifierHandler.Instance.DropCurrentModifier();
            QNT_Timestamp newStartTick = Timeline.time;
            QNT_Timestamp firstTick = new QNT_Timestamp((ulong)copiedEntries.First().startTick);
            float tickOffset = newStartTick.tick - copiedEntries.First().startTick;
            posGetter.position = Vector3.zero;
            float positionOffset = posGetter.position.x - copiedEntries.First().startPosX;
            float miniOffset = MiniTimeline.Instance.GetXForTheBookmarkThingy() - copiedEntries.First().miniStartX;
            if (tickOffset == 0 && mode == CopyMode.Copy) return;
            isPasting = mode != CopyMode.Restore;
            if(mode != CopyMode.Restore)
            {
                foreach (ModifierDTO dto in copiedEntries)
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
            }
            StartCoroutine(ModifierHandler.Instance.LoadModifiers(copiedEntries, false, true));
            isPasting = false;
            DeselectAllModifiers();
            if (mode == CopyMode.Restore)
            {
                copiedEntries.Clear();
                mode = CopyMode.Copy;
            }
            else
            {
                ModifierUndoRedo.Instance.AddAction(tempCopiedModifiers.ToList(), Action.Create);
                tempCopiedModifiers.Clear();
            }
           
           
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
            ModifierHandler.Instance.FillData(null, false, true);
        }

        public void SelectModifier(Modifier m, bool singleSelect)
        {
            ModifierHandler.Instance.DropCurrentModifier();
            if (selectedEntries.Count == 0) singleSelect = true;
            if (selectedEntries.Contains(m))
            {
                if (singleSelect)
                {
                    bool reselect = selectedEntries.Count > 1;
                    DeselectAllModifiers();
                   
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
            if (selectedEntries.Count == 0) ModifierHandler.Instance.HideWindow(false);
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
                int layerMask = LayerMask.GetMask("Modifier");
                RaycastHit2D hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000000f, layerMask);
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
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    DeselectAllModifiers();
                }
                else
                {
                    layerMask = LayerMask.GetMask("Timeline");
                    hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000000f, layerMask);
                    if(hit.collider != null)
                    {
                        if (hit.transform.gameObject.layer == 14)
                        {
                            DeselectAllModifiers();
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                int layerMask = LayerMask.GetMask("Modifier");
                RaycastHit2D hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000000f, layerMask);
                if (hit.collider != null)
                {
                    Modifier m = hit.transform.GetComponent<ClickNotifier>().GetModifier();
                    DeselectAllModifiers();
                    selectedEntries.Add(m);
                    DeleteSelectedModifiers();
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
                if (Input.GetKeyDown(InputManager.undo) && !Input.GetKey(KeyCode.LeftShift))
                {
                    ModifierUndoRedo.Instance.Undo();
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(InputManager.redo))
                {
                    ModifierUndoRedo.Instance.Redo();
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
                        
                        for (int i = 0; i < ModifierHandler.Instance.modifiers.Count; i++)
                        {
                            if(ModifierHandler.Instance.modifiers[i].startMark.transform.position.x > rend.bounds.min.x && ModifierHandler.Instance.modifiers[i].startMark.transform.position.x < rend.bounds.max.x)
                            {
                                if (!selectedEntries.Contains(ModifierHandler.Instance.modifiers[i]))
                                {
                                    SelectModifier(ModifierHandler.Instance.modifiers[i], false);
                                }
                            }
                            else
                            {
                                if (selectedEntries.Contains(ModifierHandler.Instance.modifiers[i]))
                                {
                                    SelectModifier(ModifierHandler.Instance.modifiers[i], false);
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
            Cut,
            Restore
        }
    }
}

