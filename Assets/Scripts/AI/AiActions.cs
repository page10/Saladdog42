using System;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 被game Manager调用了角色的AI（ExecuteAi函数）而调用
/// 返回得到aiNodeData，然后根据aiNodeData来执行
/// </summary>
public static class AiActions
{
    public static Dictionary<string,AIAction> AIActionsDict = new Dictionary<string, AIAction>()
    {
        {"moveToNearestEnemy",moveToNearestEnemy},
        {"attackEnemy",attackEnemy},
        // {"GetAttackableCharacterObjects", GetAttackableCharacterObjects}  // aiAction应该是个完整的东西 不该是一小步 所以不应该有这个
    };

    // 移动和攻击应该是要单写的 
    /// <summary>
    /// 移动到攻击范围内最近的敌人
    /// </summary>
    /// <param name="characterObj">移动执行者</param>
    public static AiNodeData moveToNearestEnemy(in CharacterObject characterObj)
    {
        Debug.Log("move to nearest enemy!");
        
        //characterObj.hasMoved = true; //不能在这里做，因为这是要执行的事情

        //得到所有敌人
        List<CharacterObject> enemies = GameState.gameManager.AllEnemies(characterObj);
        //得到我能走到的范围
        List<Vector2Int> myGrids = GameState.gameManager.CanMoveToGrids(characterObj);

        //得出最近的一个
        int distance = int.MaxValue;
        Vector2Int targetGrid = characterObj.gPos.grid;
        foreach (CharacterObject enemy in enemies)
        {
            //todo 筛选出我能走到的最近的格子
        }
        
        //todo 临时试一下状态机的，看看上下左右，哪儿能走，就走哪儿
        //筛选出能走的格子（假如1格肯定移动力足够）
        List<Vector2Int> canMoveGrids = GameState.gameManager.GetCharacterCanMoveToArea(characterObj);

        Vector2Int res = canMoveGrids[Random.Range(0, canMoveGrids.Count)];
        
        AiNodeData aiNodeData = new AiNodeData(new MoveToGrid(characterObj, res), new List<AiNodeData>());
        return aiNodeData;
    }

    /// <summary>
    /// 攻击敌人
    /// </summary>
    /// <param name="characterObj">攻击执行者</param>
    public static AiNodeData attackEnemy(in CharacterObject selectedCharacterObject)
    {
        //todo 0607 差一个aiNode结构 在这个函数里只应该生成node信息 然后gamemanager里另外有一个去执行这些node里函数的东西
        // 拿到所有的可攻击敌人 根据当前的武器
        List<CharacterObject> AttackableCharacterObjects = GameState.GetAttackableCharacterObjects(selectedCharacterObject); 
        Debug.Log("attack enemy!");
        
        // 根据血量排序可攻击敌人
        AttackableCharacterObjects.Sort((a, b) => a.CharacterResource.hp.CompareTo(b.CharacterResource.hp));
        
        // 选择血量最少的敌人
        CharacterObject targetCharacterObject = AttackableCharacterObjects[0];
        
        // 在玩家阶段 这里跳转到了一个PlayBattleAnimation的状态
        // 对于敌人 应该也有一个播动画的状态但不是和玩家这个一样的东西
        
        AiNodeData aiNodeData = new AiNodeData(new AttackOrHeal(selectedCharacterObject, targetCharacterObject,selectedCharacterObject.attack.weaponCurIndex), new List<AiNodeData>());
        return aiNodeData;
    }

    
}