using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这部分的结构 对应ai实现时候各个函数所需要的参数
// 这部分只负责「要干什么」 至于「获取某个信息」不应该是它在干
// 而没有 筛选敌人 判断背包里有没有能吃的东西 这类应该是脑子 应该是aiAction里 aiAction里是思考我要干什么 并且由gameManager发布指令


/// <summary>
/// 移动到某个格子
/// </summary>
public struct MoveToGrid
{
    public CharacterObject Character;
    public Vector2Int Grid;
}


/// <summary>
/// 攻击或治疗某个目标
/// 先写成武器index吧 之后可能写成武器weaponObj（似乎没有index好）
/// </summary>
public struct AttackOrHeal
{
    public CharacterObject Character;
    public CharacterObject Target;
    /// <summary>
    /// 使用的武器index
    /// </summary>
    public int WeaponIndex;  
}

// 如果还有什么行为就接着加

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
