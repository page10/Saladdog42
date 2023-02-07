using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameControlState
{
    NewTurn,  // New turn
    SelectCharacter,  // select one focused character
    ShowRange,  // showing moverange prefabs 
    CharacterMoving,  //character moving to target position
    WeaponSelect,
    SelectAttackObject,
    ConfirmWeapon,
    Attack,
    CharacterMoveDone, 
    EnemyTurnStart,
    EndTurn,
}
public static class GameState 
{
    public static GameControlState gameControlState = GameControlState.SelectCharacter;
}
