using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UISettings : MonoBehaviour
{
   public GameObject bg;
   public GameObject window;



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
      System.Diagnostics.Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper", "NRConfig.json"));
   }
   
}
