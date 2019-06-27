using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class HamburgerPanelButton : MonoBehaviour
    {
        private Animator buttonAnimator;

        void Start()
        {
            buttonAnimator = this.GetComponent<Animator>();
        }

        public void HoverButton()
        {
            if (buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("HB Hover to Pressed"))
            {
                // do nothing because it's clicked
            }

            else
            {
                buttonAnimator.Play("HB Hover");
            }
        }

        public void NormalizeButton()
        {
            if (buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("HB Hover to Pressed"))
            {
                // do nothing because it's clicked
            }

            else
            {
                buttonAnimator.Play("HB Hover to Normal");
            }
        }
    }
}