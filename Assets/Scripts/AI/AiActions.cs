using System;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public static class AiActions
{

    public static Dictionary<string,AIAction> AIActionsDict = new Dictionary<string, AIAction>()
    {
        {"moveToNearestEnemy",moveToNearestEnemy},
        {"attackEnemy",attackEnemy},
        // {"GetAttackableCharacterObjects", GetAttackableCharacterObjects}  // aiAction应该是个完整的东西 不该是一小步 所以不应该有这个
    };

    /// <summary>
    /// 移动到攻击范围内最近的敌人
    /// </summary>
    /// <param name="characterObj">移动执行者</param>
    public static void moveToNearestEnemy(CharacterObject characterObj)
    {
        Debug.Log("move to nearest enemy!");
        characterObj.hasMoved = true;
    }

    /// <summary>
    /// 攻击敌人
    /// </summary>
    /// <param name="characterObj">攻击执行者</param>
    public static void attackEnemy(CharacterObject selectedCharacterObject)
    {
        //todo 0607 差一个aiNode结构 在这个函数里只应该生成node信息 然后gamemanager里另外有一个去执行这些node里函数的东西
        // 拿到所有的可攻击敌人 根据当前的武器
        List<CharacterObject> AttackableCharacterObjects = GameState.GetAttackableCharacterObjects(selectedCharacterObject); 
        Debug.Log("attack enemy!");
        
        // 根据血量排序可攻击敌人
        AttackableCharacterObjects.Sort((a, b) => a.CharacterResource.hp.CompareTo(b.CharacterResource.hp));
        
        // 选择血量最少的敌人
        CharacterObject targetCharacterObject = AttackableCharacterObjects[0];
        
        // 对敌人发动攻击

        BattleInputInfo battleInputInfo = new BattleInputInfo();
        battleInputInfo.attacker = selectedCharacterObject;  // 我是攻击者
        battleInputInfo.defender = targetCharacterObject;  // 受攻击的敌人
        battleInputInfo.attackerWeapon =
            selectedCharacterObject.attack.Weapons[selectedCharacterObject.attack.weaponCurIndex];  // 我的武器
        battleInputInfo.defenderWeapon =
            targetCharacterObject.attack.Weapons[targetCharacterObject.attack.weaponCurIndex];  // 敌人的武器
        battleInputInfo.attackerPos =
            new Vector2Int(selectedCharacterObject.gPos.grid.x, selectedCharacterObject.gPos.grid.y);  // 我的位置
        battleInputInfo.defenderPos =
            new Vector2Int(targetCharacterObject.gPos.grid.x, targetCharacterObject.gPos.grid.y);  // 敌人的位置
        
        // 我是用gamestate存一个map的引用还是把地形的修改以某种资源形式存在角色身上呢 就类似hp那种的当前状态 每次移动完之后根据地形格子重新计算
        // 存在角色身上可以但是没必要 如果角色站在森林上森林被烧了就不是森林了 这时候也要重算 不如每次要算的时候直接算就好了
        battleInputInfo.attackerTerrainStatus = GameState.gameManager.mapGenerator
            .Map[selectedCharacterObject.gPos.grid.x, selectedCharacterObject.gPos.grid.y].percentageModifier;  // 我的地形
        battleInputInfo.defenderTerrainStatus = GameState.gameManager.mapGenerator
            .Map[targetCharacterObject.gPos.grid.x, targetCharacterObject.gPos.grid.y].percentageModifier;  // 敌人的地形
        battleInputInfo.isSameSide = selectedCharacterObject.slaveTo.masterPlayerIndex ==
                                     targetCharacterObject.slaveTo.masterPlayerIndex;  // 是不是同一边
        
        
        GameState.gameManager.battleManager.StartBattle(battleInputInfo);
        GameState.gameManager.battleManager.StartBattleAnim();
 
        selectedCharacterObject.hasAttacked = true;  // 攻击完了 改一下标签
        
        
        // 在玩家阶段 这里跳转到了一个PlayBattleAnimation的状态
        // 对于敌人 应该也有一个播动画的状态但不是和玩家这个一样的东西
        

    }

    
}