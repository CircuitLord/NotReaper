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
			if (!Directory.Exists(Path.Combine(Application.dataPath, @"../", "saves"))) {
				Directory.CreateDirectory(Path.Combine(Application.dataPath, @"../", "saves"));
			}
		}

		public static void ClearCache() {
			if (Directory.Exists($"{Application.dataPath}/.cache")) {
				Directory.Delete($"{Application.dataPath}/.cache", true);
			}
		}


		public static void ClearCueCache() {
			File.Delete($"{Application.dataPath}/.cache/expert.cues");
			File.Delete($"{Application.dataPath}/.cache/advanced.cues");
			File.Delete($"{Application.dataPath}/.cache/moderate.cues");
			File.Delete($"{Application.dataPath}/.cache/beginner.cues");
            if (File.Exists($"{Application.dataPath}/.cache/modifiers.json"))
                File.Delete($"{Application.dataPath}/.cache/modifiers.json");
        }

	}

}