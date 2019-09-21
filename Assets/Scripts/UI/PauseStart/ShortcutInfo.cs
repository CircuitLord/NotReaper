using System.Collections;
using System.Collections.Generic;
using NotReaper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutInfo : MonoBehaviour {

    public Image BG;
    public CanvasGroup window;
    public GameObject version;
    public GameObject readme;
    public Image readmeUnderline;

    // Start is called before the first frame update
    void Start() {
        var t = transform;
        var position = t.localPosition;
        t.localPosition = new Vector3(0, position.y, position.z);
        
        TextMeshProUGUI versionLabel = version.GetComponent<TextMeshProUGUI>();
        var versionButton = version.GetComponent<Button>();
        versionLabel.text = "Version " + Application.version;
        versionButton.onClick.AddListener(() => {
            Application.OpenURL("https://github.com/CircuitLord/NotReaper/releases");
        });
        var readmeButton = readme.GetComponent<Button>();
        readmeButton.onClick.AddListener(() => {
            Application.OpenURL("https://github.com/CircuitLord/NotReaper/blob/master/README.md");
        });
        hide();
    }

    public void show() {
        BG.gameObject.SetActive(true);
        window.gameObject.SetActive(true);
    }

    public void hide() {
        BG.gameObject.SetActive(false);
        window.gameObject.SetActive(false);
    }
    
    public void LoadUIColors() {
        
        readmeUnderline.color = NRSettings.config.leftColor;
    }
}
