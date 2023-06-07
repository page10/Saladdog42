using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这部分的结构 对应ai实现时候各个函数所需要的参数


/// <summary>
/// 移动到某个格子
/// </summary>
public struct MoveToGrid
{
    public CharacterObject Character;
    public Vector2Int Grid;
}

public struct MoveToEnemy
{
    public CharacterObject Character;
    public CharacterObject Enemy;
}

public struct GetLowestHpEnemy
{
    public CharacterObject[] Enemies;
}

/// <summary>
/// todo 0608 不应该把「找敌人」和「向敌人发起攻击」分开写吧
/// 这样的话找到的那个敌人不知道该怎么传诶 看起来目前的结构就没法传了
/// 应该「找到最近的人并攻击」在这里是一个struct 对应到ai实现时候是一个完整的函数
/// </summary>
public struct GetNearstEnemy
{
    public CharacterObject[] Enemies;
}

/// <summary>
/// 生成AiNode所用的数据
/// </summary>
public class AiNodeData
{
    public object ThisEventData;
    public List<AiNodeData> NextEventDatas;
    
    public AiNodeData(object thisEventData, List<AiNodeData> nextEventDatas)
    {
        ThisEventData = thisEventData;
        NextEventDatas = nextEventDatas;
    }
}
