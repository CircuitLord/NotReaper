using System.Runtime.CompilerServices;
using System;
using System.Net;
using System.IO;
using UnityEngine;


namespace NotReaper {

    public static class NRSettings {

        public static NRJsonSettings config = new NRJsonSettings();

        public static bool isLoaded = false;

        private static string configFilePath = Path.Combine(Application.persistentDataPath, "NRConfig.json");

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


            isLoaded = true;

            

        }

        public static void SaveSettingsJson() {
            File.WriteAllText(configFilePath, JsonUtility.ToJson(config, true));
        }


        public static void GenNewConfig() {

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
        public UserColor userLeftColor = new UserColor();

        public Color leftColor {
            get {
                return new Color((float)userLeftColor.r, (float)userLeftColor.g, (float)userLeftColor.b);
            }
            set {
                return;
            }
        }
        public UserColor userRightColor = new UserColor();
        public Color rightColor {
            get {
                return new Color((float)userRightColor.r, (float)userRightColor.g, (float)userRightColor.b);
            }
            set {
                return;
            }
        }
        public double mainVol = 0.5f;
        public double noteVol = 0.5f;
        public double sustainVol = 0.5f;

        public double UIFadeDuration = 1.0f;
    }

}