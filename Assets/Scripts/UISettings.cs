using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NotReaper;
using NotReaper.Grid;
using NotReaper.Managers;
using NotReaper.Models;
using NotReaper.Targets;
using NotReaper.Timing;
using NotReaper.Tools.ChainBuilder;
using NotReaper.UI;
using SFB;
using UnityEngine;
using UnityEditor;

public class UISettings : MonoBehaviour
{
   public GameObject bg;
   public GameObject window;

   public void Start() {
      var t = transform;
      var position = t.localPosition;
      t.localPosition = new Vector3(0, position.y, position.z);
      Deactivate();
   }

   public void Activate()
   {
      bg.SetActive(true);
      window.SetActive(true);
   }

   public void Deactivate()
   {
      bg.SetActive(false);
      window.SetActive(false);
   }

   public void OpenSettingsFile()
   {
      Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper", "NRConfig.txt"));
   }

   public void OpenSettingsFolder()
   {
      ProcessStartInfo startInfo = new ProcessStartInfo
      {
         Arguments = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper"),
         FileName = "explorer.exe"
      };
      Process.Start(startInfo);


      //EditorUtility.RevealInFinder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper", "NRConfig.txt"));
   }


   public void ExportAsCues() {
      
      //Pick folder
      //string prevDir = PlayerPrefs.GetString("recentDirCues", "");

      
      string diff;

      switch (DifficultyManager.I.loadedIndex) {
         case 0:
            diff = "Expert";
            break;
         case 1:
            diff = "Advanced";
            break;
         case 2:
            diff = "Standard";
            break;
         default:
            diff = "Easy";
            break;
         
         
      }
      string fileName = Path.GetFileName(Timeline.audicaFile.filepath)?.Replace(".audica", "");
      fileName = fileName + "_NRExport-" + diff + ".cues";

      string path;



      if (!String.IsNullOrEmpty(NRSettings.config.cuesSavePath)) {
         path = Path.Combine(NRSettings.config.cuesSavePath, fileName);
         


      }
      
      else {
         path = StandaloneFileBrowser.SaveFilePanel("Find community_maps/maps folder in Audica folder", Path.Combine(Application.dataPath, @"../"), fileName, "cues");
         if (String.IsNullOrEmpty(path)) return;
         
         NRSettings.config.cuesSavePath = Path.GetDirectoryName(path);
         NRSettings.SaveSettingsJson();
      }
      
      
      //Ensure all chains are generated
      List<TargetData> nonGeneratedNotes = new List<TargetData>();
      foreach(Target note in Timeline.instance.notes) {
         if(note.data.behavior == TargetBehavior.NR_Pathbuilder && note.data.pathBuilderData.createdNotes == false) {
            nonGeneratedNotes.Add(note.data);
         }
      }

      foreach(var data in nonGeneratedNotes) {
         ChainBuilder.GenerateChainNotes(data);
      }

      CueFile export = new CueFile();
      export.cues = new List<Cue>();
      export.NRCueData = new NRCueData();

      foreach (Target target in Timeline.orderedNotes) {

         if (target.data.beatLength == 0) target.data.beatLength = Constants.SixteenthNoteDuration;
				
         if (target.data.behavior == TargetBehavior.Metronome) continue;
				
         var cue = NotePosCalc.ToCue(target, Timeline.offset);

         if(target.data.behavior == TargetBehavior.NR_Pathbuilder) {
            export.NRCueData.pathBuilderNoteCues.Add(cue);
            export.NRCueData.pathBuilderNoteData.Add(target.data.pathBuilderData);
            continue;
         }

         export.cues.Add(cue);
      }

      
      File.WriteAllText(path, JsonUtility.ToJson(export));
      
      NotificationShower.AddNotifToQueue(new NRNotification("Saved cues!"));
      
   }
   
   public void RegenConfig()
   {
      NRSettings.LoadSettingsJson(true);
   }
   
}
