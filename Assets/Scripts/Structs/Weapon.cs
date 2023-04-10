using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 武器struct
/// </summary>
public struct WeaponObj
{
    /// <summary>
    /// 武器的攻击力
    /// </summary>
    public int atkPower;

    /// <summary>
    /// 武器的攻击范围最小值
    /// </summary>
    public int minRange;

    /// <summary>
    /// 武器的攻击范围最大值
    /// </summary>
    public int maxRange;

    /// <summary>
    /// 武器的攻击目标
    /// </summary>
    public byte target;

    /// <summary>
    /// 武器的攻击先后手类型
    /// </summary>
    public WeaponType weaponType;
    
    /// <summary>
    ///  武器的名字
    /// </summary>
    public string weaponName;

    /// <summary>
    /// 武器可使用次数
    /// </summary>
    public int count;

    /// <summary>
    /// 武器图标
    /// </summary>
    public string icon;

    public WeaponObj(int atkPower, int minRange, int maxRange, byte target, WeaponType weaponType, string weaponName, int count, string icon)
    {
        this.atkPower = atkPower;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.target = target;
        this.weaponType = weaponType;
        this.weaponName = weaponName;
        this.count = count;
        this.icon = icon;
    }

    
    /// <summary>
    /// 根据武器类型给出战斗行动点消耗
    /// </summary>
    /// <returns></returns>
    public AttackActionCosts GetActionCostByType()
    {
        switch (weaponType)
        {
            case WeaponType.NormalAttack:
                return new AttackActionCosts(0, 4);
            case WeaponType.DoubleAttack:
                return new AttackActionCosts(0, 4);
            case WeaponType.LaterAttack:
                return new AttackActionCosts(2, 4);
            default:
                return new AttackActionCosts(0, 4);
        }
    }
}


/// <summary>
/// 武器攻击先后手类型enum
/// </summary>
public enum WeaponType
{
    NormalAttack,
    DoubleAttack,
    LaterAttack,
}

/// <summary>
/// 武器的攻击行动力消耗
/// 包括初始化消耗和攻击消耗
/// </summary>
public struct AttackActionCosts
{
    /// <summary>
    /// 武器的初始化消耗攻击行动力点数
    /// </summary>
    public int initCost;
    /// <summary>
    /// 武器的攻击消耗攻击行动力点数
    /// </summary>
    public int attackCost;
    
    public AttackActionCosts(int initCost, int attackCost)
    {
        this.initCost = initCost;
        this.attackCost = attackCost;
    }
}