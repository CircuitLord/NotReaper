using NotReaper.Grid;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.UI;
using NotReaper.Notifications;
using NotReaper.Targets;

namespace NotReaper.UI {

    public enum DropdownToVelocity {
        Standard = 0, Snare = 1, Percussion = 2, ChainStart = 3, Chain = 4, Melee = 5
    }


    public class UINoteHandler : MonoBehaviour {


        public Timeline timeline;

        [SerializeField] private Dropdown soundDropdown;

        public TargetIcon hover;

        public NoteGridSnap noteGrid;

        public Notification notification;


        public void SoundWasChanged(Dropdown dpd) {
            timeline.CurrentSound = (DropdownToVelocity) dpd.value;
        }


        public void SelectLeftHand() {
            timeline.SetHandType(TargetHandType.Left);
            hover.SetHandType(TargetHandType.Left);
        }

        public void SelectRightHand() {
            timeline.SetHandType(TargetHandType.Right);
            hover.SetHandType(TargetHandType.Right);
        }

        public void SelectEitherHand() {
            timeline.SetHandType(TargetHandType.Either);
            hover.SetHandType(TargetHandType.Either);
        }

        public void SelectNoHand() {
            timeline.SetHandType(TargetHandType.None);
            hover.SetHandType(TargetHandType.None);
        }


        public void SelectStandard() {
            timeline.SetBehavior(TargetBehavior.Standard);
            timeline.SetVelocity(TargetVelocity.Standard);
            soundDropdown.value = (int) DropdownToVelocity.Standard;
            hover.SetBehavior(TargetBehavior.Standard);
            noteGrid.SetSnappingMode(SnappingMode.Grid);
            //notification.ShowNotification("STANDARD", "A new note tool was selected, causing a notification to come.", 5);

        }

        public void SelectHold() {
            timeline.SetBehavior(TargetBehavior.Hold);
            timeline.SetVelocity(TargetVelocity.Hold);
            soundDropdown.value = (int) DropdownToVelocity.Standard;
            hover.SetBehavior(TargetBehavior.Hold);
            noteGrid.SetSnappingMode(SnappingMode.Grid);
        }

        public void SelectChainNode() {
            timeline.SetBehavior(TargetBehavior.Chain);
            timeline.SetVelocity(TargetVelocity.Chain);
            soundDropdown.value = (int) DropdownToVelocity.Chain;
            hover.SetBehavior(TargetBehavior.Chain);
            noteGrid.SetSnappingMode(SnappingMode.Grid);
        }

        public void SelectChainStart() {
            timeline.SetBehavior(TargetBehavior.ChainStart);
            timeline.SetVelocity(TargetVelocity.ChainStart);
            soundDropdown.value = (int) DropdownToVelocity.ChainStart;
            hover.SetBehavior(TargetBehavior.ChainStart);
            noteGrid.SetSnappingMode(SnappingMode.Grid);
        }

        public void SelectHorizontal() {
            timeline.SetBehavior(TargetBehavior.Horizontal);
            timeline.SetVelocity(TargetVelocity.Horizontal);
            soundDropdown.value = (int) DropdownToVelocity.Standard;
            hover.SetBehavior(TargetBehavior.Horizontal);
            noteGrid.SetSnappingMode(SnappingMode.Grid);
        }

        public void SelectVertical() {
            timeline.SetBehavior(TargetBehavior.Vertical);
            timeline.SetVelocity(TargetVelocity.Vertical);
            soundDropdown.value = (int) DropdownToVelocity.Standard;
            hover.SetBehavior(TargetBehavior.Vertical);
            noteGrid.SetSnappingMode(SnappingMode.Grid);
        }

        public void SelectMelee() {
            timeline.SetBehavior(TargetBehavior.Melee);
            timeline.SetVelocity(TargetVelocity.Melee);
            soundDropdown.value = (int) DropdownToVelocity.Melee;
            hover.SetBehavior(TargetBehavior.Melee);
            noteGrid.SetSnappingMode(SnappingMode.Melee);
        }
    }
}