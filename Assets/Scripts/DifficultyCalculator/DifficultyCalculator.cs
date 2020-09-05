using NAudio.Gui.TrackView;
using NotReaper;
using NotReaper.Models;
using NotReaper.Targets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DifficultyCalculator
{
    public static float GetRatingFromCurrentSong()
    {
        var currentDifficulty = new CalculatedDifficulty(Timeline.orderedNotes);
        return currentDifficulty.difficultyRating;
    }

}
public class CalculatedDifficulty
{
    public static float spacingMultiplier = 1f;
    public static float lengthMultiplier = 0.7f;
    public static float densityMultiplier = 1f;
    public static float readabilityMultiplier = 1.2f;

    public float difficultyRating;
    public float spacing;
    public float density;
    public float readability;

    float length;

    public CalculatedDifficulty(List<Target> targets)
    {
        EvaluateCues(targets);
    }

    public static Dictionary<TargetBehavior, float> objectDifficultyModifier = new Dictionary<TargetBehavior, float>()
    {
        { TargetBehavior.Standard, 1f },
        { TargetBehavior.Vertical, 1.2f },
        { TargetBehavior.Horizontal, 1.3f },
        { TargetBehavior.Hold, 1f },
        { TargetBehavior.ChainStart, 1.2f },
        { TargetBehavior.Chain, 0.2f },
        { TargetBehavior.Melee, 0.6f }
    };

    List<Target> leftHandCues = new List<Target>();
    List<Target> rightHandCues = new List<Target>();
    List<Target> eitherHandCues = new List<Target>();
    List<Target> allCues = new List<Target>();

    public void EvaluateCues(List<Target> targets)
    {
        var timeline = GameObject.FindObjectOfType<Timeline>();
        this.length = (timeline.TimestampToSeconds(targets.Last().data.time) - timeline.TimestampToSeconds(targets.First().data.time)) * 1000f;
        if (targets.Count >= 15 && this.length > 30000f)
        {
            SplitCues(targets);
            CalculateSpacing();
            CalculateDensity();
            CalculateReadability();
            difficultyRating = ((spacing + readability) / length) * 500f + (length / 100000f * lengthMultiplier);
        }
        else difficultyRating = 0f;

        Debug.Log($"Length: {length.ToString()}");
        Debug.Log($"Density: {density.ToString()}");
    }

    void CalculateReadability()
    {
        for (int i = 0; i < allCues.Count; i++)
        {
            float modifierValue = 0f;
            objectDifficultyModifier.TryGetValue(allCues[i].data.behavior, out modifierValue);
            readability += modifierValue * readabilityMultiplier;
        }
        //readability /= length;
    }

    void CalculateSpacing()
    {
        GetSpacingPerHand(leftHandCues);
        GetSpacingPerHand(rightHandCues);
        //spacing /= length;
    }

    void CalculateDensity()
    {
        density = (float) allCues.Count / length;
    }

    private void GetSpacingPerHand(List<Target> targets)
    {
        for (int i = 1; i < targets.Count; i++)
        {
            float dist = Vector2.Distance(GetTrueCoordinates(targets[i]), GetTrueCoordinates(targets[i - 1]));
            float distMultiplied = targets[i].data.behavior == TargetBehavior.Melee ? float.Epsilon :
                dist * spacingMultiplier;
            spacing += distMultiplied;
        }
    }

    Vector2 GetTrueCoordinates(Target target)
    {
        return new Vector2(target.data.x, target.data.y);
    }

    void SplitCues(List<Target> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            allCues.Add(targets[i]);
            switch (targets[i].data.handType)
            {
                case TargetHandType.Left:
                    leftHandCues.Add(targets[i]);
                    break;
                case TargetHandType.Right:
                    rightHandCues.Add(targets[i]);
                    break;
                case TargetHandType.Either:
                    eitherHandCues.Add(targets[i]);
                    break;
                default:
                    break;
            }
        }
    }
}
