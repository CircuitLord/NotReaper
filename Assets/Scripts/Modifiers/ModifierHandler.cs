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

public class ModifierHandler : MonoBehaviour
{
    public static ModifierHandler instance = null;

    [HideInInspector] public bool activated;
    [HideInInspector] public bool isHovering;

    [Header("References")]
    [SerializeField] private GameObject modifierWindow;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject amountSlider;
    [SerializeField] private GameObject value1;
    [SerializeField] private GameObject value2;
    [SerializeField] private GameObject option1;
    [SerializeField] private GameObject option2;
    [SerializeField] private GameObject startTickButton;
    [SerializeField] private GameObject endTickButton;
    [SerializeField] private GameObject createModifierButton;

    public List<ModifierData> modifierData = new List<ModifierData>();
    public List<Modifier> modifiers = new List<Modifier>();
    private ModifierData currentData;
    

    private LabelSetter slider;
    private Vector3 activatePosition = new Vector3(292.1306f, -54.84449f, 1f);

    public void Start()
    {
        if(instance is null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Trying to create another Modifier instance.");
            return;
        }

        currentData = new ModifierData();
        currentData.type = ModifierType.AimAssist;
        currentData.shorthand = "AA";
        value1.SetActive(false);
        value2.SetActive(false);
        option1.SetActive(false);
        option2.SetActive(false);
        slider = amountSlider.transform.parent.GetComponent<LabelSetter>();
    }
    public void Activate(bool activate)
    {
        activated = activate;
        if (activate)
        {
            if(MiniTimeline.Instance != null)
            {
                ModifierTimeline.Instance.ShowModifiers(true);
                Timeline.OptimizeInvisibleTargets();
            }
            
            modifierWindow.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
            modifierWindow.SetActive(true);
            modifierWindow.transform.localPosition = activatePosition;
        }
        else
        {
            if (MiniTimeline.Instance != null)
            {
                ModifierTimeline.Instance.ShowModifiers(false);
                Timeline.OptimizeInvisibleTargets();
            }
            modifierWindow.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
            modifierWindow.transform.localPosition = new Vector3(-2700, 0f, 0f);
            modifierWindow.SetActive(false);
        }
    }

    public void CreateModifier()
    {
        ModifierTimeline.Instance.CreateModifier(currentData);
        modifierData.Add(currentData);
        Modifier m = CreateModifierType();
        modifiers.Add(m);
        Timeline.audicaFile.modifiers.modifiers = modifiers;
        currentData = new ModifierData();
    }

    private Modifier CreateModifierType()
    {
        Modifier m = null;
        float st = currentData.startTick.tick;
        float et = currentData.endTick.tick;
        float amount = currentData.amount;
        string type = currentData.type.ToString();
        string val1 = currentData.value1;
        string val2 = currentData.value2;
        bool opt1 = currentData.option1;
        bool opt2 = currentData.option2;

        switch (currentData.type)
        {
            case ModifierType.AimAssist:
                m = new AimAssistChange(type, st, et, amount);
                break;
            case ModifierType.ArenaBrightness:
                m = new ArenaBrightness(type, st, et, amount, opt1, opt2);             
                break;
            case ModifierType.ArenaRotation:
                m = new ArenaRotation(type, st, et, amount, opt1);
                break;
            case ModifierType.ColorChange:
                m = new ColorChange(type, st, currentData.value1, currentData.value2);
                break;
            case ModifierType.ColorSwap:
                m = new ColorSwap(type, st, et);
                break;
            case ModifierType.ColorUpdate:
                m = new ColorUpdate(type, st, currentData.value1, currentData.value2);
                break;
            case ModifierType.Fader:
                m = new Fader(type, st, et, amount);
                break;
            case ModifierType.HiddenTelegraphs:
                m = new HiddenTelegraphs(type, st, et);
                break;
            case ModifierType.InvisibleGuns:
                m = new InvisibleGuns(type, st, et);
                break;
            case ModifierType.Particles:
                m = new Particles(type, st, et, amount);
                break;
            case ModifierType.Psychedelia:
                m = new Psychedelia(type, st, et, amount);
                break;
            case ModifierType.PsychedeliaUpdate:
                m = new PsychedeliaUpdate(type, st, amount);
                break;
            case ModifierType.Speed:
                m = new SpeedChange(type, st, et, amount);
                break;
            case ModifierType.zOffset:
                float f = 0;
                float.TryParse(val1, out f);
                m = new ZOffset(type, st, et, amount, f);
                break;
        }
        return m;
    }

    public void SetStartTick()
    {
        ulong tick = Timeline.time.tick;

        if (tick != 0 && tick > currentData.endTick.tick && currentData.endTick.tick != 0)
        {
            currentData.startTick = new QNT_Timestamp(tick);
            startTickButton.GetComponent<LabelSetter>().SetText(tick.ToString());
            ModifierTimeline.Instance.UpdateMark(ModifierTimeline.UpdateType.UpdateStart, tick);
        }
        else
        {
            currentData.startTick = new QNT_Timestamp(tick);
            startTickButton.GetComponent<LabelSetter>().SetText(tick.ToString());
            ModifierTimeline.Instance.SetModifierMark(currentData.type, currentData.startTick, currentData.shorthand, true);
        }       
    }

    public void SetEndTick()
    {
        ulong tick = Timeline.time.tick;
        
        if (tick != 0 && tick < currentData.startTick.tick)
        {
            tick = currentData.startTick.tick;
            currentData.endTick = new QNT_Timestamp(tick);
            endTickButton.GetComponent<LabelSetter>().SetText(tick.ToString());
            ModifierTimeline.Instance.UpdateMark(ModifierTimeline.UpdateType.UpdateEnd);
        }
        else if(tick == 0)
        {
            currentData.endTick = new QNT_Timestamp(tick);
            endTickButton.GetComponent<LabelSetter>().SetText(tick.ToString());
            ModifierTimeline.Instance.UpdateMark(ModifierTimeline.UpdateType.UpdateEnd);
        }
        else
        {
            currentData.endTick = new QNT_Timestamp(tick);
            endTickButton.GetComponent<LabelSetter>().SetText(tick.ToString());
            ModifierTimeline.Instance.SetModifierMark(currentData.type, currentData.endTick, currentData.shorthand, false);
        }      
    }

    public void OnValue1Changed()
    {
        currentData.value1 = value1.GetComponent<LabelSetter>().GetText();
    }

    public void OnValue2Changed()
    {
        currentData.value2 = value2.GetComponent<LabelSetter>().GetText();
    }

    public void OnOption1Changed()
    {
        currentData.option1 = !currentData.option1;
    }

    public void OnOption2Changed()
    {
        currentData.option2 = !currentData.option2;
    }

    public void OnAmountChanged()
    {
        currentData.amount = amountSlider.GetComponent<Slider>().value;
    }

    public void OnDropdownValueChanged()
    {
        currentData.type = (ModifierType)dropdown.value;

        switch (currentData.type)
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
                break;
            case ModifierType.ArenaBrightness:
                amountSlider.SetActive(true);
                endTickButton.SetActive(true);
                value1.SetActive(false);
                value2.SetActive(false);
                option1.GetComponentInChildren<LabelSetter>().SetText("Continuous");
                option2.GetComponentInChildren<LabelSetter>().SetText("Strobo");
                option1.SetActive(true);
                option2.SetActive(true);
                break;
            case ModifierType.ArenaRotation:
                amountSlider.SetActive(true);
                endTickButton.SetActive(true);
                value1.SetActive(false);
                value2.SetActive(false);
                option2.SetActive(false);
                option1.GetComponentInChildren<LabelSetter>().SetText("Continuous");
                option1.SetActive(true);
                break;
            case ModifierType.ColorChange:
                amountSlider.SetActive(false);
                endTickButton.SetActive(true);
                option1.SetActive(false);
                option2.SetActive(false);
                value1.GetComponentInChildren<LabelSetter>().SetText("Left Hand Color");
                value2.GetComponentInChildren<LabelSetter>().SetText("Right Hand Color");
                value1.SetActive(true);
                value2.SetActive(true);
                break;
            case ModifierType.ColorUpdate:
                amountSlider.SetActive(false);
                endTickButton.SetActive(false);
                option1.SetActive(false);
                option2.SetActive(false);
                value1.GetComponentInChildren<LabelSetter>().SetText("Left Hand Color");
                value2.GetComponentInChildren<LabelSetter>().SetText("Right Hand Color");
                value1.SetActive(true);
                value2.SetActive(true);
                break;
            case ModifierType.PsychedeliaUpdate:
                amountSlider.SetActive(true);
                endTickButton.SetActive(false);
                value1.SetActive(false);
                value2.SetActive(false);
                option1.SetActive(false);
                option2.SetActive(false);
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
                break;
            case ModifierType.zOffset:
                amountSlider.SetActive(true);
                endTickButton.SetActive(true);
                value2.SetActive(false);
                option1.SetActive(false);
                option2.SetActive(false);
                value1.GetComponent<LabelSetter>().SetText("Transition Target Amount");
                value1.SetActive(true);
                break;
        }
        SetMinMax(currentData.type);
        SetShorthand(currentData.type);
    }

    private void SetShorthand(ModifierType type)
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
            default:
                break;
        }
        currentData.shorthand = sh;
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
                slider.SetMinValue(0f);
                slider.SetMaxValue(1000f);
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
                slider.SetMaxValue(300f);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Type: " + currentData.type);
            Debug.Log("Starttick: " + currentData.startTick.ToString());
            Debug.Log("Val1: " + currentData.value1);
            Debug.Log("Opt1: " + currentData.option1.ToString());
        }
    }


    [Serializable]
    public struct ModifierData
    {
        public ModifierType type;
        public string shorthand;
        public QNT_Timestamp startTick;
        public QNT_Timestamp endTick;
        public float amount;
        public string value1;
        public string value2;
        public bool option1;
        public bool option2;
    }

    public enum ModifierType { AimAssist = 0, ColorChange = 1, ColorUpdate = 2, ColorSwap = 3, HiddenTelegraphs = 4, InvisibleGuns = 5, Particles = 6, Psychedelia = 7, PsychedeliaUpdate = 8, Speed = 9, zOffset = 10, ArenaRotation = 11, ArenaBrightness = 12, Fader = 13 }

}


