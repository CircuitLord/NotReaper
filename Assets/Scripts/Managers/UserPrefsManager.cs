using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NotReaper {


    public class UserPrefsManager : MonoBehaviour {

        public static Color leftColor { get; set; } = new Color(1f, 0.4f, 0.67f);
        public static Color rightColor { get; set; } = new Color(0f, 0.67f, 0.87f);
        public static Color bothColor { get; set; } = Color.gray;
        public static Color neitherColor { get; set; } = Color.magenta;

        public static Color selectedColor { get; set; } = Color.yellow;

        public static float mouseHoldForDrag { get; set; } = 15f;

        public static float mainVol { get; set; } = 0.5f;
        public static float noteVol { get; set; } = 0.5f;
        public static float sustainVol { get; set; } = 0.5f;


        //public Image LImage;
        //public Image RImage;

        public void Start() {
            //LImage.color = leftColor;
            //RImage.color = rightColor;
        }

        public static void ChangeColors() {
            //leftColor = Color.yellow;
        }


    }
}