using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

public static class RecentAudicaFiles
{
    public static List<string> audicaPaths;
    private static readonly string recentsFilePath = Path.Combine(Application.persistentDataPath, "RecentDirs.json");

    public static void AddRecentDir(string dir)
    {
        if (audicaPaths.Contains(dir)) audicaPaths.Remove(dir);
        audicaPaths.Insert(0, dir);
        if (audicaPaths.Count > 6) audicaPaths = audicaPaths.GetRange(0, 6);

        SaveRecents();
        Debug.Log("Saved " + dir);
    }
    
    public static void SaveRecents()
    {
        string text = JsonConvert.SerializeObject(audicaPaths);
        File.WriteAllText(recentsFilePath, text);
    }

    public static void LoadRecents()
    {
        if (audicaPaths != null) return;
        if (File.Exists(recentsFilePath))
        {
            try
            {
                string text = File.ReadAllText(recentsFilePath);
                audicaPaths = JsonConvert.DeserializeObject<List<string>>(text);
                //foreach (var item in audicaPaths)
                //{
                //    Debug.Log(item);
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }
        else
        {
            audicaPaths = new List<string>();
        }
    }

    public static void ClearRecents()
    {
        audicaPaths = new List<string>();
        SaveRecents();
    }

}
