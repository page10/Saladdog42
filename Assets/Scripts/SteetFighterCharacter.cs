using UnityEngine;

namespace DefaultNamespace
{
    public class SteetFighterCharacter
    {
        public string name;
        
        public ArtModel artModel;  // 3d 模型
        public Picture picture;    // 立绘
        
        public CharacterResources characterAttibute; // 角色资源
        public int moveSpeed; // 移动速度
        
        public Transform position; // 位置
        // public bool direction;     // 朝向 似乎没啥必要
        
        public DefendStatus defendStatus; // 防御状态
        
        public AttackStatus attackStatus; // 攻击状态
        
        public FightAction[] fightActions; // 战斗动作组
        
    }
    
    public struct CharacterResources
    {
        public int hp;             // 血量
        public int attackModifier; // 攻击力乘子 可能会因为加buff之类的改变
        public int ragePoints;     // 漫画槽
    }
    
    /// <summary>
    /// 防御状态
    /// </summary>
    public enum DefendStatus
    {
        None,
        UpperDefend,
        LowerDefend,
        Air,

    }
    
    /// <summary>
    /// 目前的攻击状态
    /// </summary>
    public struct AttackStatus
    {
        public FightAction currentAction; // 当前正在进行的动作
        public int currentActionFrame; // 当前正在进行的动作的第几帧
    }

    /// <summary>
    /// 角色的3d模型
    /// </summary>
    public struct ArtModel
    {
        // 不知道3d模型在unity里面是什么格式 这个结构假装一下是用来存3d模型的结构好了
    }

    /// <summary>
    /// 角色立绘
    /// </summary>
    public struct Picture
    {
        // 不知道角色立绘在unity里面是什么格式 这个结构假装一下是用来角色立绘的结构好了
    }

    /// <summary>
    /// 角色动作
    /// </summary>
    public struct FightAction
    {
        public string name; // 动作名字
        public int percentageModifier; // 这个动作的伤害百分比 
        public InputKeyGroup[] inputKeyGroups; // 这个动作对应的按键输入要求 比如后-下-前-两个脚一起按在这就是个长度4的数组 
        
        public FightActionFrame[] frames; // 这个动作
        
    }
    
    /// <summary>
    /// 输入按键
    /// </summary>
    public struct InputKeyGroup
    {
        public string name;
        public KeyCode[] keys;   
    }

    /// <summary>
    /// 动作的一帧
    /// </summary>
    public struct FightActionFrame
    {
        public Transform hitBox; // 攻击框的大小和位置
        public Transform hurtBox; // 受击框的大小和位置
        
        public DefendStatus[] defendStatus; // 对哪些防御状态有效
        public int damage; // 这一帧击中的伤害
        
        public Vector3[] modelPosition; // 这个角色模型各个点的位置 美术表现需要
        
        public CancelAbility[] cancelAction; // 这帧可以跳转到的动作 顺序代表优先级 可以为空 为空时候这一帧不可被cancel
    }

    /// <summary>
    /// 打断帧
    /// </summary>
    public struct CancelAbility
    {
        public FightAction action; // 跳转到的动作
        public int frame; // 跳转到的动作的第几帧
    }
}