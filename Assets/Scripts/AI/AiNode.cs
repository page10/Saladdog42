using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNode
{
    private AiPerform _aiPerform; // ai执行的方法
    public readonly AiNodeData[] NextEvents; // 接下来的ai事件


    public AiNode(AiPerform aiPerform, AiNodeData[] nextEvents)
    {
        _aiPerform = aiPerform;
        NextEvents = nextEvents;
    }
    
    /// <summary>
    /// 判断这个node是否执行完成用
    /// 这个判断是否执行完成和动画那个不一样
    /// </summary>
    /// <param name="args"></param>
    /// <returns>是否执行完成一条AiNode中的内容</returns>
    public bool UpdateNode(params object[] args)
    {
        // todo 这个和动画不一样 不是用时间判定的 可能得由node自己返回？
        return _aiPerform(args);
    }


    /// <summary>
    /// 执行AiNodeData里写的函数 并且生成一个AiNode
    /// </summary>
    /// <param name="data">ai执行内容信息</param>
    /// <returns>生成的ainode</returns>
    public static AiNode FromAiNodeData(AiNodeData data)
    {
        return new AiNode(GenAiPerform(data), data.NextEventDatas.ToArray());
    }

    public static AiPerform GenAiPerform(AiNodeData data)
    {
        return data.ThisEventData.GetPerform();
        // 移动 执行
        //     if (data.ThisEventData is MoveToGrid)  
        //     {
        //         MoveToGrid m = (MoveToGrid)data.ThisEventData;
        //         return (parameters) =>
        //         {
        //             // 它没法寻路 character身上没有寻路功能 应该是gameManager拿好了给他路径
        //             // 根据拿到的character和grid 改变character的位置 并且播动画
        //             // 播完动画之后返回true
        //             
        //             List<Vector2Int> path = new List<Vector2Int>();
        //             // todo 这里应该要修改对应character的gpos 但是这里拿到的是个struct 可以在这直接改character吗
        //             // 难道要让gamemanager做？感觉不对 gamemanager应该不是在这层调 是在aiaction里
        //             
        //             if (m.Nodes != null)  // 放进path里
        //             {
        //                 for (int i = 0; i < m.Nodes.Length; i++)
        //                 {
        //                     path.Add(m.Nodes[i]);
        //                 }
        //             }
        //             
        //             m.Character.gPos.grid = path[path.Count - 1];
        //             m.Character.animator.StartMove(path);
        //             return true;
        //         };
        //     }
        //
        //     if (data.ThisEventData is AttackOrHeal)
        //     {
        //         AttackOrHeal a = (AttackOrHeal)data.ThisEventData;
        //         Debug.Log("attack or heal");
        //         // todo 发起攻击 并且播动画
        //         return (parameters) => { return true; };
        //     }
        //
        //     return (args) => { return true; };
        // }
    }
}

/// <summary>
/// 暂时还不知道要多少参数
/// 只知道是个bool 返回这个ai流程是不是执行完了
/// </summary>
public delegate bool AiPerform(params object[] args);

// 这部分写完之后应该是写action里传数据的部分

