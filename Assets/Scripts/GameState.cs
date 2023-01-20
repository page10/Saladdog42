using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameControlState
{
    NewTurn,  // New turn
    SelectCharacter,  // select one focused character
    ShowMoveRange,  // showing moverange prefabs 
    CharacterMoving,  //character moving to target position
}
public static class GameState 
{
    public static GameControlState gameControlState = GameControlState.SelectCharacter;
}
