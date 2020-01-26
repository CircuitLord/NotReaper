using System.Collections;
using System.Collections.Generic;
using NotReaper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NotReaper.UserInput;

public class ShortcutInfo : MonoBehaviour {

    public Image BG;
    public CanvasGroup window;
    public GameObject version;
    public GameObject readme;
    public Image readmeUnderline;
    public bool isOpened = false;

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
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
        window.gameObject.transform.DOMove(new Vector3(0,6,0), 1.0f).SetEase(Ease.OutQuint);
        isOpened = true;
    }

    public void hide() {
        gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f);
        gameObject.SetActive(false);
        isOpened = false;
        window.gameObject.transform.DOMove(new Vector3(0,-5,0), 1.0f);
    }
    
    public void LoadUIColors() {
        
        readmeUnderline.color = NRSettings.config.leftColor;
    }
}
