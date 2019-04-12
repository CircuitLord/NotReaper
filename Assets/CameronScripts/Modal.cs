using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Modal : MonoBehaviour {

    public Text question;
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;
    public GameObject modalPanelObject;
    
    private static Modal modalPanel;
    
    public static Modal Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (Modal)) as Modal;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    public void Choice (string question, UnityAction option1, UnityAction option2, UnityAction option3, UnityAction option4, string button1Text, string button2Text, string button3Text, string button4Text,bool option1ClosePanel,bool option2ClosePanel,bool option3ClosePanel,bool option4ClosePanel) {
        modalPanelObject.SetActive (true);
        
        Button1.onClick.RemoveAllListeners();
        Button1.onClick.AddListener (option1);
        Button1.GetComponentInChildren<Text>().text = button1Text;
        if(option1ClosePanel)
            Button1.onClick.AddListener (ClosePanel);
        
        Button2.onClick.RemoveAllListeners();
        Button2.onClick.AddListener (option2);
        Button2.GetComponentInChildren<Text>().text = button2Text;
         if(option2ClosePanel)
        Button2.onClick.AddListener (ClosePanel);
        
        Button3.onClick.RemoveAllListeners();
        Button3.onClick.AddListener (option3);
        Button3.GetComponentInChildren<Text>().text = button3Text;
         if(option3ClosePanel)
        Button3.onClick.AddListener (ClosePanel);

        Button4.onClick.RemoveAllListeners();
        Button4.onClick.AddListener (option4);
        Button4.GetComponentInChildren<Text>().text = button4Text;
         if(option4ClosePanel)
        Button4.onClick.AddListener (ClosePanel);

        this.question.text = question;
        Button1.gameObject.SetActive (true);
        Button2.gameObject.SetActive (true);
        Button3.gameObject.SetActive (true);
        Button4.gameObject.SetActive (true);
    }

    void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
}