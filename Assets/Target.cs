using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public TimelineTarget timelineTarget;
    public GridTarget gridTarget;

    public TargetIcon icon;

    public Cue ToCue(int offset)
    {
        int pitch = 0;
        float offsetX, offsetY = 0;
        if (gridTarget.behavior == TargetBehavior.Melee)
        {
            pitch = 98;
            if (gridTarget.transform.position.x > 0) pitch += 1;
            if (gridTarget.transform.position.y > 0) pitch += 2;

            offsetX = 0;
            offsetY = 0;
        }
        else
        {
            var x = Mathf.RoundToInt(gridTarget.transform.position.x + 5.5f);
            var y = Mathf.RoundToInt(gridTarget.transform.position.y + 3);
            pitch = x + 12 * y;
            offsetX = gridTarget.transform.position.x + 5.5f - x;
            offsetY = gridTarget.transform.position.y + 4 - y;
        }
        Cue cue = new Cue()
        {
            tick = Mathf.RoundToInt(gridTarget.transform.localPosition.z * 480f) + offset,
            tickLength = Mathf.RoundToInt(gridTarget.beatLength * 480f),
            pitch = pitch,
            velocity = gridTarget.velocity,
            gridOffset = new Cue.GridOffset { x = (float)Math.Round(offsetX, 2), y = (float)Math.Round(offsetY,2) },
            handType = gridTarget.handType,
            behavior = gridTarget.behavior          
        };
        return cue;
    }

    internal void SetHandType(TargetHandType handType)
    {
        gridTarget.handType = handType;
        gridTarget.icon.SetHandType(handType);
        timelineTarget.icon.SetHandType(handType);
    }

    internal void SetBehavior(TargetBehavior behavior)
    {
        gridTarget.behavior = behavior;
        timelineTarget.icon.SetBehavior(behavior);
        gridTarget.icon.SetBehavior(behavior);
    }

    internal void SetBeatLength(float beatLength)
    {
        gridTarget.beatLength = beatLength;
    }

    public void SetVelocity(int velocity)
    {
        gridTarget.velocity = velocity;
    }

}

