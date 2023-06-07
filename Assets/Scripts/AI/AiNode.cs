using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNode 
{
    private AiPerform _aiPerform;  // ai执行的方法
    public readonly AiNodeData[] NextEvents;  // 接下来的ai事件
}

/// <summary>
/// 暂时还不知道要多少参数
/// 只知道是个bool 返回这个ai流程是不是执行完了
/// </summary>
public delegate bool AiPerform(params object[] args);