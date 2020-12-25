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
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
        
        Transform camTrans = Camera.main.transform;

        window.transform.position = new Vector3(camTrans.position.x, camTrans.position.y, transform.position.z);
        
        window.transform.DOMove(new Vector3(transform.position.x,camTrans.position.y + 5.5f, transform.position.z), 1.0f).SetEase(Ease.OutQuint);
        isOpened = true;
    }



    public void Hide() {

        gameObject.SetActive(false);


        isOpened = false;
    }



    
    public void LoadUIColors() {
        
        readmeUnderline.color = NRSettings.config.leftColor;
    }
}
