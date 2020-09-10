using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class WindowManager : MonoBehaviour
{
    public int windowCount;
    //public bool windowsActive = windowCount > 0;
    public List<Window> windowList;

    void Awake()
    {
        this.windowList = new List<Window>();
    }
    public void OpenWindowByName(string windowName, bool closeOtherWindows = false)
    {
        foreach (Window window in windowList)
        {
            if (window.windowName == windowName)
            {
                window.ShowWindow();
            }
            else if (closeOtherWindows)
            {
                window.HideWindow();
            }
        }
    }
    public void OpenSingleWindow(string windowName)
    {
        foreach (Window window in windowList)
        {
            if (window.windowName == windowName)
            {
                if (!window.isWindowActive)
                {
                    window.ShowWindow();
                }
                else
                {
                    window.HideWindow();
                }

            }

        }
    }

    public void CloseAllWindows()
    {
        foreach (Window window in windowList)
        {
            window.HideWindow();
        }
    }
    //Mostly for debugging
    public void OpenAllWindows()
    {
        foreach (Window window in windowList)
        {
            window.ShowWindow();
        }
    }

    public void LogAllWindowNames()
    {
        foreach (Window window in windowList)
        {
            Debug.Log(window.windowName);
        }
    }
}
