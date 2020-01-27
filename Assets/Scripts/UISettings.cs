using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NotReaper;
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

   public void RegenConfig()
   {
      NRSettings.LoadSettingsJson(true);
   }
   
}
