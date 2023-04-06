using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// 角色做一个动作
    /// </summary>
    public struct CharacterDoAction
    {
        public CharacterObject Character;
        public string ActionId;
        public const string Attack = "Attack";
        public const string Hurt = "Hurt";
        public CharacterDoAction(CharacterObject character, string actionId)
        {
            Character = character;
            ActionId = actionId;
        }
    }
   
    /// <summary>
    /// 角色发生位移，比如挨打后往后推一下，如果需要的话
    /// </summary>
    public struct CharacterMove
    {
        public CharacterObject Character;
        public Vector2Int[] Nodes;
        public float Speed;
        public int Index;
    }
    
    /// <summary>
    /// 在角色身上播放特效
    /// </summary>
    public struct PlayVFXOnCharacter
    {
        public CharacterObject Character;
        public string VFXUniqueId;
        public string VFXName;
        public bool Infinity;
        public float Duration;
    }
    
    /// <summary>
    /// 停止一个角色身上的特效
    /// </summary>
    public struct StopVFXOnCharacter
    {
        public CharacterObject Character;
        public string VFXUniqueId;
    }
    
    /// <summary>
    /// 在角色身上播放特效
    /// </summary>
    public struct PlayVFXInGrid
    {
        public Vector2Int[] Grid;
        public string VFXUniqueId;
        public string VFXName;
        public bool Infinity;
        public float Duration;
    }
    
    /// <summary>
    /// 停止一个角色身上的特效
    /// </summary>
    public struct StopVFXInGrid
    {
        public Vector2Int[] Grid;
        public string VFXUniqueId;
    }

    
    /// <summary>
    /// 等待多少时间
    /// </summary>
    public struct Wait
    {
        public float Second;
        public Wait(float second)
        {
            Second = second;
        }
    }

    /// <summary>
    /// 跳字
    /// </summary>
    public struct PopText
    {
        public CharacterObject OnCharacter;
        public string Text;
        public Color Color;
        public static readonly Color Hit = Color.red;
        public static readonly Color Miss = Color.white;
        public PopText(CharacterObject onCharacter, string text, Color color)
        {
            OnCharacter = onCharacter;
            Text = text;
            Color = color;
        }
    }

    /// <summary>
    /// 角色挂掉要移除
    /// </summary>
    public struct RemoveCharacter 
    {
        public CharacterObject Character;
        public RemoveCharacter(CharacterObject character)
        {
            Character = character;
        }
    }

    /// <summary>
    /// 改变角色面向
    /// </summary>
    public struct ChangeFaceDirection
    {
        public CharacterObject Character;
        public Vector2Int Direction;
        public static readonly Vector2Int Default = Vector2Int.down;
        public ChangeFaceDirection(CharacterObject character, Vector2Int direction)
        {
            Character = character;
            Direction = direction;
        }
    }

    /// <summary>
    /// 当前的event 和接下来的event list
    /// </summary>
    public class BattleAnimData
    {
        public object ThisEventData;
        public List<BattleAnimData> NextEventsDatas;

        public BattleAnimData(object thisEventData)
        {
            ThisEventData = thisEventData;
            NextEventsDatas = new List<BattleAnimData>();
        }
    }

    
    
    
