using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Used for singleton
    public static InputManager IM;
 
    //Create Keycodes that will be associated with each of our commands.
    //These can be accessed by any other script in our game
    public KeyCode selectStandard {get; set;}
    public KeyCode selectHold {get; set;}
    public KeyCode selectHorz {get; set;}
    public KeyCode selectVert {get; set;}
    public KeyCode selectChainStart {get; set;}
    public KeyCode selectChainNode {get; set;}
    public KeyCode selectMelee {get; set;}
    public KeyCode toggleColor { get; set; }
    public KeyCode selectSoundKick { get; set; }
    public KeyCode selectSoundSnare { get; set; }
    public KeyCode selectSoundPercussion { get; set; }
    public KeyCode selectSoundChainStart { get; set; }
    public KeyCode selectSoundChainNode { get; set; }
    public KeyCode selectSoundMelee { get; set; }
 
    
 
    void Awake()
    {
        //Singleton pattern
        if(IM == null)
        {
            DontDestroyOnLoad(gameObject);
            IM = this;
        }
        else if(IM != this)
        {
            Destroy(gameObject);
        }
        /*Assign each keycode when the game starts.
         * Loads data from PlayerPrefs so if a user quits the game,
         * their bindings are loaded next time. Default values
         * are assigned to each Keycode via the second parameter
         * of the GetString() function
         */
        selectStandard = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectStandard", "Alpha1"));
        selectHold = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectHold", "Alpha2"));
        selectHorz = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectHorz", "Alpha3"));
        selectVert = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectVert", "Alpha4"));
        selectChainStart = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectChainStart", "Alpha5"));
        selectChainNode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectChainNode", "Alpha6"));
        selectMelee = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectChainNode", "Alpha7"));

        toggleColor = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("toggleColor", "S"));
        
        selectSoundKick = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectSoundKick", "Q"));
        selectSoundSnare = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectSoundSnare", "W"));
        selectSoundPercussion = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectSoundPercussion", "E"));
        selectSoundChainStart = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectSoundChainStart", "R"));
        selectSoundChainNode = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectSoundChainNode", "T"));
        selectSoundMelee = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectSoundMelee", "Y"));
 
    }
}
