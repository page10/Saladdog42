using UnityEngine;

namespace Structs
{
    /// <summary>
    /// 给BattleManager用的战斗信息
    /// </summary>
    public struct BattleInputInfo
    {
        /// <summary>
        /// 攻击方
        /// </summary>
        public CharacterObject attacker;
        /// <summary>
        /// 被攻击方
        /// </summary>
        public CharacterObject defender;
        /// <summary>
        /// 攻击方武器
        /// </summary>
        public WeaponObj attackerWeapon;
        /// <summary>
        /// 受击方武器
        /// </summary>
        public WeaponObj defenderWeapon;
        /// <summary>
        /// 攻击方位置
        /// </summary>
        public Vector2Int attackerPos;
        /// <summary>
        /// 受击方位置
        /// </summary>
        public Vector2Int defenderPos;
        /// <summary>
        /// 攻击方所在地形
        /// </summary>
        public CharacterStatus attackerTerrainStatus;
        /// <summary>
        /// 受击方所在地形
        /// </summary>
        public CharacterStatus defenderTerrainStatus;
        /// <summary>
        /// 双方距离
        /// </summary>
        public readonly int distance => Mathf.Abs(attackerPos.x - defenderPos.x) + Mathf.Abs(attackerPos.y - defenderPos.y);
        /// <summary>
        /// 是否是同一方
        /// </summary>
        public bool isSameSide;

        public BattleInputInfo(CharacterObject attacker, CharacterObject defender, WeaponObj attackerWeapon,
            WeaponObj defenderWeapon, Vector2Int attackerPos, Vector2Int defenderPos, CharacterStatus attackerTerrainStatus, CharacterStatus defenderTerrainStatus, 
            bool isSameSide)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.attackerWeapon = attackerWeapon;
            this.defenderWeapon = defenderWeapon;
            this.attackerTerrainStatus = attackerTerrainStatus;
            this.defenderTerrainStatus = defenderTerrainStatus;
            this.isSameSide = isSameSide;
            this.attackerPos = attackerPos;
            this.defenderPos = defenderPos;
            //distance = Mathf.RoundToInt(Vector2Int.Distance(attackerPos, defenderPos));
            //distance = Mathf.Abs(attackerPos.x - defenderPos.x) + Mathf.Abs(attackerPos.y - defenderPos.y);
        }
    }

    /// <summary>
    /// BattleManager输出的战斗结果信息
    /// </summary>
    public struct BattleResInfo
    {
        public bool isAttacker;  // 是攻击还是反击
        public int damage;
        public bool isCrit;
        public bool isKill;
        public CharacterStatus attackerBuffChange;
        public CharacterStatus defenderBuffChange;
        public Vector2Int attackerPosChange;
        public Vector2Int defenderPosChange;
        public CharacterObject attacker;
        public CharacterObject defender;

        public BattleResInfo(bool isAttacker, int damage, bool isCrit, 
            bool isKill, CharacterStatus attackerBuffChange,
            CharacterStatus defenderBuffChange , Vector2Int attackerPosChange, Vector2Int defenderPosChange, 
            CharacterObject attacker, CharacterObject defender)
        {
            this.isAttacker = isAttacker;
            this.damage = damage;
            this.isCrit = isCrit;
            this.isKill = isKill;
            this.attackerBuffChange = attackerBuffChange;
            this.defenderBuffChange = defenderBuffChange;
            this.attackerPosChange = attackerPosChange;
            this.defenderPosChange = defenderPosChange;
            this.attacker = attacker;
            this.defender = defender;
        }
    }

    // /// <summary>
    // /// 一次战斗的视觉数据
    // /// </summary>
    // public struct VisualResult
    // {
    //     public string attackerFaceDirection;
    //     public string defenderFaceDirection;
    //     public string attackerAction;
    //     public string defenderAction;
    //     public string attackerEffectPath;
    //     public string defenderEffectPath;
    //     public VisualResult(string attackerFaceDirection = "", string defenderFaceDirection= "", string attackerAction= "", 
    //         string defenderAction= "", string attackerEffectPath= "", string defenderEffectPath= "")
    //     {
    //         this.attackerFaceDirection = attackerFaceDirection;
    //         this.defenderFaceDirection = defenderFaceDirection;
    //         this.attackerAction = attackerAction;
    //         this.defenderAction = defenderAction;
    //         this.attackerEffectPath = attackerEffectPath;
    //         this.defenderEffectPath = defenderEffectPath;
    //     }
    //
    // }

    /// <summary>
    /// 战斗中一方的信息
    /// </summary>
    public class BattleGuyInfo
    {
        public bool isAttacker;
        public int actPoints;
        public AttackActionCosts actionCosts;
        public CharacterStatus characterStatus;
        public CharacterObject characterObject;
        public WeaponObj weaponObj;
        public Vector2Int gPos;
        
        public BattleGuyInfo(bool isAttacker, int actPoints, AttackActionCosts actionCosts, CharacterStatus characterStatus, CharacterObject characterObject, WeaponObj weaponObj, Vector2Int gPos)
        {
            this.isAttacker = isAttacker;
            this.actPoints = actPoints;
            this.actionCosts = actionCosts;
            this.characterStatus = characterStatus;
            this.characterObject = characterObject;
            this.weaponObj = weaponObj;
            this.gPos = gPos;
        }

    }
}