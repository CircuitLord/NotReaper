using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class DemoHamburgerMenu : MonoBehaviour
    {
        public Animator hamburgerMenu;
        public Animator hamburgerButton;

        private bool isOpen = false;

        public void PanelAnim()
        {
            if (isOpen == false)
            {
                hamburgerMenu.Play("HM Open");
                hamburgerButton.Play("HTE Exit");
                isOpen = true;
            }
            else
            {
                hamburgerMenu.Play("HM Close");
                hamburgerButton.Play("HTE Hamburger");
                isOpen = false;
            }
        }
    }
}
