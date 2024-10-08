using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这部分的结构 对应ai实现时候各个函数所需要的参数
// 这部分只负责「要干什么」 至于「获取某个信息」不应该是它在干
// 而没有 筛选敌人 判断背包里有没有能吃的东西 这类应该是脑子 应该是aiAction里 aiAction里是思考我要干什么 并且由gameManager发布指令

public interface IAiActionData
{
    /// <summary>
    /// 这个接口实现 if (x is xxx)要做的事情
    /// </summary>
    /// <returns></returns>
    public AiPerform GetPerform();
}

/// <summary>
/// 等待一定时间
/// </summary>
public class AIWait : IAiActionData
{
    public float WaitSec;

    public AIWait(float waitSec)
    {
        WaitSec = waitSec;
    }
    
    public AiPerform GetPerform()
    {
        return (character, e, args) => e >= WaitSec;
    }
}

/// <summary>
/// 移动到某个格子
/// </summary>
public class MoveToGrid : IAiActionData
{
    /// <summary>
    /// 要移动的那个角色
    /// </summary>
    public CharacterObject Character;
    /// <summary>
    /// 移动路径 
    /// 其实只要目标点就行了，路径可以是移动系统里面算出来，不归我AI管理
    /// </summary>
    public Vector2Int TargetGrid;
    
    public MoveToGrid(CharacterObject character, Vector2Int targetGrid)
    {
        Character = character;
        TargetGrid = targetGrid;
    }

    //todo 
    public AiPerform GetPerform()
    {
        //todo 执行
        return (character, elapsed, args) =>
        {
            if (elapsed <= 0)
            {
                //刚开始的时候让角色开始移动 
                Debug.Log("ai move to grid: " + TargetGrid + " from " + character.GetComponent<GridPosition>().grid);
                List<Vector2Int> mg;
                GameState.gameManager.StartCharacterMove(character, TargetGrid, out mg);
                return false;
            }
            else
            {
                //之后就是等待结果
                return character.IsMovingAnimDone();
            }
        };
    }
}


/// <summary>
/// 攻击或治疗某个目标
/// 先写成武器index吧 之后可能写成武器weaponObj（似乎没有index好）
/// </summary>
public class AttackOrHeal: IAiActionData
{
    public CharacterObject Character;
    public CharacterObject Target;
    /// <summary>
    /// 使用的武器index
    /// </summary>
    public int WeaponIndex;  
    
    public AttackOrHeal(CharacterObject character, CharacterObject target, int weaponIndex)
    {
        Character = character;
        Target = target;
        WeaponIndex = weaponIndex;
    }
    
    //todo 
    public AiPerform GetPerform()
    {
        //todo 执行
        return (character, elapsed, args) =>
        {
            Debug.Log("ai attack or heal:" + Target);
            GameState.gameManager.StartAttack(character, Target);
            return true;
        };
    }
}

// 如果还有什么行为就接着加

/// <summary>
/// 生成AiNode所用的数据
/// 执行行为所需要的参数 和执行的方法
/// </summary>
public class AiNodeData
{
    /// <summary>
    /// 要做的事情 包括参数和执行方法
    /// </summary>
    public IAiActionData ThisEventData;
    /// <summary>
    /// 接下来的节点
    /// </summary>
    public List<AiNodeData> NextEventDatas;
    
    public AiNodeData(IAiActionData thisEventData, List<AiNodeData> nextEventDatas)
    {
        ThisEventData = thisEventData;
        NextEventDatas = nextEventDatas;
    }

    public static AiNodeData Empty = new AiNodeData(null, new List<AiNodeData>());
}
