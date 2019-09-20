using System.Runtime.CompilerServices;
using System;
using System.Net;
using System.IO;
using UnityEngine;


namespace NotReaper {

    public class NRSettings : MonoBehaviour {

        public static NRJsonSettings config = new NRJsonSettings();

        public static bool isLoaded = false;

        private static readonly string configFilePath = Path.Combine(Application.persistentDataPath, "NRConfig.json");

        public static void LoadSettingsJson(bool regenConfig = false) {
            
            //If it doesn't exist, we need to gen a new one.
            if (regenConfig || !File.Exists(configFilePath)) {
                GenNewConfig();
            }

            try {
                config = JsonUtility.FromJson<NRJsonSettings>(File.ReadAllText(configFilePath));
            } catch(Exception e) {
                Debug.LogError(e);
            }

            config.leftColor = new Color((float)config.userLeftColor.r, (float)config.userLeftColor.g, (float)config.userLeftColor.b);
            config.rightColor = new Color((float)config.userRightColor.r, (float)config.userRightColor.g, (float)config.userRightColor.b);
            config.selectedHighlightColor = new Color((float)config.userSelectedHighlightColor.r, (float)config.userSelectedHighlightColor.g, (float)config.userSelectedHighlightColor.b);


            isLoaded = true;

            

        }

        public static void SaveSettingsJson() {
            File.WriteAllText(configFilePath, JsonUtility.ToJson(config, true));
        }


        private static void GenNewConfig() {

            Debug.Log("Generating new configuration file...");

            NRJsonSettings temp = new NRJsonSettings();
            
            File.WriteAllText(configFilePath, JsonUtility.ToJson(temp, true));

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
        public UserColor userLeftColor = new UserColor() {
            r = 0.05,
            g = 0.6,
            b = 0.8
        };
        public Color leftColor;
        
        public UserColor userRightColor = new UserColor() {
            r = 0.9,
            g = 0.5,
            b = 0.05
        };
        public Color rightColor;
        
        public UserColor userSelectedHighlightColor = new UserColor() {
            r = 1.0,
            g = 0.5,
            b = 0.0
        };
        public Color selectedHighlightColor;

        public double mainVol = 0.5f;
        public double noteVol = 0.5f;
        public double sustainVol = 0.5f;

        public double UIFadeDuration = 1.0f;

        public bool useDiscordRichPresence = true;
        public bool showTimeElapsed = true;
    }

}