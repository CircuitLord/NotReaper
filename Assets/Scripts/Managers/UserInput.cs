using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : MonoBehaviour
{

    public Toggle standardButton;
    public Toggle holdButton;
    public Toggle horzButton;
    public Toggle vertButton;
    public Toggle chainStartButton;
    public Toggle chainNodeButton;
    public Toggle meleeButton;

    public Toggle L;
    public Toggle R;

    public Dropdown soundSelect;

    [SerializeField] private OptionsMenu optionsMenu;

    void Update () {



        if(Input.GetKeyDown(InputManager.IM.selectStandard)) {
            //optionsMenu.SelectStandard();
            standardButton.isOn = true;
            UserPrefsManager.ChangeColors();
        }
        if(Input.GetKeyDown(InputManager.IM.selectHold)) {
            holdButton.isOn = true;
        }
        if(Input.GetKeyDown(InputManager.IM.selectHorz)) {
            horzButton.isOn = true;
        }
        if(Input.GetKeyDown(InputManager.IM.selectVert)) {
            vertButton.isOn = true;
        }
        if(Input.GetKeyDown(InputManager.IM.selectChainStart)) {
            chainStartButton.isOn = true;
        }
        if(Input.GetKeyDown(InputManager.IM.selectChainNode)) {
            chainNodeButton.isOn = true;
        }
        if(Input.GetKeyDown(InputManager.IM.selectMelee)) {
            meleeButton.isOn = true;
        }

        if(Input.GetKeyDown(InputManager.IM.toggleColor)) {

            if (L.isOn == false) {
                L.isOn = true;
            } else if (R.isOn == false) {
                R.isOn = true;
            } else {
                L.isOn = true;
            }
        }

        if(Input.GetKeyDown(InputManager.IM.selectSoundKick)) {
            soundSelect.value = 0;
        }
        if(Input.GetKeyDown(InputManager.IM.selectSoundSnare)) {
            soundSelect.value = 1;
        }
        if(Input.GetKeyDown(InputManager.IM.selectSoundPercussion)) {
            soundSelect.value = 2;
        }
        if(Input.GetKeyDown(InputManager.IM.selectSoundChainStart)) {
            soundSelect.value = 3;
        }
        if(Input.GetKeyDown(InputManager.IM.selectSoundChainNode)) {
            soundSelect.value = 4;
        }
        if(Input.GetKeyDown(InputManager.IM.selectSoundMelee)) {
            soundSelect.value = 5;
        }


    }
}
