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
        int attackerActPoints, defenderActPoints;  // 攻击方和受击方的行动力 临时变量
        // 初始化双方行动力
        attackerActPoints = 7;
        defenderActPoints = 6;
        // 计算攻击方和受击方的武器消耗
        AttackActionCosts attackActionCosts = BattleInputInfo.attackerWeapon.GetActionCostByType();
        AttackActionCosts defendActionCosts = BattleInputInfo.defenderWeapon.GetActionCostByType();
        // 计算武器初始化之后 攻击方和受击方的行动力
        
    }
    
    
    
    
    
    
    
    
    
    
    /// <summary>
    /// 计算单次战斗
    /// </summary>
    /// <param name="isAttacker">是攻击方的攻击</param>
    private void CalSingleBattle(bool isAttacker)
    {
        BattleResInfo battleResInfo = new BattleResInfo();
        battleResInfo.isAttacker = isAttacker;
        if (isAttacker)  //这次攻击来自攻击方
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
            else  // 没命中
            {
                damage = 0;
                battleResInfo.isHit = false;
            }
        }
        else  // 这次攻击来自反击方
        {
            int hit = defenderModifiedStatus.hit - attackerModifiedStatus.dodge;
            int crit = defenderModifiedStatus.crit;
            int damage = defenderModifiedStatus.attack - attackerModifiedStatus.defense;
            
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
                if (damage >= attackerModifiedStatus.hp)
                {
                    battleResInfo.isKill = true;
                    attackerModifiedStatus.hp = 0;
                }
                else
                {
                    attackerModifiedStatus.hp -= damage;
                }
            }
            else  // 没命中
            {
                damage = 0;
                battleResInfo.isHit = false;
            }
        }
        SingleBattleInfo.Add(battleResInfo);

    }
    
}