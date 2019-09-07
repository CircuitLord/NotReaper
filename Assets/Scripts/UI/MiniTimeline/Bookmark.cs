using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace NotReaper.UI {

    public class Bookmark : MonoBehaviour {

        public int index = 0;

        public TextMeshProUGUI text;

        public double percentBookmark = 0;

        public void SetIndex(int i) {
            index = i;
            text.text = index.ToString();

        }


        // Start is called before the first frame update

    }

}