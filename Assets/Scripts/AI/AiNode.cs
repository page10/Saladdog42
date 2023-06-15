using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNode
{
    private CharacterObject _character; //谁执行这个ai
    private AiPerform _aiPerform; // ai执行的方法
    public readonly AiNodeData[] NextEvents; // 接下来的ai事件
    private float _timeElapsed = 0; //运行了多久了

    public AiNode(CharacterObject character,AiPerform aiPerform, AiNodeData[] nextEvents)
    {
        _character = character;
        _aiPerform = aiPerform;
        NextEvents = nextEvents;
    }
    
    /// <summary>
    /// 判断这个node是否执行完成用
    /// 这个判断是否执行完成和动画那个不一样
    /// </summary>
    /// <param name="deltaTime">这一tick过了多久</param>
    /// <param name="args"></param>
    /// <returns>是否执行完成一条AiNode中的内容</returns>
    public bool UpdateNode(float deltaTime, params object[] args)
    {
        bool done = _aiPerform(_character, _timeElapsed, args);
        _timeElapsed += deltaTime;
        return done;
    }


    /// <summary>
    /// 执行AiNodeData里写的函数 并且生成一个AiNode
    /// </summary>
    /// <param name="character">执行ai的角色</param>
    /// <param name="data">ai执行内容信息</param>
    /// <returns>生成的ainode</returns>
    public static AiNode FromAiNodeData(CharacterObject character, AiNodeData data)
    {
        return new AiNode(character, GenAiPerform(data), data.NextEventDatas.ToArray());
    }

    public static AiPerform GenAiPerform(AiNodeData data)
    {
        return data.ThisEventData.GetPerform();
    }
}

/// <summary>
/// 参数：
/// character：执行这个行为的角色
/// timeElapsed：表演了多少秒了
/// args：特定参数，留着备用
/// 只知道是个bool 返回这个ai流程是不是执行完了
/// </summary>
public delegate bool AiPerform(CharacterObject character, float timeElapsed, params object[] args);

// 这部分写完之后应该是写action里传数据的部分

