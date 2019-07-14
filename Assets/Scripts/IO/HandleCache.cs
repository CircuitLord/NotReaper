using System.IO;
using UnityEngine;

namespace NotReaper.IO {


	public class HandleCache {

		public static void CheckCacheFolderValid() {
			if (!Directory.Exists($"{Application.dataPath}/.cache")) {
				Directory.CreateDirectory($"{Application.dataPath}/.cache");
			}
		}

		public static void CheckSaveFolderValid() {
			if (!Directory.Exists($"{Application.persistentDataPath}/saves")) {
				Directory.CreateDirectory($"{Application.persistentDataPath}/saves");
			}
		}


		public static void ClearCueCache() {
			File.Delete($"{Application.dataPath}/.cache/expert.cues");
			File.Delete($"{Application.dataPath}/.cache/advanced.cues");
			File.Delete($"{Application.dataPath}/.cache/standard.cues");
			File.Delete($"{Application.dataPath}/.cache/easy.cues");
		}

	}

}