using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NotReaper {

    public class NRSettings : MonoBehaviour {

        public static NRJsonSettings config = new NRJsonSettings();

        public static bool isLoaded = false;

        private static readonly string configFilePath = Path.Combine(Application.persistentDataPath, "NRConfig.txt");


        private static bool failsafeThingy = false;

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
                return;
            }

            try {
                config = JsonUtility.FromJson<NRJsonSettings>(File.ReadAllText(configFilePath));
            } catch (Exception e) {
                Debug.LogError(e);
            }

            //config.leftColor = new Color((float)config.userLeftColor.r, (float)config.userLeftColor.g, (float)config.userLeftColor.b);
            //config.rightColor = new Color((float)config.userRightColor.r, (float)config.userRightColor.g, (float)config.userRightColor.b);
            //config.selectedHighlightColor = new Color((float)config.userSelectedHighlightColor.r, (float)config.userSelectedHighlightColor.g, (float)config.userSelectedHighlightColor.b);


            isLoaded = true;

        }



        public static void SaveSettingsJson() {
            File.WriteAllText(configFilePath, JsonUtility.ToJson(config, true));
        }


        private static void GenNewConfig() {

            //Debug.Log("Generating new configuration file...");

            NRJsonSettings temp = new NRJsonSettings();
            
            config = temp;
            isLoaded = true;

            if (File.Exists(configFilePath)) File.Delete(configFilePath);

            File.WriteAllText(configFilePath, JsonUtility.ToJson(temp, true));
            

            string destPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper");
            
            //It's release time and I need a fix ok, don't make fun of my code.
            if (File.Exists(Path.Combine(destPath, "BG1.png"))) return;
            if (File.Exists(Path.Combine(destPath, "BG2.png"))) return;
            if (File.Exists(Path.Combine(destPath, "BG3.png"))) return;
            if (File.Exists(Path.Combine(destPath, "BG4.jpg"))) return;
            
            //Copy bg images over
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG1.png"), destPath + "/BG1.png", false);
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG2.png"), destPath + "/BG2.png", false);
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG3.png"), destPath + "/BG3.png", false);
            File.Copy(Path.Combine(Application.streamingAssetsPath, "BG4.jpg"), destPath + "/BG4.jpg", false);

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

        public double mainVol = 0.5f;
        public double noteVol = 0.5f;
        public double sustainVol = 0.5f;

        public double UIFadeDuration = 1.0f;

        public bool useDiscordRichPresence = true;
        public bool showTimeElapsed = true;

        public string bgImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "CircuitCubed", "NotReaper", "BG1.png");
    }

}