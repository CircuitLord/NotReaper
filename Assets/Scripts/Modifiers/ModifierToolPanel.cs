using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierToolPanel : MonoBehaviour
{
    public GameObject noteToolSelect;
    public GameObject modifierList;
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SwapToolPanel();
        }
    }

    private void SwapToolPanel()
    {
        noteToolSelect.SetActive(!noteToolSelect.activeSelf);
        modifierList.SetActive(!modifierList.activeSelf);
    }
}
