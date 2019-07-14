using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.UserInput {


	public static class InputManager {


		//Used for singleton

		//Create Keycodes that will be associated with each of our commands.
		//These can be accessed by any other script in our game
		public static KeyCode selectStandard { get; set; }
		public static KeyCode selectHold { get; set; }
		public static KeyCode selectHorz { get; set; }
		public static KeyCode selectVert { get; set; }
		public static KeyCode selectChainStart { get; set; }
		public static KeyCode selectChainNode { get; set; }
		public static KeyCode selectMelee { get; set; }
		public static KeyCode toggleColor { get; set; }
		public static KeyCode selectSoundKick { get; set; }
		public static KeyCode selectSoundSnare { get; set; }
		public static KeyCode selectSoundPercussion { get; set; }
		public static KeyCode selectSoundChainStart { get; set; }
		public static KeyCode selectSoundChainNode { get; set; }
		public static KeyCode selectSoundMelee { get; set; }
		public static KeyCode selectTool { get; set; }


		public static void LoadHotkeys() {
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

			selectTool = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectTool", "F"));
		}

	}
}
