using UnityEngine;

public enum TargetHandType { Either = 0, Right = 1, Left = 2, None = 3 }
public enum TargetBehavior { Standard = 0, Vertical = 1, Horizontal = 2, Hold = 3, ChainStart = 4, Chain = 5, Melee = 6, HoldEnd = 7}
public enum TargetVelocity { Standard = 20, Vertical = 20, Horizontal = 20, Hold = 20, ChainStart = 1, Chain = 2, Melee = 3 }

[System.Serializable]
public class Cue
{
    public int tick;
    public int tickLength;
    public int pitch;
    public TargetVelocity velocity = TargetVelocity.Standard;
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

