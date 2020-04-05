using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NotReaper {

    public class NRSettings : MonoBehaviour {

        public static NRJsonSettings config = new NRJsonSettings();
        private static bool isLoaded = false;
        private static readonly string configFilePath = Path.Combine(Application.persistentDataPath, "NRConfig.txt");
        private static bool failsafeThingy = false;
        private static List<Action> pendingActions = new List<Action>();

        public static void LoadSettingsJson(bool regenConfig = false) {
            //If it doesn't exist, we need to gen a new one.
            if (regenConfig || !File.Exists(configFilePath)) {
                //Gen new config will autoload the new config.
                if (!failsafeThingy && File.Exists(Path.Combine(Application.persistentDataPath, "NRConfig.json"))) {
                    File.Move(Path.Combine(Application.persistentDataPath, "NRConfig.json"),
                        Path.Combine(Application.persistentDataPath, "NRConfig.txt"));

                    failsafeThingy = true;
                    LoadSettingsJson();
                    return;
                }

                GenNewConfig();
                
                // Load config on first startup.
                LoadSettingsJson();
                return;
            }

            try {
                config = JsonUtility.FromJson<NRJsonSettings>(File.ReadAllText(configFilePath));
                string winConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper", "BG1.png");
                
                // Check if Linux / Mac user is using an old cfg.
                if (((Application.platform == RuntimePlatform.LinuxEditor) || (Application.platform == RuntimePlatform.LinuxPlayer)) || 
                    ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)))
                    if (config.bgImagePath == winConfigDir)
                        config.bgImagePath = GetbgImagePath();

                if (!(File.Exists(config.bgImagePath)))
                    GenNewConfig(true);

            } catch (Exception e) {
                Debug.LogError(e);
            }

            isLoaded = true;
            foreach (var pendingAction in pendingActions) {
                pendingAction();
            }
            pendingActions.Clear();
        }

        public static void OnLoad(Action action) {
            if (isLoaded) action();
            else {
                pendingActions.Add(action);
            }
        }

        public static void SaveSettingsJson() {
            File.WriteAllText(configFilePath, JsonUtility.ToJson(config, true));
        }


        private void OnApplicationQuit() {
	        SaveSettingsJson();
        }

        private static void GenNewConfig(bool regenConfig = false) {

            //Debug.Log("Generating new configuration file...");

            NRJsonSettings temp = new NRJsonSettings();
            
            config = temp;
            isLoaded = true;

            if (File.Exists(configFilePath)) File.Delete(configFilePath);

            File.WriteAllText(configFilePath, JsonUtility.ToJson(temp, true));
            

            string destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", Application.companyName, Application.productName);

            if ((Application.platform == RuntimePlatform.LinuxEditor) || (Application.platform == RuntimePlatform.LinuxPlayer))
                destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/unity3d/" + Application.companyName + "/" + Application.productName);

            // On OSX the folder that unity creates is different based on if NotReaper is being launched in the editor/player.
            if (Application.platform == RuntimePlatform.OSXEditor)
                destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Application Support/" + Application.companyName + "/" + Application.productName);

            if (Application.platform == RuntimePlatform.OSXPlayer)
                destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Application Support/" + Application.identifier);
            
            //It's release time and I need a fix ok, don't make fun of my code.
            if (!regenConfig) {
                if (File.Exists(Path.Combine(destPath, "BG1.png"))) return;
                if (File.Exists(Path.Combine(destPath, "BG2.png"))) return;
                if (File.Exists(Path.Combine(destPath, "BG3.png"))) return;
                if (File.Exists(Path.Combine(destPath, "BG4.jpg"))) return;
            }
            
            //Copy bg images over
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG1.png"), destPath + "/BG1.png", false);
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG2.png"), destPath + "/BG2.png", false);
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG3.png"), destPath + "/BG3.png", false);
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG4.jpg"), destPath + "/BG4.jpg", false);

        }

        public static string GetbgImagePath() {
            string imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", Application.companyName, Application.productName, "BG1.png");

            if ((Application.platform == RuntimePlatform.LinuxEditor) || (Application.platform == RuntimePlatform.LinuxPlayer))
                imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/unity3d/" + Application.companyName + "/" + Application.productName + "/BG1.png");
            
            if (Application.platform == RuntimePlatform.OSXEditor)
                imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Application Support/" + Application.companyName + "/" + Application.productName + "/BG1.png");

            if (Application.platform == RuntimePlatform.OSXPlayer)
                imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Application Support/" + Application.identifier + "/BG1.png");

            return(imagePath);
        }

        public static bool GetDiscordRichPresence() {
            // Discord Manager is broken on mac.
            if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer))
                return false;

            return true;
        }
    }

    [System.Serializable]
    public class UserColor {
        public double r = 0.3;
        public double g = 0.3;
        public double b = 0.3;
    }

    [System.Serializable]
    public class NRJsonSettings {

        public Color leftColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
        public Color rightColor = new Color(1.0f, 0.47f, 0.14f, 1.0f);
        public Color selectedHighlightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color waveformColor = new Color(0f, 255 / 255f, 178 / 255f, 0.75f);

        public float mainVol = 0.5f;
        public float noteVol = 0.5f;
        public float sustainVol = 0.5f;
        public int audioDSP = 480;
        public float noteHitScale = 0.8f;
        public float bgMoveMultiplier = 1.0f;
        public bool useBouncyAnimations = true;

        public bool useAutoZOffsetWith360 = true;

        public double UIFadeDuration = 1.0f;

        public bool useDiscordRichPresence = NRSettings.GetDiscordRichPresence();
        public bool showTimeElapsed = true;

        public bool clearCacheOnStartup = true;
        public bool saveOnLoadNew = true;

        public bool singleSelectCtrl = false;

        public bool enableTraceLines = true;

        public bool enableDualines = true;

        public string cuesSavePath = "";

        public string bgImagePath = NRSettings.GetbgImagePath();

    }

}