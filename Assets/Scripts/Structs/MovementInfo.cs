using System;

/// <summary>
/// key - value pair, represents character move type and corresponding move cost 
/// also can be used to represent character movement ability 
/// </summary>
[Serializable]
public struct MovementInfo
{
    public MoveType moveType;
    public int moveCost;
    public MovementInfo(MoveType moveType, int moveCost)
    {
        this.moveType = moveType;
        this.moveCost = moveCost;
    }
}


/// <summary>
/// types of characters movement 
/// </summary>
public enum MoveType
{
    Land,
    Air,
    Mount,
}