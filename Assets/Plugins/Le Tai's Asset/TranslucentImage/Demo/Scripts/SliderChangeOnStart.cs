using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderChangeOnStart : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();

        slider.value -= 1;
        slider.value += 1;
    }
}
