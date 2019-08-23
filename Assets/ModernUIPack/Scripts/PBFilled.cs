using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class PBFilled : MonoBehaviour
    {
        public ProgressBar proggresBar;

        [Header("SETTINGS")]
        public Animator barAnimatior;
        [Range(0, 100)] public int transitionAfter = 50;
        private string animText = "Radial PB Filled";
        private string emptyAnimText = "Radial PB Empty";

        void Update()
        {
            if (proggresBar.currentPercent >= transitionAfter)
            {
                barAnimatior.Play(animText);
            }

            if (proggresBar.currentPercent <= transitionAfter)
            {
                barAnimatior.Play(emptyAnimText);
            }
        }
    }
}