using System;

using System.Windows.Forms;


using UnityEngine;

namespace NotReaper.IO {


	class PathLogic {

		public const string AudicaAddonPath = @"\Audica_Data\StreamingAssets\HmxAudioAssets\songs";

		public const string AudicaExe = "Audica.exe";

		public const int AudicaSteamAppId = 1020340;

		public const string AudicaOculusSubFolderPath = @"Software\harmonix-audica-k\";




		public static string GetSongFolder() {

				string songFolder = PlayerPrefs.GetString("audicaSongFolder");

				if (songFolder != "") return songFolder;
			

				//Attempt to automatically find song folder.
				string installFolderSteam = GetSteamLocation(AudicaSteamAppId);
				if (installFolderSteam != null) {
					SetInstallFolder(installFolderSteam);
					return PlayerPrefs.GetString("audicaSongFolder");
				}

				//string installFolderOculus = GetValidOculusLocation(AudicaOculusSubFolderPath, AudicaExe);
				//if (installFolderOculus != null) {
				//	SetInstallFolder(installFolderOculus);
				//	return PlayerPrefs.GetString("audicaSongFolder");
				//}

				//Let the user manually select it.
				string installFolderUser = FolderSelectionDialog();
				if (installFolderUser != null) {
					SetInstallFolder(installFolderUser);
					return PlayerPrefs.GetString("audicaSongFolder");
				}

				void SetInstallFolder(string installFolder) {
					PlayerPrefs.SetString("audicaSongFolder", installFolder + AudicaAddonPath);
				}

				return null;

		}

		// Bring up a dialog to chose a folder path in which to open or save a file.
		public static string FolderSelectionDialog() {
			var fbd = new FolderBrowserDialog();
			fbd.Description = @"Please select your Audica install folder.";

			// Show the FolderBrowserDialog.
			DialogResult result = fbd.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK) {
				return fbd.SelectedPath;

			}

			if (result == System.Windows.Forms.DialogResult.Cancel) {
				//System.Windows.Application.Current.Shutdown();
			}

			return null;
		}


		public static string GetSteamLocation(int appId) {
			try {
				var steamFinder = new SteamFinder();
				if (!steamFinder.FindSteam())
					return null;

				return steamFinder.FindGameFolder(appId);
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return null;
			}

		}

		/*
		public static string GetValidOculusLocation(string subFolderPath, string exeName) {

			string path = Registry.LocalMachine.OpenSubKey("SOFTWARE")?.OpenSubKey("WOW6432Node")?.OpenSubKey("Oculus VR, LLC")?.OpenSubKey("Oculus")?.OpenSubKey("Config")?.GetValue("InitialAppLibrary") as string;
			Console.WriteLine(path);
			if (path == null) {
				// No Oculus Home detected
				//return null;
			}



			// With the old Home
			//string folderPath = Path.Combine(path, subFolderPath);
			string folderPath = path + subFolderPath;
			//string fullAppPath = Path.Combine(folderPath, AppFileName);
			string fullAppPath = folderPath + exeName;

			if (File.Exists(fullAppPath)) {
				return folderPath;
			} else {
				// With the new Home / Dash
				using (RegistryKey librariesKey = Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("Oculus VR, LLC")?.OpenSubKey("Oculus")?.OpenSubKey("Libraries")) {
					// Oculus libraries uses GUID volume paths like this "\\?\Volume{0fea75bf-8ad6-457c-9c24-cbe2396f1096}\Games\Oculus Apps", we need to transform these to "D:\Game"\Oculus Apps"
					WqlObjectQuery wqlQuery = new WqlObjectQuery("SELECT * FROM Win32_Volume");
					ManagementObjectSearcher searcher = new ManagementObjectSearcher(wqlQuery);
					Dictionary<string, string> guidLetterVolumes = new Dictionary<string, string>();

					foreach (ManagementBaseObject disk in searcher.Get()) {
						var diskId = ((string)disk.GetPropertyValue("DeviceID")).Substring(11, 36);
						var diskLetter = ((string)disk.GetPropertyValue("DriveLetter")) + @"\";

						if (!string.IsNullOrWhiteSpace(diskLetter)) {
							guidLetterVolumes.Add(diskId, diskLetter);
						}
					}

					// Search among the library folders
					foreach (string libraryKeyName in librariesKey.GetSubKeyNames()) {
						Console.WriteLine(libraryKeyName);
						using (RegistryKey libraryKey = librariesKey.OpenSubKey(libraryKeyName)) {
							string libraryPath = (string)libraryKey.GetValue("Path");
							folderPath = Path.Combine(guidLetterVolumes.First(x => libraryPath.Contains(x.Key)).Value, libraryPath.Substring(49), subFolderPath);
							fullAppPath = Path.Combine(folderPath, exeName);

							if (File.Exists(fullAppPath)) {
								return folderPath;
							}
						}
					}
				}
			}

			return null;
		}
		*/
	}
}