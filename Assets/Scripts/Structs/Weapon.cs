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

    public WeaponObj(int atkPower, int minRange, int maxRange, byte target, WeaponType weaponType)
    {
        this.atkPower = atkPower;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.target = target;
        this.weaponType = weaponType;
    }

}

/// <summary>
/// 武器攻击先后手类型enum
/// </summary>
public enum WeaponType
{
    normalAttack,
    doubleAttack,
    laterAttack,
}
