using UnityEngine;

namespace NotReaper.UserInput {


	[CreateAssetMenu(fileName = "KeybindingsSO", menuName = "NotReaper/KeybindingsSO", order = 0)]
	public class KeybindingsSO : ScriptableObject {

		public KeyCode selectStandard, selectHold, selectHorz, selectVert, selectChainStart, selectChainPart, selectMelee;

	}


}