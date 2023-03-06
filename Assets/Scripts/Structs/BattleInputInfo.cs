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
        public CharacterStatus attackerStatus;
        /// <summary>
        /// 被攻击方
        /// </summary>
        public CharacterStatus defenderStatus;
        /// <summary>
        /// 攻击方武器
        /// </summary>
        public WeaponObj attackerWeapon;
        /// <summary>
        /// 受击方武器
        /// </summary>
        public WeaponObj defenderWeapon;
        /// <summary>
        /// 攻击方所在地形
        /// </summary>
        public CharacterStatus attackerTerrainStatus;
        /// <summary>
        /// 受击方所在地形
        /// </summary>
        public CharacterStatus defenderTerrainStatus;
        
        public BattleInputInfo(CharacterStatus attackerStatus, CharacterStatus defenderStatus, WeaponObj attackerWeapon,
            WeaponObj defenderWeapon, CharacterStatus attackerTerrainStatus, CharacterStatus defenderTerrainStatus)
        {
            this.attackerStatus = attackerStatus;
            this.defenderStatus = defenderStatus;
            this.attackerWeapon = attackerWeapon;
            this.defenderWeapon = defenderWeapon;
            this.attackerTerrainStatus = attackerTerrainStatus;
            this.defenderTerrainStatus = defenderTerrainStatus;
        }
    }

    /// <summary>
    /// BattleManager输出的战斗结果信息
    /// </summary>
    public struct BattleResInfo
    {
        public bool isAttacker;  // 是攻击还是反击
        public int damage;
        public bool isHit;
        public bool isCrit;
        public bool isKill;
        public CharacterStatus attackerBuffChange;
        public CharacterStatus defenderBuffChange;
        public Vector2Int attackerPos;
        public Vector2Int defenderPos;
        public VisualResult visualResult;

        public BattleResInfo(bool isAttacker, int damage, bool isHit, bool isCrit, bool isKill,
            CharacterStatus attackerBuffChange, CharacterStatus defenderBuffChange, Vector2Int attackerPos,
            Vector2Int defenderPos, VisualResult visualResult)
        {
            this.isAttacker = isAttacker;
            this.damage = damage;
            this.isHit = isHit;
            this.isCrit = isCrit;
            this.isKill = isKill;
            this.attackerBuffChange = attackerBuffChange;
            this.defenderBuffChange = defenderBuffChange;
            this.attackerPos = attackerPos;
            this.defenderPos = defenderPos;
            this.visualResult = visualResult;
        }
    }

    /// <summary>
    /// 一次战斗的视觉数据
    /// </summary>
    public struct VisualResult
    {
        public string attackerFaceDirection;
        public string defenderFaceDirection;
        public string attackerAction;
        public string defenderAction;
        public string attackerEffectPath;
        public string defenderEffectPath;

    }
}