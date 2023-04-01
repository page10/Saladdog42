using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleInputInfo battleInputInfo { get; set; }
    public List<BattleResInfo> SingleBattleInfo { get; set; }
    private List<BattleGuyInfo> battleGuyInfos = new List<BattleGuyInfo>();
    
    private const int comboThreshold = 5;
    private const int attackerActPointsBase = 7;
    private const int defenderActPointsBase = 6;

    /// <summary>
    /// 计算双方受武器和地形影响后的status
    /// </summary>
    private void CalInitStatus()
    {
        battleGuyInfos.Clear();

        // 双方受武器和地形影响后的status
        CharacterStatus attackerModifiedStatus = battleInputInfo.attacker.Status + battleInputInfo.attackerTerrainStatus;
        CharacterStatus defenderModifiedStatus = battleInputInfo.defender.Status + battleInputInfo.defenderTerrainStatus;

        attackerModifiedStatus.attack += battleInputInfo.attackerWeapon.atkPower;
        defenderModifiedStatus.attack += battleInputInfo.defenderWeapon.atkPower;

        battleGuyInfos.Add(
            new BattleGuyInfo(
                true,
                attackerActPointsBase - battleInputInfo.attackerWeapon.GetActionCostByType().initCost,
                battleInputInfo.attackerWeapon.GetActionCostByType(),
                attackerModifiedStatus,battleInputInfo.attacker,
                battleInputInfo.attackerWeapon,battleInputInfo.attackerPos
            )
        );
        battleGuyInfos.Add(
            new BattleGuyInfo(
                false,
                CanFightBack() ? (defenderActPointsBase - battleInputInfo.defenderWeapon.GetActionCostByType().initCost) : 0,
                battleInputInfo.defenderWeapon.GetActionCostByType(),
                defenderModifiedStatus,battleInputInfo.defender,
                battleInputInfo.defenderWeapon,battleInputInfo.defenderPos
            )
        );
    }

    /// <summary>
    /// 开始战斗 获得完整的战斗信息
    /// </summary>
    public void StartBattle()
    {
        CalInitStatus();
        // // 第一次打架
        while (true) // 这里没有因为死亡而停止计算 但是死亡的话 会有一个死亡的结果输出 在另外的逻辑里根据这个运算实际结果
        {
            battleGuyInfos.Sort((BattleGuyInfo a, BattleGuyInfo b) =>  a.actPoints > b.actPoints ? -1 : 1 );
            if (battleGuyInfos[0].actPoints > 0)
            {
                FightOnce(battleGuyInfos[0], battleGuyInfos[1]);
                battleGuyInfos[0].actPoints -= battleGuyInfos[0].actionCosts.attackCost;
            }
            else
            {
                break;
            }
        }

        // 判定攻速差 第二次打架
        int speedDiff = battleGuyInfos[0].characterStatus.speed - battleGuyInfos[1].characterStatus.speed;
        if (Mathf.Abs(speedDiff) >= comboThreshold) // 有一方速度大于5
        {
            int attackerIndex = speedDiff > 0 ? 0 : 1;
            // 第2次打架 不存在反击了 也不用再初始化双方行动力 直接让能二动的人打
            FightOnce(
                battleGuyInfos[attackerIndex],
                battleGuyInfos[1 - attackerIndex] // 讨巧获得了另一方下标
            );
        }
    }

    private void FightOnce(BattleGuyInfo attackGuyInfo, BattleGuyInfo defendGuyInfo)

    {
        int times = attackGuyInfo.weaponObj.weaponType == WeaponType.DoubleAttack ? 2 : 1;
        for (int i = 0; i < times; i++)
        {
            SingleBattleInfo.Add(CalDamage(attackGuyInfo, defendGuyInfo));
        }
    }

    /// <summary>
    /// 根据生成的battleResInfo 把伤害和状态修改施加到角色身上
    /// </summary>
    public BattleAnimEvent GenerateHit()
    {
        SingleBattleInfo = new List<BattleResInfo>();
        Vector2Int faceDirection = (battleInputInfo.defender.gPos.grid - battleInputInfo.attacker.gPos.grid) /
                                   Mathf.RoundToInt(Vector2Int.Distance(battleInputInfo.defender.gPos.grid,
                                       battleInputInfo.attacker.gPos.grid));
        BattleAnimEvent changeDirection = new BattleAnimEvent(new ChangeFaceDirection(battleInputInfo.attacker,faceDirection));
        BattleAnimEvent lastEvent = new BattleAnimEvent(new ChangeFaceDirection(battleInputInfo.defender, -faceDirection));
        changeDirection.NextEvents.Add(lastEvent);

        for (int i = 0; i < SingleBattleInfo.Count; i++)
        {
            BattleResInfo battleResInfo = SingleBattleInfo[i];
            BattleAnimEvent battleAnim =
                new BattleAnimEvent(new CharacterDoAction(battleResInfo.attacker, CharacterDoAction.Attack));
            lastEvent.NextEvents.Add(battleAnim);
            lastEvent = battleAnim;
            if (battleResInfo.isHit)
            {
                battleAnim = new BattleAnimEvent(new Wait(0.2f)); // 等待0.5s 先这么写 之后读取
                lastEvent.NextEvents.Add(battleAnim);
                lastEvent = battleAnim;
                battleAnim = new BattleAnimEvent(new CharacterDoAction(battleResInfo.defender, CharacterDoAction.Hurt));
                lastEvent.NextEvents.Add(battleAnim);
                lastEvent = battleAnim;
                BattleAnimEvent battleAnim3 = new BattleAnimEvent(new PopText(battleResInfo.defender,
                    battleResInfo.damage.ToString(), PopText.Hit));
                lastEvent.NextEvents.Add(battleAnim3);

                // 这里还要加上一个判定是否死亡的逻辑
                // 如果死亡了 就要加上一个死亡的动画
                if (battleResInfo.isKill)
                {
                    battleAnim = new BattleAnimEvent(new Wait(0.2f)); // 等待0.5s 先这么写 之后读取
                    lastEvent.NextEvents.Add(battleAnim);
                    lastEvent = battleAnim;

                    battleAnim = new BattleAnimEvent(new RemoveCharacter(battleResInfo.defender));
                    lastEvent.NextEvents.Add(battleAnim);
                    lastEvent = battleAnim;

                    break;
                }
            }
            else
            {
                BattleAnimEvent battleAnim2 =
                    new BattleAnimEvent(new PopText(battleResInfo.defender, "miss", PopText.Miss));
                lastEvent.NextEvents.Add(battleAnim2);
                lastEvent = battleAnim2;
            }
        }
        // 打完了 转回去
        // 这里先写俩人都播转回去的动画 
        // 播的时候判断一下是不是还存在 如果已经不存在了就不用播了
        
        BattleAnimEvent battleAnim4 = new BattleAnimEvent(new ChangeFaceDirection(battleInputInfo.attacker, ChangeFaceDirection.Default));
        lastEvent.NextEvents.Add(battleAnim4);
        BattleAnimEvent battleAnim5 = new BattleAnimEvent(new ChangeFaceDirection(battleInputInfo.defender, ChangeFaceDirection.Default));
        lastEvent.NextEvents.Add(battleAnim5);
        BattleAnimEvent battleAnim6 = new BattleAnimEvent(new Wait(0.2f));
        battleAnim4.NextEvents.Add(battleAnim6);
         
        return changeDirection;
    }

    /// <summary>
    /// 防守方是不是能反击
    /// </summary>
    /// <returns></returns>
    private bool CanFightBack()
    {
        // 是否在攻击范围内 debug
        // 这里有个问题 判定攻击范围 需要知道攻击方和受击方的位置或者距离
        // 在这不能调characters拿gridPos 需要外部再传一个「距离」参数
        // 把这个参数在这里和受击方的攻击范围比较
        bool inRange = battleInputInfo.defenderWeapon.maxRange >= battleInputInfo.distance &&
                       battleInputInfo.defenderWeapon.minRange <= battleInputInfo.distance;
        // 是否同一方 debug
        // 这里也有个问题 判定是否同一方 需要知道攻击方和受击方的阵营
        // 阵营这里拿也不太好 在gameManager调的时候 比较一下attacker和defender的阵营就行了
        bool sameSide = battleInputInfo.isSameSide;
        return inRange && !sameSide;
    }


    // /// <summary>
    // /// 计算单次战斗
    // /// </summary>
    // /// <param name="isAttacker">是攻击方的攻击</param>
    // private BattleResInfo CalSingleBattle(bool isAttacker)
    // {
    //     if (isAttacker) //这次攻击来自攻击方
    //     {
    //         return CalDamage(attackerModifiedStatus, defenderModifiedStatus, true);
    //     }
    //     else // 这次攻击来自反击方
    //     {
    //         return CalDamage(defenderModifiedStatus, attackerModifiedStatus, false);
    //     }
    // }

    /// <summary>
    /// 一次伤害计算流程
    /// </summary>
    /// <param name="attacker">本次出手攻击人</param>
    /// <param name="defender">本次防御人</param>
    private BattleResInfo CalDamage(BattleGuyInfo attacker, BattleGuyInfo defender)
    {
        CharacterStatus thisAttackStatus = attacker.characterStatus;
        CharacterStatus thisDefendStatus = defender.characterStatus;
        bool isAttacker = attacker.isAttacker;

        int hit = thisAttackStatus.hit - thisDefendStatus.dodge;
        int crit = thisAttackStatus.crit;
        int damage = thisAttackStatus.attack - thisDefendStatus.defense;
        bool isCrit = false;
        bool isKill = false;
        bool isHit = true;

        // 判定是否命中
        if (hit >= Random.Range(0, 100))
        {
            // 判定是否暴击
            if (crit >= Random.Range(0, 100))
            {
                isCrit = true;
                damage *= 3;
            }

            // 判定是否杀死
            if (damage >= thisDefendStatus.hp)
            {
                isKill = true;
                thisDefendStatus.hp = 0;
            }
            else
            {
                thisDefendStatus.hp -= damage;
            }
        }
        else // 没命中
        {
            damage = 0;
            isHit = false;
        }

        CharacterStatus attackerBuffChange = new CharacterStatus(); // 先占个坑 攻击方buff变化
        CharacterStatus defenderBuffChange = new CharacterStatus(); // 先占个坑 受击方buff变化
        Vector2Int attackerPosChange = new Vector2Int(0, 0); // 先占个坑 攻击方位置变化
        Vector2Int defenderPosChange = new Vector2Int(0, 0); // 先占个坑 受击方位置变化

        BattleResInfo battleResInfo = new BattleResInfo(isAttacker, damage, isHit, isCrit, isKill, attackerBuffChange,
            defenderBuffChange, attackerPosChange, defenderPosChange, attacker.characterObject, defender.characterObject);
        

        
        
        return battleResInfo;
    }


}