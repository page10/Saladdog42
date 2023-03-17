using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleInputInfo battleInputInfo { get; set; }
    public List<BattleResInfo> SingleBattleInfo { get; set; }
    
    public CharacterStatus attackerModifiedStatus;  // 攻击方受武器和地形影响后的status
    public CharacterStatus defenderModifiedStatus;  // 受击方受武器和地形影响后的status

    /// <summary>
    /// 计算双方受武器和地形影响后的status
    /// </summary>
    private void CalInitStatus()
    {
        attackerModifiedStatus = battleInputInfo.attackerStatus + battleInputInfo.attackerTerrainStatus;
        defenderModifiedStatus = battleInputInfo.defenderStatus + battleInputInfo.defenderTerrainStatus;
        
        attackerModifiedStatus.attack += battleInputInfo.attackerWeapon.atkPower;
        defenderModifiedStatus.attack += battleInputInfo.defenderWeapon.atkPower;
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
        AttackActionCosts attackActionCosts = battleInputInfo.attackerWeapon.GetActionCostByType();
        AttackActionCosts defendActionCosts = battleInputInfo.defenderWeapon.GetActionCostByType();
        // 计算武器初始化之后 攻击方和受击方的行动力
        attackerActPoints -= attackActionCosts.initCost;
        defenderActPoints -= defendActionCosts.initCost;
        // 第一次打架
        while (true)  // 这里没有因为死亡而停止计算 但是死亡的话 会有一个死亡的结果输出 在另外的逻辑里根据这个运算实际结果
        {
            if (attackerActPoints >= defenderActPoints)  
            {
                if (attackerActPoints > 0)
                {
                    CalSingleBattle(true);
                    attackerActPoints -= attackActionCosts.attackCost;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (defenderActPoints > 0 && CanFightBack())
                {
                    CalSingleBattle(false);
                    defenderActPoints -= defendActionCosts.attackCost;
                }
                else
                {
                    break;
                }
            }
        }
        // 判定攻速差 第二次打架
        
    }

    /// <summary>
    /// 防守方是不是能反击
    /// </summary>
    /// <returns></returns>
    private bool CanFightBack()
    {
        bool inRange = battleInputInfo.defenderWeapon.maxRange >= battleInputInfo.distance &&
                       battleInputInfo.defenderWeapon.minRange <= battleInputInfo.distance;  
        // 是否在攻击范围内 debug
        // 这里有个问题 判定攻击范围 需要知道攻击方和受击方的位置或者距离
        // 在这不能调characters拿gridPos 需要外部再传一个「距离」参数
        // 把这个参数在这里和受击方的攻击范围比较
        
        bool sameSide = battleInputInfo.isSameSide;  
        // 是否同一方 debug
        // 这里也有个问题 判定是否同一方 需要知道攻击方和受击方的阵营
        // 阵营这里拿也不太好 在gameManager调的时候 比较一下attacker和defender的阵营就行了
        
        return inRange && !sameSide;
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