using System.Collections;
using NotReaper;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//  This script will be updated in Part 2 of this 2 part series.
public class PauseModalInstance : MonoBehaviour {
    private Modal modalPanel;
    private UnityAction Action1;
    private UnityAction Action2;
    private UnityAction Action3;
    private UnityAction Action4;

    public Timeline timeline;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && timeline.projectStarted) {
            LoadPanelStart();
        }
    }
    void Awake() {
        modalPanel = Modal.Instance();

        Action1 = new UnityAction(Function1);
        Action2 = new UnityAction(Function2);
        Action3 = new UnityAction(Function3);
        Action4 = new UnityAction(Function4);
    }

    //  Send to the Modal Panel to set up the Buttons and Functions to call
    public void LoadPanelStart() {
        modalPanel.Choice("What Would you like to do?", Function1, Function2, Function3, Function4, "New edica project", "Load edica save", PlayerPrefs.HasKey("previousSave") ? "Load \"" + PlayerPrefs.GetString("previousSave") + "\"" : "No recent files", "Resume", true, true, true, true);
    }


    //new file
    void Function1() {
        timeline.NewFile();
    }

    //load
    void Function2() {
        timeline.Load();
    }

    //load previous
    void Function3() {
        if (PlayerPrefs.HasKey("previousSave"))
            timeline.LoadPrevious();
        else
            timeline.NewFile();
    }

    //resume
    void Function4() {

    }
}