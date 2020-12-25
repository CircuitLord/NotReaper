using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Keyboard
{
    public class ShortcutKeyboardHandler : MonoBehaviour
    {
        public static ShortcutKeyboardHandler Instance = null;
        public ShortcutInfo shortcutMenu;
        private ShortcutKey[] keys;
        private bool showCtrl = false;
        private bool showShift = false;
        private Camera main;
        private void Start()
        {
            if (Instance is null) Instance = this;
            else
            {
                Debug.LogWarning("Trying to create second ShortcutKeyboardHandler instance.");
                return;
            }
            main = Camera.main;
            keys = GetComponentsInChildren<ShortcutKey>();
        }

        private void Update()
        {
            if (!shortcutMenu.isOpened) return;
            
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                SelectCtrlKey();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SelectShiftKey();
            }
        }

        public void SelectCtrlKey()
        {
            showCtrl = !showCtrl;
            showShift = false;
            EnableKeys(showCtrl, OfType.Ctrl);
        }

        public void SelectShiftKey()
        {
            showShift = !showShift;
            showCtrl = false;
            EnableKeys(showShift, OfType.Shift);
        }

        private void EnableKeys(bool enable, OfType type)
        {
            DisableAllKeys();
            switch (type)
            {
                case OfType.Ctrl:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (keys[i].selectableCtrl) keys[i].Enable(enable, type);
                    }
                    break;
                case OfType.Shift:
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (keys[i].selectableShift) keys[i].Enable(enable, type);
                    }
                    break;
                default:
                    break;
            }
            if (!showShift && !showCtrl)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].selectableNormal) keys[i].Enable(true, OfType.Normal);
                }
            }

        }

        private void DisableAllKeys()
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].Enable(false, OfType.None);
            }
        }

       
    }
    public enum OfType
    {
        Normal,
        Ctrl,
        Shift,
        None
    }
}

