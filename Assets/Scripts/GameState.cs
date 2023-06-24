using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public enum GameControlState
{
    NewTurn,  // New turn
    /// <summary>
    /// 如果选中了我方角色
    /// 优先判定是不是可移动
    /// 如果可移动 则展开移动和攻击范围 并进入「ShowRange」
    /// 不可移动状态下 判定是否可攻击
    /// 如果可攻击 进入攻击武器选择状态「WeaponSelect」
    /// 如果不可攻击 则什么事情都不发生
    /// （可移动不可攻击的判定在「ShowRange」里面处理）
    /// </summary>
    SelectCharacter,  // select one focused character
    ShowRange,  // showing moverange prefabs 
    CharacterMoving,  //character moving to target position
    /// <summary>
    /// 选择武器 并展示对应的攻击距离
    /// 选择攻击对象
    /// 选择完成后展示攻击预览 并且进入攻击确认状态「ConfirmWeapon」
    /// </summary>
    WeaponSelect,
    ShowAttackableArea,  // 显示可以移动去的攻击位置
    ShowCommandMenu, // 指令菜单显示
    /// <summary>
    /// 展示攻击预览
    /// 确认是否要用选中的武器发动攻击
    /// 可以切换武器
    /// 确认攻击则跳转
    /// </summary>
    ConfirmWeapon,
    // Attack,
    PlayBattleAnimation,
    CharacterActionDone, 
    EnemyTurn,
    EndTurn,
    /// <summary>
    /// 敌方攻击动画
    /// 如果播放动画结束就回到「EnemyTurn」 进行下一个人的逻辑
    /// 也许可以加一个按键跳过的功能
    /// </summary>
    EnemyExecuteAi,
}
public static class GameState 
{
    public static GameControlState gameControlState = GameControlState.SelectCharacter;
    public static GameManager gameManager;

    /// <summary>
    /// 返回攻击范围内是不是有可以攻击或者治疗的对象
    /// </summary>
    /// <param name="selectedCharacterObject">攻击实行方</param>
    /// <returns>所有的可攻击或治疗对象对应byte</returns>
    public static byte GetAttackableCharacters(CharacterObject selectedCharacterObject)
    {
        return gameManager.CheckAbilities(selectedCharacterObject);  // CheckAbilities 是用来检查攻击范围内是不是有对象的
    }
    
    /// <summary>
    /// 拿到当前使用的武器攻击范围内所有敌方对象
    /// </summary>
    /// <param name="selectedCharacterObject"></param>
    /// <returns></returns>
    public static List<CharacterObject> GetAttackableCharacterObjects(CharacterObject selectedCharacterObject)
    {
        return gameManager.GetAttackableCharacterObjects(selectedCharacterObject);
    }
    
}
