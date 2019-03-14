using UnityEngine;

public enum TargetHandType { Either = 0, Right = 1, Left = 2, None = 3 }
public enum TargetBehavior { Standard = 0, Vertical = 1, Horizontal = 2, Hold = 3, ChainStart = 4, Chain = 5, Melee = 6 }

[System.Serializable]
public class Cue
{
    public int tick;
    public int tickLength;
    public int pitch;
    public int velocity;
    public GridOffset gridOffset = new GridOffset { x = 0, y = 0};
    public TargetHandType handType = TargetHandType.Right;
    public TargetBehavior behavior = TargetBehavior.Standard;

    [System.Serializable]
    public struct GridOffset
    {
        public double x;
        public double y;
    }
}

