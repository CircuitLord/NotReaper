using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NotReaper;
using NotReaper.UI;
using NotReaper.Timing;
using NotReaper.Targets;
using System;
using NotReaper.Modifier;
using NotReaper.UserInput;

namespace NotReaper.Modifier
{
    public class ModifierHandler : MonoBehaviour
    {
        public static ModifierHandler Instance = null;
        public static bool inputFocused = false;
        public static bool activated;
        public static bool isLoading = false;
        [HideInInspector] public bool isHovering;

        [Header("References")]
        [SerializeField] private GameObject modifierWindow;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private GameObject amountSlider;
        [SerializeField] private GameObject colorPicker;
        [SerializeField] private GameObject value1;
        [SerializeField] private GameObject value2;
        [SerializeField] private GameObject option1;
        [SerializeField] private GameObject option2;
        [SerializeField] private GameObject startTickButton;
        [SerializeField] private GameObject endTickButton;
        [SerializeField] private GameObject createModifierButton;
        [SerializeField] private GameObject modifierPrefab;
        [SerializeField] private Transform leftMax;
        [SerializeField] private Transform rightMax;

        public List<Modifier> modifiers = new List<Modifier>();
        private Modifier currentModifier;


        private LabelSetter slider;
        private Vector3 activatePosition = new Vector3(290.6859f, -36.6f, 0f);
        private bool init = false;
        private bool skipRefresh = false;
        private bool isHidden = false;
        private bool parentSet = false;

        public void Start()
        {
            if (Instance is null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("Trying to create another Modifier instance.");
                return;
            }
            value1.SetActive(false);
            value2.SetActive(false);
            option1.SetActive(false);
            option2.SetActive(false);
            colorPicker.SetActive(false);
            slider = amountSlider.GetComponent<LabelSetter>();
            
        }

        public List<Modifier> GetZOffsetModifiers()
        {
            List<Modifier> list = new List<Modifier>();
            foreach (Modifier m in modifiers) if (m.modifierType == ModifierType.zOffset) list.Add(m);
            return list;
        }

        public void OptimizeModifiers()
        {
            foreach(Modifier m in modifiers)
            {
                m.Optimize(false);
                if (m.endMarkExists)
                {
                    if (m.endMark.transform.position.x > leftMax.position.x && m.startMark.transform.position.x < rightMax.position.x)
                    {
                        m.Optimize(true);
                    }
                       
                }
                else if (m.startMark.transform.position.x > leftMax.position.x && m.startMark.transform.position.x < rightMax.position.x)
                {
                    m.Optimize(true);
                }
                   
            }
        }

        public void DropCurrentModifier()
        {
            if (currentModifier is null) return;
            if (!currentModifier.isCreated) return;
            modifiers.Add(currentModifier);
            currentModifier = null;
        }

        public void CleanUp()
        {
            for(int i = 0; i < modifiers.Count; i++)
            {
                modifiers[i].Delete();
            }
            modifiers.Clear();
            currentModifier = null;
            ModifierSelectionHandler.Instance.CleanUp();
            GameObject[] stubbornModifiers = GameObject.FindGameObjectsWithTag("Modifier");
            for(int i = 0; i < stubbornModifiers.Length; i++)
            {
                GameObject.Destroy(stubbornModifiers[i]);
            }
        }

        public void OnInputFocusChange(string _)
        {
            inputFocused = !inputFocused;
        }

        public void OnButtonClicked()
        {
            EditorInput.I.SelectTool(EditorTool.ModifierCreator);
        }

        public void HideWindow(bool hide)
        {
            isHidden = hide;
            modifierWindow.SetActive(!hide);
        }

        public void Activate(bool activate)
        {
            if (!init && !activate) return;
            activated = activate;
            if (activate)
            {
                if (MiniTimeline.Instance != null)
                {
                    ShowModifiers(true);
                    Timeline.OptimizeInvisibleTargets();
                }
                modifierWindow.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
                modifierWindow.SetActive(isHidden ? false : true);
                modifierWindow.transform.localPosition = activatePosition;
                if (!init)
                {
                    OnDropdownValueChanged();
                    init = true;
                }
            }
            else
            {
                if (MiniTimeline.Instance != null)
                {
                    ShowModifiers(false);
                    Timeline.OptimizeInvisibleTargets();
                }
                modifierWindow.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
                if (!init)
                {
                    OnDropdownValueChanged();
                    init = true;
                }
                else
                {
                    activatePosition = modifierWindow.transform.localPosition;
                }
                modifierWindow.SetActive(false);
            }
        }

        public void ShowModifiers(bool show)
        {
            foreach (Modifier m in modifiers) m.Show(show);
        }

        public IEnumerator IUpdateLevels()
        {
            if (ModifierUndoRedo.undoRedoActive) yield break;
            modifiers.Sort((mod1, mod2) => mod1.startTime.tick.CompareTo(mod2.startTime.tick));
            foreach (Modifier m in modifiers)
            {
                m.UpdateLevel();
                yield return new WaitForSeconds(.01f);
            }                          
        }

        public IEnumerator LoadModifiers(List<ModifierDTO> modList, bool fromLoad = false, bool fromAction = false)
        {
            if (currentModifier != null)
            {
                CreateModifier();
            }
            foreach (ModifierDTO dto in modList)
            {
                Modifier m = Instantiate(modifierPrefab).GetComponent<Modifier>();
                m.LoadFromDTO(dto);
                m.shorthand = GetShorthand(m.modifierType);
                LoadModifier(m);
            }
            ModifierSelectionHandler.isPasting = false;
            yield return new WaitForSeconds(.001f);
            ShowModifiers(activated);
            isLoading = false;
        }

        public void LoadModifier(Modifier modifier)
        {
            currentModifier = modifier;
            SetStartTick(modifier.startTime);
            SetEndTick(modifier.endTime);
            CreateModifier(false);
        }


        public List<ModifierDTO> MapToDTO()
        {
            Modifier current = null;
            if (currentModifier != null && currentModifier.isSelected) current = currentModifier;
            ModifierSelectionHandler.Instance.DeselectAllModifiers();
            DropCurrentModifier();
            List<ModifierDTO> dtoList = new List<ModifierDTO>();
            foreach(Modifier m in modifiers)
            {
                dtoList.Add(m.GetDTO());
            }
            if (current != null) ModifierSelectionHandler.Instance.SelectModifier(current, true);
            return dtoList;
            
        }

        public void CreateModifier(bool save = false)
        {
            if (currentModifier is null) return;
            if (!currentModifier.startSet) return;
            if (!CanCreateModifier(currentModifier.modifierType, currentModifier.startTime)) return;
            currentModifier.CreateModifier(save);
            modifiers.Add(currentModifier);
            List<Modifier> lmo = new List<Modifier>();
            lmo.Add(currentModifier);
            if (!save && !isLoading && !ModifierSelectionHandler.isPasting) ModifierUndoRedo.Instance.AddAction(lmo, Action.Create);
            if(ModifierUndoRedo.recreating) ModifierUndoRedo.Instance.recreatedModifiers.Add(currentModifier);
            if (ModifierSelectionHandler.isPasting) ModifierSelectionHandler.Instance.tempCopiedModifiers.Add(currentModifier);
            currentModifier = null;
            OnDropdownValueChanged();           
        }

        public bool CanCreateModifier(ModifierType type, QNT_Timestamp tick)
        {
            if(type == ModifierType.Speed || type == ModifierType.Fader)
            {
                if (currentModifier.endTime.tick == 0) return false;
            }
            if (type != ModifierType.ColorUpdate && type != ModifierType.PsychedeliaUpdate) return true;
            foreach (Modifier m in modifiers)
            {
                if (m.startTime < tick && m.endTime > tick)
                {
                    if (m.modifierType == ModifierType.ColorChange && type == ModifierType.ColorUpdate)
                    {
                        return true;
                    }
                    else if (m.modifierType == ModifierType.Psychedelia && type == ModifierType.PsychedeliaUpdate) return true;
                }
                else if (m.endTime.tick == 0)
                {
                    if (m.modifierType == ModifierType.ColorChange && type == ModifierType.ColorUpdate) return true;
                    else if (m.modifierType == ModifierType.Psychedelia && type == ModifierType.PsychedeliaUpdate) return true;
                }
            }
            return false;
        }

        public bool IsDropdownExpanded()
        {
            return dropdown.IsExpanded;
        }
        
        public void FillData(Modifier modifier, bool shouldFill, bool isEmpty)
        {
             if (!shouldFill || isEmpty)
             {
                if (currentModifier != null)
                {
                    CreateModifier(true);
                }
                else
                {
                    OnDropdownValueChanged();
                }                
                 return;
             }

            int modType = (int)modifier.modifierType;
            skipRefresh = dropdown.value != modType;
            currentModifier = modifier;
            modifiers.Remove(modifier);
            dropdown.value = modType;

            amountSlider.GetComponent<LabelSetter>().SetSliderValue(currentModifier.amount);
            value1.GetComponent<LabelSetter>().SetInputText(currentModifier.value1);
            value2.GetComponent<LabelSetter>().SetInputText(currentModifier.value2);
            option1.GetComponent<LabelSetter>().SetToggleState(currentModifier.option1);
            option2.GetComponent<LabelSetter>().SetToggleState(currentModifier.option2);
            startTickButton.GetComponent<LabelSetter>().SetLabelText(currentModifier.startTime.tick.ToString());
            endTickButton.GetComponent<LabelSetter>().SetLabelText(currentModifier.endTime.tick.ToString());
            createModifierButton.GetComponent<LabelSetter>().SetLabelText("Update Modifier");
            colorPicker.GetComponent<LabelSetter>().SetColorSliderLeft(currentModifier.leftHandColor);
            colorPicker.GetComponent<LabelSetter>().SetColorSliderRight(currentModifier.rightHandColor);
        }

        private void SetStartTick(QNT_Timestamp tick)
        {
            currentModifier.startSet = true;
            currentModifier.CreateModifierMark(true, tick, true);
        }
        private void SetEndTick(QNT_Timestamp tick)
        {
            SetEndTick(tick.tick);
        }

        public void SetStartTick()
        {
            
            InitializeModifier();
            ulong tick = Timeline.time.tick;
            currentModifier.startTime = new QNT_Timestamp(tick);
            if (tick != 0 && tick >= currentModifier.endTime.tick && currentModifier.endTime.tick != 0)
            {
                UpdateEndTick(tick);
                startTickButton.GetComponent<LabelSetter>().SetLabelText(tick.ToString());
                currentModifier.UpdateMark(Modifier.UpdateType.UpdateStart, tick);
            }
            else
            {
                startTickButton.GetComponent<LabelSetter>().SetLabelText(tick.ToString());
                if (currentModifier.startSet)
                {
                    UpdateEndTick(tick);
                    currentModifier.UpdateMark(Modifier.UpdateType.MoveStart, tick);
                }
                else
                {
                    UpdateEndTick(tick);
                    currentModifier.CreateModifierMark(true, currentModifier.startTime, false);
                }

            }
            currentModifier.startSet = true;           
        }

        private void UpdateEndTick(float tick)
        {
            InitializeModifier();
            if (!currentModifier.startSet) return;
            if (!currentModifier.endMarkExists && currentModifier.endTime.tick != 0)
            {
                endTickButton.GetComponent<LabelSetter>().SetLabelText(tick.ToString());
                currentModifier.endTime = currentModifier.startTime;
            }
        }

        public void SetEndTick(float loadTick = -1f)
        {
            InitializeModifier();
            if (!currentModifier.startSet) return;
            ulong tick = Timeline.time.tick;
            if (loadTick != -1f) tick = (ulong)loadTick;
            if (tick != 0 && tick <= currentModifier.startTime.tick)
            {
                tick = currentModifier.startTime.tick;
                currentModifier.endTime = new QNT_Timestamp(tick);
                endTickButton.GetComponent<LabelSetter>().SetLabelText(tick.ToString());
                currentModifier.UpdateMark(Modifier.UpdateType.UpdateEnd);
            }
            else if (tick == 0)
            {
                currentModifier.endTime = new QNT_Timestamp(tick);
                endTickButton.GetComponent<LabelSetter>().SetLabelText(tick.ToString());
                currentModifier.UpdateMark(Modifier.UpdateType.UpdateEnd);
            }
            else
            {
                currentModifier.endTime = new QNT_Timestamp(tick);
                endTickButton.GetComponent<LabelSetter>().SetLabelText(tick.ToString());
                currentModifier.CreateModifierMark(false, currentModifier.endTime, loadTick != -1f);
            }
        }
        public void Scale(float targetScale)
        {
            foreach (Modifier m in modifiers) m.Scale(targetScale);
            if (currentModifier != null) currentModifier.Scale(targetScale);
        }

        public void OnValue1Changed()
        {
            InitializeModifier();
            currentModifier.value1 = value1.GetComponent<LabelSetter>().GetText();
        }

        public void OnValue2Changed()
        {
            InitializeModifier();
            currentModifier.value2 = value2.GetComponent<LabelSetter>().GetText();
        }

        public void OnOption1Changed()
        {
            InitializeModifier();
            currentModifier.option1 = option1.GetComponent<LabelSetter>().GetToggleState();
            SetHintText(currentModifier.modifierType);
        }

        public void OnOption2Changed()
        {
            InitializeModifier();
            currentModifier.option2 = option2.GetComponent<LabelSetter>().GetToggleState();
            SetHintText(currentModifier.modifierType);
        }

        public void OnAmountChanged()
        {
            InitializeModifier();
            currentModifier.amount = amountSlider.GetComponentInChildren<Slider>().value;
        }

        public void OnLeftColorChanged()
        {
            InitializeModifier();
            currentModifier.leftHandColor = colorPicker.GetComponent<LabelSetter>().GetLeftColor();
        }

        public void OnRightColorChanged()
        {
            InitializeModifier();
            currentModifier.rightHandColor = colorPicker.GetComponent<LabelSetter>().GetRightColor();
        }

        public void DeleteModifier()
        {
            ModifierSelectionHandler.Instance.DeleteSelectedModifiers();
            currentModifier = null;
            OnDropdownValueChanged();
        }

        public void OnDeleteButtonClicked()
        {
            if (currentModifier != null)
            {
                if (currentModifier.isCreated)
                {
                    DeleteModifier();
                }
                else
                {
                    currentModifier.Delete();
                    currentModifier = null;
                    OnDropdownValueChanged();
                }
               
            }
        }

        public void OnDropdownValueChanged()
        {
            if (!skipRefresh) ResetCurrentData();
           
            ModifierType type = (ModifierType)dropdown.value;
            switch (type)
            {
                case ModifierType.AimAssist:
                case ModifierType.Fader:
                case ModifierType.Particles:
                case ModifierType.Psychedelia:
                case ModifierType.Speed:
                    amountSlider.SetActive(true);
                    endTickButton.SetActive(true);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    colorPicker.SetActive(false);
                    break;
                case ModifierType.ArenaBrightness:
                    amountSlider.SetActive(true);
                    endTickButton.SetActive(false);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    colorPicker.SetActive(false);
                    option1.GetComponentInChildren<LabelSetter>().SetLabelText("Continuous");
                    option2.GetComponentInChildren<LabelSetter>().SetLabelText("Strobo");
                    option1.SetActive(true);
                    option2.SetActive(true);
                    break;
                case ModifierType.ArenaRotation:
                    amountSlider.SetActive(true);
                    endTickButton.SetActive(false);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    option2.SetActive(false);
                    colorPicker.SetActive(false);
                    option1.GetComponentInChildren<LabelSetter>().SetLabelText("Continuous");
                    option2.GetComponentInChildren<LabelSetter>().SetLabelText("Incremental");
                    option1.SetActive(true);
                    option2.SetActive(true);
                    break;
                case ModifierType.ColorChange:
                    amountSlider.SetActive(false);
                    endTickButton.SetActive(true);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    colorPicker.SetActive(true);
                    break;
                case ModifierType.ColorUpdate:
                    amountSlider.SetActive(false);
                    endTickButton.SetActive(false);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    colorPicker.SetActive(true);
                    break;
                case ModifierType.PsychedeliaUpdate:
                    amountSlider.SetActive(true);
                    endTickButton.SetActive(false);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    colorPicker.SetActive(false);
                    break;
                case ModifierType.ColorSwap:
                case ModifierType.HiddenTelegraphs:
                case ModifierType.InvisibleGuns:
                    endTickButton.SetActive(true);
                    amountSlider.SetActive(false);
                    value1.SetActive(false);
                    value2.SetActive(false);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    colorPicker.SetActive(false);
                    break;
                case ModifierType.zOffset:
                    amountSlider.SetActive(true);
                    endTickButton.SetActive(true);
                    value2.SetActive(false);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    value1.GetComponent<LabelSetter>().SetLabelText("Transition Target Amount");
                    value1.SetActive(true);
                    colorPicker.SetActive(false);
                    break;
                case ModifierType.ArenaChange:
                    amountSlider.SetActive(false);
                    endTickButton.SetActive(false);
                    value1.GetComponent<LabelSetter>().SetLabelText("Arena Option 1");
                    value2.GetComponent<LabelSetter>().SetLabelText("Arena Option 2");
                    option1.GetComponent<LabelSetter>().SetLabelText("Preload");
                    value1.SetActive(true);
                    value2.SetActive(true);
                    option1.SetActive(true);
                    option2.SetActive(false);
                    colorPicker.SetActive(false);
                    break;
                case ModifierType.OverlaySetter:
                    amountSlider.SetActive(false);
                    endTickButton.SetActive(false);
                    value1.GetComponent<LabelSetter>().SetLabelText("Song Info");
                    value2.GetComponent<LabelSetter>().SetLabelText("Mapper");
                    value1.SetActive(true);
                    value2.SetActive(true);
                    option1.SetActive(false);
                    option2.SetActive(false);
                    colorPicker.SetActive(false);
                    break;
            }
            SetHintText(type);
            SetMinMax(type);
            if (!skipRefresh) createModifierButton.GetComponent<LabelSetter>().SetLabelText("Create Modifier");
           
            skipRefresh = false;
        }

        private void PickLastUsedColor()
        {
            if (currentModifier.modifierType != ModifierType.ColorChange && currentModifier.modifierType != ModifierType.ColorUpdate) return;
            Modifier closestColorModifier = null;
            foreach(Modifier m in modifiers)
            {
                if (m.modifierType != ModifierType.ColorChange && m.modifierType != ModifierType.ColorUpdate) continue;
                if (m.startTime >= Timeline.time) continue;
                if(closestColorModifier is null)
                {
                    closestColorModifier = m;
                    continue;
                }
                if (closestColorModifier.startTime < m.startTime) closestColorModifier = m;
            }
            if(closestColorModifier != null)
            {
                currentModifier.leftHandColor = closestColorModifier.leftHandColor;
                currentModifier.rightHandColor = closestColorModifier.rightHandColor;
                colorPicker.GetComponent<LabelSetter>().SetColorSliderLeft(currentModifier.leftHandColor);
                colorPicker.GetComponent<LabelSetter>().SetColorSliderRight(currentModifier.rightHandColor);
            }
            else
            {
                currentModifier.leftHandColor = colorPicker.GetComponent<LabelSetter>().GetLeftColor();
                currentModifier.rightHandColor = colorPicker.GetComponent<LabelSetter>().GetRightColor();
            }
        }

        private void SetHintText(ModifierType type)
        {
            string text;
            switch (type)
            {
                case ModifierType.AimAssist:
                case ModifierType.Particles:
                case ModifierType.Speed:
                    text = "Default: 100";
                    break;
                case ModifierType.ArenaBrightness:
                    if(currentModifier != null)
                    {
                        if (currentModifier.option2)
                        {
                            text = "Amount represents flashes per beat (1/4 note)";
                            slider.SetMinValue(1f);
                            slider.SetMaxValue(128f);
                            endTickButton.SetActive(true);
                            break;
                        }
                        if (currentModifier.option1)
                        {
                            SetMinMax(type);
                            text = "Amount represents speed";
                            endTickButton.SetActive(true);
                            break;
                        }
                    }
                    SetMinMax(type);
                    endTickButton.SetActive(false);
                    text = "Default: 100";
                    break;
                case ModifierType.ArenaRotation:
                    if(currentModifier != null)
                    {
                        if (currentModifier.option2)
                        {
                            text = "Amount represents speed at end tick";
                            endTickButton.SetActive(true);
                            break;
                        }
                        if (currentModifier.option1)
                        {
                            text = "Amount represents rotation speed";
                            endTickButton.SetActive(true);
                            break;
                        }
                    }
                    endTickButton.SetActive(false);
                    text = "Default: 0";
                    break;
                case ModifierType.Psychedelia:
                case ModifierType.PsychedeliaUpdate:
                    text = "Amount represents cycle speed";
                    break;
                case ModifierType.zOffset:
                    text = "Default: 0";
                    break;
                case ModifierType.Fader:
                    text = "Fades from current brightness to amount";
                    break;
                default:
                    text = "";
                    break;
            }                   
            amountSlider.GetComponent<LabelSetter>().SetHintText(text);
        }

        private void InitializeModifier()
        {            
            if (currentModifier != null) return;
                
            ModifierType type = (ModifierType)dropdown.value;
            string shorthand = GetShorthand(type);
            currentModifier = Instantiate(modifierPrefab).GetComponent<Modifier>();
            currentModifier.modifierType = type;
            currentModifier.shorthand = shorthand;
            PickLastUsedColor();
        }

        private string GetShorthand(ModifierType type)
        {
            string sh = "";
            switch (type)
            {
                case ModifierType.AimAssist:
                    sh = "AA";
                    break;
                case ModifierType.ArenaBrightness:
                    sh = "AB";
                    break;
                case ModifierType.ArenaRotation:
                    sh = "AR";
                    break;
                case ModifierType.ColorChange:
                    sh = "CC";
                    break;
                case ModifierType.ColorSwap:
                    sh = "CS";
                    break;
                case ModifierType.ColorUpdate:
                    sh = "CU";
                    break;
                case ModifierType.Fader:
                    sh = "FA";
                    break;
                case ModifierType.HiddenTelegraphs:
                    sh = "HT";
                    break;
                case ModifierType.InvisibleGuns:
                    sh = "IG";
                    break;
                case ModifierType.Particles:
                    sh = "PA";
                    break;
                case ModifierType.Psychedelia:
                    sh = "PS";
                    break;
                case ModifierType.PsychedeliaUpdate:
                    sh = "PU";
                    break;
                case ModifierType.Speed:
                    sh = "SP";
                    break;
                case ModifierType.zOffset:
                    sh = "ZO";
                    break;
                case ModifierType.ArenaChange:
                    sh = "AC";
                    break;
                case ModifierType.OverlaySetter:
                    sh = "OS";
                    break;
                default:
                    break;
            }
            return sh;
        }

        private void SetMinMax(ModifierType type)
        {
            switch (type)
            {
                case ModifierType.AimAssist:
                case ModifierType.ArenaBrightness:
                    slider.SetMinValue(0f);
                    slider.SetMaxValue(100f);
                    break;
                case ModifierType.ArenaRotation:
                    slider.SetMinValue(-500f);
                    slider.SetMaxValue(500f);
                    break;
                case ModifierType.Fader:
                    slider.SetMinValue(0f);
                    slider.SetMaxValue(100f);
                    break;
                case ModifierType.Particles:
                    slider.SetMinValue(0f);
                    slider.SetMaxValue(1000f);
                    break;
                case ModifierType.Psychedelia:
                case ModifierType.PsychedeliaUpdate:
                    slider.SetMinValue(0f);
                    slider.SetMaxValue(10000f);
                    break;
                case ModifierType.Speed:
                    slider.SetMinValue(0f);
                    slider.SetMaxValue(200f);
                    break;
                case ModifierType.zOffset:
                    slider.SetMinValue(-100f);
                    slider.SetMaxValue(500f);
                    break;
                case ModifierType.ColorChange:
                case ModifierType.ColorUpdate:
                    colorPicker.GetComponent<LabelSetter>().SetMinMaxColorSliders(0f, 1f);
                    break;
                default:
                    break;
            }
        }

        private void ResetCurrentData()
        {
            if(currentModifier != null)
            {
                if (!currentModifier.isCreated)
                {
                    currentModifier.Delete();
                }
                else
                {
                    CreateModifier();
                }
                   
                currentModifier = null;
            }
            amountSlider.GetComponent<LabelSetter>().SetSliderValue(0f);
            startTickButton.GetComponent<LabelSetter>().SetLabelText("0");
            endTickButton.GetComponent<LabelSetter>().SetLabelText("0");
            value1.GetComponent<LabelSetter>().SetInputText("");
            value2.GetComponent<LabelSetter>().SetInputText("");
            option1.GetComponent<LabelSetter>().SetToggleState(false);
            option2.GetComponent<LabelSetter>().SetToggleState(false);
        }

        public enum ModifierType { AimAssist = 0, ColorChange = 1, ColorUpdate = 2, ColorSwap = 3, HiddenTelegraphs = 4, InvisibleGuns = 5, Particles = 6, Psychedelia = 7, PsychedeliaUpdate = 8, Speed = 9, zOffset = 10, ArenaRotation = 11, ArenaBrightness = 12, ArenaChange = 13, Fader = 14, OverlaySetter = 15 }

    }
}



