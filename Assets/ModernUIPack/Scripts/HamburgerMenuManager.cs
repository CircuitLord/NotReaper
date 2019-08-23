using UnityEngine;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class HamburgerMenuManager : MonoBehaviour
    {
        private Animator menuAnimator;

        [Header("RESOURCES")]
        public Animator animatedButton;
        public TextMeshProUGUI title;

        [Header("SETTINGS")]
        public bool openAtStart = false;
        public string titleAtStart;

        bool isOpen;

        void Start()
        {
            menuAnimator = gameObject.GetComponent<Animator>();
            title.text = titleAtStart;

            if(openAtStart == true)
            {
                menuAnimator.Play("Expand");
                animatedButton.Play("HTE Expand");
                isOpen = true;
            }

            else
            {
                menuAnimator.Play("Minimize");
                animatedButton.Play("HTE Hamburger");
                isOpen = false;
            }
        }

        public void Animate()
        {
            if (isOpen == true)
            {
                menuAnimator.Play("Minimize");
                animatedButton.Play("HTE Hamburger");
                isOpen = false;
            }

            else
            {          
                menuAnimator.Play("Expand");
                animatedButton.Play("HTE Exit");
                isOpen = true;
            }
        }

        public void ChangeText(string titleText)
        {
            title.text = titleText;
        }
    }
}