using UnityEngine;
using UnityEngine.UI;

namespace NotReaper.UI {
    

    public class NRThemeable : MonoBehaviour {
        
        [SerializeField] private NRThemeableType type;

        [SerializeField]
        [Range(-0.1f, 0.1f)] private float hueModifier = 0f;

        [SerializeField]
        [Range(-0.1f, 0.1f)] private float saturationModifier = 0f;

        [SerializeField]
        [Range(-0.1f, 0.1f)] private float valueModifier = 0f;
        
        


        private void Start() {
            NRSettings.OnLoad(() => {

                var img = transform.GetComponent<Image>();

                if (img) {

                    Color newColor;
                    
                    if (type == NRThemeableType.Left) newColor = NRSettings.config.leftColor;
                    else if (type == NRThemeableType.Right) newColor = NRSettings.config.rightColor;
                    else newColor = NRSettings.config.selectedHighlightColor;
                    
                    
                    
                    float h, s, v;
                    Color.RGBToHSV(newColor, out h, out s, out v);

                    h += hueModifier;
                    s += saturationModifier;
                    v += valueModifier;

                    img.color = Color.HSVToRGB(h, s, v);



                }

            });
        }

    }






    public enum NRThemeableType {
        Left,
        Right,
        Selected
    }


}