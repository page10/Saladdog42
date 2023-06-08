using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNode 
{
    private AiPerform _aiPerform;  // ai执行的方法
    public readonly AiNodeData[] NextEvents;  // 接下来的ai事件
    
    public static AiPerform GenAiPerform(AiNodeData data)
    {
        if (data.ThisEventData is MoveToGrid)
        {
            MoveToGrid m = (MoveToGrid)data.ThisEventData;
            return (parameters) => { return true; };
        }

        if (data.ThisEventData is AttackOrHeal)
        {
            AttackOrHeal a = (AttackOrHeal)data.ThisEventData;
            return (parameters) => { return true; };
        }

        return (args) => { return true; };
    }
}

/// <summary>
/// 暂时还不知道要多少参数
/// 只知道是个bool 返回这个ai流程是不是执行完了
/// </summary>
public delegate bool AiPerform(params object[] args);

