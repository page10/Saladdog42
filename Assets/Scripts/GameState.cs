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
    ShowAttackableArea,  // 显示可以移动去的攻击位置
    ShowCommandMenu, // 指令菜单显示
    ConfirmWeapon,
    Attack,
    PlayBattleAnimation,
    CharacterActionDone, 
    EnemyTurnStart,
    EndTurn,
}
public static class GameState 
{
    public static GameControlState gameControlState = GameControlState.SelectCharacter;
}
