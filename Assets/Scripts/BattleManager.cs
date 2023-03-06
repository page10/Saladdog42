using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleInputInfo BattleInputInfo { get; set; }
    public List<BattleResInfo> SingleBattleInfo { get; set; }
    
    public CharacterStatus attackerModifiedStatus;  // 攻击方受武器和地形影响后的status
    public CharacterStatus defenderModifiedStatus;  // 受击方受武器和地形影响后的status

    /// <summary>
    /// 计算双方受武器和地形影响后的status
    /// </summary>
    private void CalInitStatus()
    {
        attackerModifiedStatus = BattleInputInfo.attackerStatus + BattleInputInfo.attackerTerrainStatus;
        defenderModifiedStatus = BattleInputInfo.defenderStatus + BattleInputInfo.defenderTerrainStatus;
        
        attackerModifiedStatus.attack += BattleInputInfo.attackerWeapon.atkPower;
        defenderModifiedStatus.attack += BattleInputInfo.defenderWeapon.atkPower;
    }
    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle()
    {
        CalInitStatus();
        SingleBattleInfo = new List<BattleResInfo>();
        // 判定攻击方拿的武器是不是后手武器
        if (BattleInputInfo.attackerWeapon.weaponType != WeaponType.laterAttack)
        {
            
        }
        
    }
    
    /// <summary>
    /// 计算单次战斗
    /// </summary>
    /// <param name="isAttacker">是攻击方的攻击</param>
    private void CalSingleBattle(bool isAttacker)
    {
        BattleResInfo battleResInfo = new BattleResInfo();
        if (isAttacker)
        {
            int hit = attackerModifiedStatus.hit - defenderModifiedStatus.dodge;
            int crit = attackerModifiedStatus.crit;
            int damage = attackerModifiedStatus.attack - defenderModifiedStatus.defense;
            
            // 判定是否命中
            if (hit >= Random.Range(0, 100))
            {
                // 判定是否暴击
                if (crit >= Random.Range(0, 100))
                {
                    battleResInfo.isCrit = true;
                    damage *= 3;
                }
                // 判定是否杀死
                if (damage >= defenderModifiedStatus.hp)
                {
                    battleResInfo.isKill = true;
                    defenderModifiedStatus.hp = 0;
                }
                else
                {
                    defenderModifiedStatus.hp -= damage;
                }
            }
            else
            {
                damage = 0;
                battleResInfo.isHit = false;
            }
        }

    }
    
}