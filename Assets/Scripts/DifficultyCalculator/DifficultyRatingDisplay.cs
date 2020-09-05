using UnityEngine;
using System.Collections;
using TMPro;
using NotReaper.Models;

public class DifficultyRatingDisplay : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    public void UpdateText()
    {
        string rating = DifficultyCalculator.GetRatingFromCurrentSong().ToString("n2");
        text.text = $"Difficulty Rating:<color=#999>{rating}</color>";
    }
}
