using System.Collections;
using System.Collections.Generic;
using System.IO;
using NotReaper;
using UnityEngine;
using UnityEngine.UI;


public class DifficultySelection : MonoBehaviour {
    public Dropdown selection;

    public Timeline timeline;

    public Sprite checkmark;

    public enum Options {
        beginner = 3,
        moderate = 2,
        advanced = 1,
        expert = 0
    }

    public string Value { get; set; }

    public void Start() {
        Value = Options.GetName(typeof(Options), selection.value);

        selection.onValueChanged.AddListener(delegate {
            DropdownValueChanged(selection);
        });

    }

    public void Update() {
        if (System.IO.Directory.Exists(Application.persistentDataPath + "\\temp\\")) {
            bool expertFile = false;
            bool advancedFile = false;
            bool moderateFile = false;
            bool beginnerFile = false;

            var cueFiles = Directory.GetFiles(Application.persistentDataPath + "\\temp\\", "*.cues");
            if (cueFiles.Length > 0) {
                foreach (var file in cueFiles) {
                    if (file.Contains("expert.cues")) {
                        expertFile = true;
                    }

                    if (file.Contains("advanced.cues")) {
                        advancedFile = true;
                    }

                    if (file.Contains("moderate.cues")) {
                        moderateFile = true;
                    }
                    if (file.Contains("beginner.cues")) {
                        beginnerFile = true;
                    }
                }
            }

            if (expertFile) {

                foreach (var option in selection.options) {
                    if (option.text == "Expert")
                        option.image = checkmark;
                }
            } else {
                foreach (var option in selection.options) {
                    if (option.text == "Expert")
                        option.image = null;
                }
            }

            if (advancedFile) {

                foreach (var option in selection.options) {
                    if (option.text == "Advanced")
                        option.image = checkmark;
                }
            } else {
                foreach (var option in selection.options) {
                    if (option.text == "Advanced")
                        option.image = null;
                }
            }

            if (moderateFile) {

                foreach (var option in selection.options) {
                    if (option.text == "Moderate")
                        option.image = checkmark;
                }
            } else {
                foreach (var option in selection.options) {
                    if (option.text == "Moderate")
                        option.image = null;
                }
            }

            if (beginnerFile) {

                foreach (var option in selection.options) {
                    if (option.text == "Beginner")
                        option.image = checkmark;
                }
            } else {
                foreach (var option in selection.options) {
                    if (option.text == "Beginner")
                        option.image = null;
                }
            }

        }
    }

    void DropdownValueChanged(Dropdown change) {
        //save current .cues
        string dirpath = Application.persistentDataPath;

        if (!System.IO.Directory.Exists(dirpath + "\\temp\\")) {
            System.IO.Directory.CreateDirectory(dirpath + "\\temp\\");
        }

        timeline.ExportCueToTemp();

        //change difficulty
        Value = Options.GetName(typeof(Options), change.value);

        //load new difficulty
        timeline.ChangeDifficulty();
    }
}