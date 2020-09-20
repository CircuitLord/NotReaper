using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;
using UnityEngine.Events;
using System.IO;
using NotReaper;

public class FileDragAndDrop : MonoBehaviour
{
    void OnEnable()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        if (aFiles[0].Contains(".audica"))
        {
            if(Timeline.audicaLoaded) Timeline.instance.Export();
            Timeline.instance.LoadAudicaFile(false, aFiles[0]);
        }
    }

}
