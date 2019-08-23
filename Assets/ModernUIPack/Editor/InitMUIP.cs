using UnityEngine;
using UnityEditor;

public class InitMUIP : MonoBehaviour
{
    [InitializeOnLoad]
	public class InitOnLoad
	{
		static InitOnLoad()
		{
			if (!EditorPrefs.HasKey("MUIP.Installed"))
			{
				EditorPrefs.SetInt("MUIP.Installed", 1);
				EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Modern UI Pack.\r\rFirst of all, import TextMesh Pro from Package Manager if you haven't already.\r\rYou can contact me at isa.steam@outlook.com for support.", "Got it!");
			}
		}
	}
}