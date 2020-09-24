using NotReaper;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] Toggle richPresence;
    [SerializeField] Toggle clearCacheOnStartup;
    [SerializeField] Toggle enableTraceLines;
    [SerializeField] Toggle enableDualines;
    [SerializeField] Toggle useAutoZOffsetWith360;
    [SerializeField] Toggle useBouncyAnimations;
    [SerializeField] Toggle playNoteSoundsWhileScrolling;
    [SerializeField] Toggle autoSongVolume;
    [SerializeField] Toggle optimizeInvisibleTargets;

    [SerializeField] TMP_InputField savedMapperField;

    [SerializeField] ColorSlider LeftHand;
    [SerializeField] ColorSlider RightHand;

    [SerializeField] GameObject WarningText;


    private void Start()
    {
        NRSettings.OnLoad(() => {
            UpdateUI();
        });
    }

    public void UpdateUI()
    {
        richPresence.isOn = NRSettings.config.useDiscordRichPresence;
        clearCacheOnStartup.isOn = NRSettings.config.clearCacheOnStartup;
        enableTraceLines.isOn = NRSettings.config.enableTraceLines;
        enableDualines.isOn = NRSettings.config.enableDualines;
        useAutoZOffsetWith360.isOn = NRSettings.config.useAutoZOffsetWith360;
        useBouncyAnimations.isOn = NRSettings.config.useBouncyAnimations;
        playNoteSoundsWhileScrolling.isOn = NRSettings.config.playNoteSoundsWhileScrolling;
        optimizeInvisibleTargets.isOn = NRSettings.config.optimizeInvisibleTargets;
        autoSongVolume.isOn = NRSettings.config.autoSongVolume;
        LeftHand.SetColor(NRSettings.config.leftColor);
        RightHand.SetColor(NRSettings.config.rightColor);
        savedMapperField.text = NRSettings.config.savedMapperName;
        
    }

    public void ApplyValues()
    {
        NRSettings.config.useDiscordRichPresence = richPresence.isOn;
        NRSettings.config.clearCacheOnStartup = clearCacheOnStartup.isOn;
        NRSettings.config.enableTraceLines = enableTraceLines.isOn;
        NRSettings.config.enableDualines = enableDualines.isOn;
        NRSettings.config.useAutoZOffsetWith360 = useAutoZOffsetWith360.isOn;
        NRSettings.config.useBouncyAnimations = useBouncyAnimations.isOn;
        NRSettings.config.playNoteSoundsWhileScrolling = playNoteSoundsWhileScrolling.isOn;
        NRSettings.config.autoSongVolume = autoSongVolume.isOn;
        NRSettings.config.leftColor = LeftHand.color;
        NRSettings.config.rightColor = RightHand.color;
        NRSettings.config.savedMapperName = savedMapperField.text;
        NRSettings.config.optimizeInvisibleTargets = optimizeInvisibleTargets.isOn;
        WarningText.SetActive(true);
        NRSettings.SaveSettingsJson();
    }
}
