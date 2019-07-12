using UnityEngine;

namespace NotReaper.Settings {


    [CreateAssetMenu(fileName = "ProgramSettings", menuName = "NotReaper/ProgramSettings", order = 0)]
    public class ProgramSettings : ScriptableObject {

        public Color userLeftColor = Color.red;
        public Color userRightColor = Color.blue;
        public Color bothColor { get; set; } = Color.gray;
        public Color neitherColor { get; set; } = Color.magenta;

    }
}