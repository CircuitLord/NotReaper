using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Keyboard
{
    public class ShortcutKeyboardHandler : MonoBehaviour
    {
        public ShortcutInfo shortcutMenu;
        private ShortcutKey[] keys;
        private bool showCtrl = false;
        private bool showShift = false;
        private void Start()
        {
            keys = GetComponentsInChildren<ShortcutKey>();
        }

        private void Update()
        {
            if (!shortcutMenu.isOpened) return;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                showCtrl = !showCtrl;
                showShift = false;
                EnableKeys(showCtrl, OfType.Ctrl);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                showShift = !showShift;
                showCtrl = false;
                EnableKeys(showShift, OfType.Shift);
            }
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

