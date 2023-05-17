using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一条「战斗动画」节点
/// 一个「节点」是：一定时间内执行一件事情 并且返回是否执行完毕
/// 在这里 之所以是「战斗动画」节点 是因为 执行的函数序列里 包含了「播动画」
/// </summary>
public class BattleAnimNode
{
    private float _timeElapsed = 0;
    private AnimationPerform _event;
    public readonly BattleAnimData[] NextEvents;
    
    /// <summary>
    /// 根据外层需要 在外层调用的时候执行
    /// 在这个项目语境中 外层每帧update时调用一次它
    /// 因此这里是每帧执行
    /// 并且返回是否已经完成
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns>是否已经完成</returns>
    public bool UpdateNode(float deltaTime)
    {
        bool res = _event == null ? true : _event(_timeElapsed, deltaTime);
        _timeElapsed += deltaTime;
        return res;
    }
    
    public BattleAnimNode(AnimationPerform eve, BattleAnimData[] nextEvents)
    {
        _timeElapsed = 0;
        _event = eve;
        NextEvents = nextEvents;
    }
    
    /// <summary>
    /// 根据战斗动画信息生成战斗动画BattleAnimNode
    /// </summary>
    /// <param name="eve"></param>
    /// <returns></returns>
    public static BattleAnimNode FromBattleAnimData(BattleAnimData eve)
    {
        return new BattleAnimNode(GetAnimationPerform(eve),eve.NextEventsDatas.ToArray());
    }

    /// <summary>
    /// 根据动画信息 决定它具体要做什么
    /// </summary>
    /// <param name="eve">拿到的战斗动画信息</param>
    /// <returns>要做的那个函数</returns>
    public static AnimationPerform GetAnimationPerform(BattleAnimData eve)
    {
        if (eve.ThisEventData is Wait)
        {
            Wait w = (Wait)eve.ThisEventData;
            return (elapsed, time, parameters) => elapsed >= w.Second;
        }
        if (eve.ThisEventData is CharacterDoAction)
        {
            CharacterDoAction c = (CharacterDoAction)eve.ThisEventData;
            return (elapsed, time, objects) =>
            {
                c.Character.animator.PlayAnimation(c.ActionId);
                return true;
            };
        }
        if (eve.ThisEventData is PopText)
        {
            PopText p = (PopText)eve.ThisEventData;
            
            return (elapsed, time, objects) =>
            {
                return elapsed > 1.0f;
                // TODO: 之后要写弹出文字 但不是UI 
                return true;
            };
        }
        if (eve.ThisEventData is RemoveCharacter)
        {
            RemoveCharacter r = (RemoveCharacter)eve.ThisEventData;
            return (elapsed, time, objects) =>
            {
                r.Character.CanBeDestroyed = true;  // 不是在运算的时候死亡 而是在动画播放完毕后死亡
                r.Character.animator.RemoveCharacter();
                return true;
            };
        }
        if (eve.ThisEventData is ChangeFaceDirection)
        {
            ChangeFaceDirection c = (ChangeFaceDirection)eve.ThisEventData;
            return (elapsed, time, objects) =>
            {
                c.Character.animator.SetFaceDirection(c.Direction);
                return true;
            };
        }
        if (eve.ThisEventData is CharacterMove)
        {
            CharacterMove c = (CharacterMove)eve.ThisEventData;
            return (elapsed, time, objects) =>
            {
                List<Vector2Int> path = new List<Vector2Int>();
                if (c.Nodes != null)
                {
                    for (int i = 0; i < c.Nodes.Length; i++)
                    {
                        path.Add(c.Nodes[i]);
                    }
                }
                c.Character.animator.StartMove(path);
                return true;
            };
        }
        return (elapsed, time, objects) => true;
    }
    
}

/// <summary>
/// 这个战斗动作具体做什么
/// timeElapsed: 总共已经运行了多少秒
/// deltaTime: 本次运行了多少秒
/// 给它这两个参数 可以看它的返回值
/// 返回值为true代表战斗动作结束了
/// </summary>
public delegate bool AnimationPerform(float timeElapsed, float deltaTime, params object[] param);