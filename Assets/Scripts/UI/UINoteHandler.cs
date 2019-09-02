using NotReaper.Grid;
using NotReaper.Models;
using UnityEngine;
using UnityEngine.UI;
using NotReaper.Notifications;
using NotReaper.Targets;
using NotReaper.UserInput;

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

        public EditorInput editorInput;


        public void SoundWasChanged(Dropdown dpd) {
            timeline.CurrentSound = (DropdownToVelocity) dpd.value;

            editorInput.SelectVelocity((DropdownToVelocity) dpd.value);
        }


        public void SelectLeftHand() {
            editorInput.SelectHand(TargetHandType.Left);
        }

        public void SelectRightHand() {
            editorInput.SelectHand(TargetHandType.Right);
        }

        public void SelectEitherHand() {
            editorInput.SelectHand(TargetHandType.Either);
        }

        public void SelectNoHand() {
            //timeline.SetHandType(TargetHandType.None);
            //hover.SetHandType(TargetHandType.None);
            editorInput.SelectHand(TargetHandType.None);
        }


        public void SelectStandard() {
            //EditorInput.selectedTool = EditorTool.Standard;
            //timeline.SetBehavior(TargetBehavior.Standard);
            //timeline.SetVelocity(TargetVelocity.Standard);
            //soundDropdown.value = (int) DropdownToVelocity.Standard;
            //hover.SetBehavior(TargetBehavior.Standard);
            //noteGrid.SetSnappingMode(SnappingMode.Grid);
            //EditorInput.selectedTool()
            editorInput.SelectTool(EditorTool.Standard);

        }

        public void SelectHold() {
            editorInput.SelectTool(EditorTool.Hold);
        }

        public void SelectChainNode() {
            editorInput.SelectTool(EditorTool.ChainNode);
        }

        public void SelectChainStart() {
            editorInput.SelectTool(EditorTool.ChainStart);
        }

        public void SelectHorizontal() {
            editorInput.SelectTool(EditorTool.Horizontal);
        }

        public void SelectVertical() {
            editorInput.SelectTool(EditorTool.Vertical);
        }

        public void SelectMelee() {
            editorInput.SelectTool(EditorTool.Melee);
        }
    }
}