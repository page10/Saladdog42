using System;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public BattleInputInfo battleInputInfo { get; private set; }
    public List<BattleResInfo> SingleBattleInfo { get; set; } = new List<BattleResInfo>();
    private List<BattleGuyInfo> battleGuyInfos = new List<BattleGuyInfo>();
    
    private const int comboThreshold = 5;
    private const int attackerActPointsBase = 7;
    private const int defenderActPointsBase = 6;
    
    public bool IsPlayingAnimation { get; private set; }
    
    private List<BattleAnimNode> battleAnimTimelineNodes = new List<BattleAnimNode>();

    private void Update()
    {
        List<BattleAnimNode> toAdd = new List<BattleAnimNode>();
        int idx = 0;
        while (idx < battleAnimTimelineNodes.Count)
        {
            if (battleAnimTimelineNodes[idx].UpdateNode(Time.deltaTime))
            {
                foreach (BattleAnimData n in battleAnimTimelineNodes[idx].NextEvents)
                {
                    BattleAnimNode node = BattleAnimNode.FromBattleAnimData(n);
                    toAdd.Add(node);
                }
                battleAnimTimelineNodes.RemoveAt(idx);
            }
            else
            {
                idx++;
            }
        }

        foreach (BattleAnimNode node in toAdd)
        {
            battleAnimTimelineNodes.Add(node);
        }
        
        IsPlayingAnimation = battleAnimTimelineNodes.Count > 0;

    }

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
    public void StartBattle(BattleInputInfo battleInputInfo)
    {
        this.battleInputInfo = battleInputInfo;
        CalInitStatus();
        SingleBattleInfo.Clear();
        // // 第一次打架
        while (true) // 这里没有因为死亡而停止计算 但是死亡的话 会有一个死亡的结果输出 在另外的逻辑里根据这个运算实际结果
        {
            battleGuyInfos.Sort((BattleGuyInfo a, BattleGuyInfo b) =>  a.actPoints > b.actPoints ? -1 : 1 );
            if (battleGuyInfos[0].actPoints > battleGuyInfos[0].actionCosts.attackCost)
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
        
        GenerateHit();
    }

    /// <summary>
    /// 播动画用的接口
    /// </summary>
    public void StartBattleAnim()
    {
        if (IsPlayingAnimation) return;
        IsPlayingAnimation = true;
        battleAnimTimelineNodes.Clear();
        
        battleAnimTimelineNodes.Add(BattleAnimNode.FromBattleAnimData(GenerateBattleAnimEvent()));

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
    /// 根据生成的battleResInfo 生成动画信息
    /// </summary>
    private BattleAnimData GenerateBattleAnimEvent()
    {
        Vector2Int faceDirection = (battleInputInfo.defender.gPos.grid - battleInputInfo.attacker.gPos.grid) /
                                   Mathf.RoundToInt(Vector2Int.Distance(battleInputInfo.defender.gPos.grid,
                                       battleInputInfo.attacker.gPos.grid));
        BattleAnimData changeDirection = new BattleAnimData(new ChangeFaceDirection(battleInputInfo.attacker,faceDirection));
        BattleAnimData lastData = new BattleAnimData(new ChangeFaceDirection(battleInputInfo.defender, -faceDirection));
        changeDirection.NextEventsDatas.Add(lastData);

        for (int i = 0; i < SingleBattleInfo.Count; i++)
        {
            BattleResInfo battleResInfo = SingleBattleInfo[i];
            BattleAnimData battleAnim =
                new BattleAnimData(new CharacterDoAction(battleResInfo.attacker, CharacterDoAction.Attack));
            lastData.NextEventsDatas.Add(battleAnim);
            lastData = battleAnim;
            
            // 每个动作一开始都要wait0.2 无论有没有命中 
            // todo 之后等的这个攻击动画时间要配置
            battleAnim = new BattleAnimData(new Wait(0.5f)); // 等待0.5s 先这么写 之后读取
            lastData.NextEventsDatas.Add(battleAnim);
            lastData = battleAnim;
            
            if (battleResInfo.isHit)
            {
                battleAnim = new BattleAnimData(new CharacterDoAction(battleResInfo.defender, CharacterDoAction.Hurt));
                lastData.NextEventsDatas.Add(battleAnim);
                lastData = battleAnim;
                BattleAnimData battleAnim3 = new BattleAnimData(new PopText(battleResInfo.defender,
                    battleResInfo.damage.ToString(), PopText.Hit));
                lastData.NextEventsDatas.Add(battleAnim3);

                // 这里还要加上一个判定是否死亡的逻辑
                // 如果死亡了 就要加上一个死亡的动画
                if (battleResInfo.isKill)
                {
                    battleAnim = new BattleAnimData(new Wait(0.5f)); // 等待0.5s 先这么写 之后读取
                    lastData.NextEventsDatas.Add(battleAnim);
                    lastData = battleAnim;

                    battleAnim = new BattleAnimData(new RemoveCharacter(battleResInfo.defender));
                    lastData.NextEventsDatas.Add(battleAnim);
                    lastData = battleAnim;

                    break;
                }
            }
            else
            {
                BattleAnimData battleAnim2 =
                    new BattleAnimData(new PopText(battleResInfo.defender, "miss", PopText.Miss));
                lastData.NextEventsDatas.Add(battleAnim2);
                lastData = battleAnim2;
            }
        }
        // 打完了 转回去
        // 这里先写俩人都播转回去的动画 
        // 播的时候判断一下是不是还存在 如果已经不存在了就不用播了
        
        BattleAnimData battleAnim4 = new BattleAnimData(new ChangeFaceDirection(battleInputInfo.attacker, ChangeFaceDirection.Default));
        lastData.NextEventsDatas.Add(battleAnim4);
        BattleAnimData battleAnim5 = new BattleAnimData(new ChangeFaceDirection(battleInputInfo.defender, ChangeFaceDirection.Default));
        lastData.NextEventsDatas.Add(battleAnim5);
        BattleAnimData battleAnim6 = new BattleAnimData(new Wait(0.2f));
        battleAnim4.NextEventsDatas.Add(battleAnim6);
         
        return changeDirection;
    }

    /// <summary>
    /// 把伤害和状态修改施加到角色身上
    /// 执行动画播放
    /// </summary>
    private void GenerateHit()
    {
        // 对游戏的逻辑数据进行修改 把数据修改到gameManager里的角色身上
        // todo 死了之后如何移除？
        // 死亡之后的逻辑判定一点都还没写
        // 涉及到对人操作的逻辑之前都要判断一下死亡 
        // 直接修改传入的battleInputInfo里的attacker和defender就可以 因为是类 引用类型
        foreach (var battleResInfo in SingleBattleInfo)
        {
            if (battleResInfo.isAttacker)  // 攻击方打的
            {
                battleInputInfo.defender.Status.hp -= battleResInfo.damage;
                if (battleResInfo.isKill)
                {
                    battleInputInfo.defender.Status.hp = 0;
                    break;  // 任意一方战死则本次战斗结束
                }
            }
            else  // 反击方打的
            {
                battleInputInfo.attacker.Status.hp -= battleResInfo.damage;
                if (battleResInfo.isKill)
                {
                    battleInputInfo.attacker.Status.hp = 0;
                    break;  // 任意一方战死则本次战斗结束
                }
            }
        }
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
        Debug.Log("hit:" + thisAttackStatus.hit);
        Debug.Log("dodge:" + thisAttackStatus.dodge);
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
            if (damage >= defender.characterStatus.hp)
            {
                isKill = true;
                defender.characterStatus.hp = 0;
            }
            else
            {
                defender.characterStatus.hp -= damage;
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