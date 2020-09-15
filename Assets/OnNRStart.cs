using NotReaper;
using NotReaper.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OnNRStart : MonoBehaviour
{
    string argsAudicaFilePath;
    void Start()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        if (arguments.Length == 1) return;
        if (arguments[1].Contains(".audica"))
        {
            argsAudicaFilePath = arguments[1];
            NRSettings.PostLoad.AddListener(new UnityAction(() =>
            {
                Invoke("LoadArgsFile", 6f); // This is really bad, don't do this.
            }));
        }
    }
    
    void LoadArgsFile()
    {
        Timeline.instance.LoadAudicaFile(false, argsAudicaFilePath);
    }
}
